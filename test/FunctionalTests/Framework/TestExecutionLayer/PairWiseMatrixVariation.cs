//---------------------------------------------------------------------
// <copyright file="PairWiseMatrixVariation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Test.ModuleCore;
using System.Diagnostics;
using System.Data.Test.Astoria;

namespace Microsoft.Test.ModuleCore {

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PWMatrixVariationAttribute : MatrixVariationAttribute {

        public static object UNSET = new int[] { };

        public override List<TestVariation> EnumerateVariations(TestCase testcase, MethodInfo method) {

            // build data 
            var columnAttrs = method.GetCustomAttributes(typeof(MatrixColumnAttribute), false) as MatrixColumnAttribute[];
            Debug.Assert(columnAttrs.Length > 0);

            object[][] inputMatrix = new object[columnAttrs.Length][];

            Dictionary<string, int> columnName2PositionMap = new Dictionary<string, int>();
            foreach (var inputColumn in columnAttrs) {
                #region Asserts
                Debug.Assert(inputMatrix[inputColumn.ParamOrderId] == null);
                Debug.Assert(!columnName2PositionMap.ContainsKey(inputColumn.Name));
                Debug.Assert(!columnName2PositionMap.ContainsValue(inputColumn.ParamOrderId));
                #endregion
                inputMatrix[inputColumn.ParamOrderId] = inputColumn.Values;
                columnName2PositionMap[inputColumn.Name] = inputColumn.ParamOrderId;
            }



            // Build Skip filters
            List<object[]> skipConstraints = CreateSkipConstraints(method, columnName2PositionMap);

            // Build Priority filters + default filter
            Dictionary<int, List<object[]>> priFilters = CreatePriorityConstraints(method, Priority, columnName2PositionMap);

            // Create Pairwise combinations based on the pri filters
            Dictionary<int, List<object[]>> combinations = Pairwise(inputMatrix, skipConstraints, priFilters);

            List<TestVariation> variations = new List<TestVariation>();
            int variationCounter = 0;

            //string orderedColumnNames = string.Concat(columnName2PositionMap.OrderBy(p => p.Value).Select((p, i) => i == 0 ? p.Key : string.Concat(",", p.Key)).ToArray());

            foreach (int priority in combinations.Keys) {
                foreach (object[] line in combinations[priority]) {
                    string name = GetVariationName(columnName2PositionMap, line);
                    TestVariation tv = new TestVariation(name, (TestFunc)Delegate.CreateDelegate(typeof(TestFunc), testcase, method));
                    tv.Priority = priority;

                    //// the last element in params will contain the parameter names (we need it to be able perform filters on the silverlight side);
                    //object[] linePlusNames = new object[line.Length + 1];
                    //Array.Copy(line, linePlusNames, line.Length);
                    //linePlusNames[line.Length] = orderedColumnNames ;
                    //tv.Params = linePlusNames;

                    tv.Params = line;
                    tv.Id = ++variationCounter;
                    variations.Add(tv);
                }
            }

            return variations;
        }

        private static Dictionary<int, List<object[]>> CreatePriorityConstraints(MethodInfo method, int MaxPriority, Dictionary<string, int> columnName2PositionMap) {
            int dim = columnName2PositionMap.Count;
            Dictionary<int, List<object[]>> skipConstraints = new Dictionary<int, List<object[]>>();
            foreach (var priAttr in method.GetCustomAttributes(typeof(MinPriorityRequirementAttribute), false) as MinPriorityRequirementAttribute[]) {
                #region Asserts
                Debug.Assert(priAttr.FilterValues != null);
                Debug.Assert(priAttr.FilterValues.Length != 0);
                #endregion
                // Add them to the priority dictionary
                if (!skipConstraints.ContainsKey(Math.Min(priAttr.Priority, MaxPriority))) skipConstraints.Add(Math.Min(priAttr.Priority, MaxPriority), new List<object[]>());
                skipConstraints[Math.Min(priAttr.Priority, MaxPriority)].AddRange(UnfoldList(priAttr.FilterColumns, priAttr.FilterValues, columnName2PositionMap));
            }

            // Add the default filter for max priority of the matrix variation
            if (!skipConstraints.ContainsKey(MaxPriority)) skipConstraints.Add(MaxPriority, new List<object[]>());
            skipConstraints[MaxPriority].Add(newUnsetRow(dim));

            return skipConstraints;
        }

        private static List<object[]> CreateSkipConstraints(MethodInfo method, Dictionary<string, int> columnName2PositionMap) {
            int dim = columnName2PositionMap.Count;
            List<object[]> skipConstraints = new List<object[]>();

            foreach (var skipAttr in method.GetCustomAttributes(typeof(MatrixSkipConstraintAttribute), false) as MatrixSkipConstraintAttribute[]) {
                #region Asserts
                Debug.Assert(skipAttr.SkipValues != null);
                Debug.Assert(skipAttr.SkipValues.Length != 0);
                #endregion
                skipConstraints.AddRange(UnfoldList(skipAttr.SkipColumns, skipAttr.SkipValues, columnName2PositionMap));
            }
            return skipConstraints;
        }

        public static Dictionary<int, List<object[]>> Pairwise(object[][] data, List<object[]> skipFilters, Dictionary<int, List<object[]>> priFilters) {
            int dim = data.Length;
            #region Asserts
            Debug.Assert(skipFilters.All(f => f.Length == dim), "skipFilters dimmensions");
            Debug.Assert(priFilters.SelectMany(pf => pf.Value).All(f => f.Length == dim), "priFilters dimmensions");
            #endregion

            // OPEN ISSUE: priorities normalization/ prioritization
            Dictionary<int, List<object[]>> retValue = new Dictionary<int, List<object[]>>();
            foreach (var filter in priFilters.OrderBy(pf => pf.Key).SelectMany(p => p.Value.Select(l => new { Pri = p.Key, Val = l }))) {
                var presentData = retValue.SelectMany(p => p.Value).Where(l => isFilterHit(l, filter.Val));
                for (int x = 0; x < dim; x++) {
                    for (int y = x + 1; y < dim; y++) {
                        foreach (object xVal in data[x]) {
                            if (!isFilterMatch(filter.Val, x, xVal)) continue;
                            foreach (object yVal in data[y]) {
                                if (!isFilterMatch(filter.Val, y, yVal)) continue;
                                // Find whethere there is a candidate (x match, y match, both match)
                                var candidate = presentData.Where(l => isFilterMatch(l, x, xVal) && isFilterMatch(l, y, yVal)).FirstOrDefault();
                                if (candidate != null) {
                                    //Console.WriteLine("Match found.");
                                    if (skipFilters.Any(f => isFilterHit(candidate, new int[] { x, y }, new object[] { xVal, yVal }, f))) {
                                        //Console.WriteLine("Candidate did not pass the filter!");
                                    }
                                    else {
                                        candidate[x] = xVal;
                                        candidate[y] = yVal;
                                        // TODO: Push up the priorities if needed
                                    }
                                    continue;
                                }

                                // Did not find anything, create new line
                                object[] newRow = newUnsetRow(filter.Val);
                                newRow[x] = xVal;
                                newRow[y] = yVal;
                                if (skipFilters.Any(f => isFilterHit(newRow, f))) {
                                    //Console.WriteLine("New Line cannot be created due to skip constraints");
                                    continue;
                                }

                                if (!retValue.ContainsKey(filter.Pri)) retValue.Add(filter.Pri, new List<object[]>());
                                //Console.WriteLine("New Line created");
                                retValue[filter.Pri].Add(newRow);
                            }
                        }
                    }
                }
            }

            // Fill in holes for UNSET values
            foreach (List<object[]> list4Pri in retValue.OrderBy(p => p.Key).Select(p => p.Value)) {
                foreach (object[] line in list4Pri.Where(x => x.Any(e => e == UNSET)).ToList()) {
                    object[] randGenerated = PossibleValues(data, line).FirstOrDefault(v => !skipFilters.Any(f => isFilterHit(v, f)));
                    if (randGenerated == null) {
                        list4Pri.Remove(line);
                        //Console.WriteLine("No match found");
                    }
                    else {
                        Array.Copy(randGenerated, line, dim);
                    }
                }
            }

            return retValue;
        }

        #region Helpers
        private static IEnumerable<object[]> UnfoldList(string[] foldedColumnNames, object[] foldedValues, Dictionary<string, int> columnName2PositionMap) {
            int dim = columnName2PositionMap.Count;
            IEnumerable<object[]> buildList = new List<object[]>();

            // make single values to be arrays of single element
            var filterValues = foldedValues.Select<object, object[]>(s => s is object[] ? s as object[] : new object[] { s }).ToArray();

            // create first set of rows
            foreach (object val in (object[])(filterValues[0])) {
                object[] line = new object[dim];
                for (int j = 0; j < line.Length; j++) line[j] = UNSET;
                line[columnName2PositionMap[foldedColumnNames[0]]] = val;
                ((List<object[]>)buildList).Add(line);
            }

            // multiply them by all the following column combinations
            for (int i = 1; i < filterValues.Length; i++) {
                buildList = buildList.SelectMany(list => filterValues[i].Select(val => {
                    object[] newLine = new object[dim];
                    Array.Copy(list, newLine, newLine.Length);
                    newLine[columnName2PositionMap[foldedColumnNames[i]]] = val;
                    return newLine;
                })).ToList();
            }

            return buildList;
        }

        private static object[] newUnsetRow(int dim) {
            object[] newRow = new object[dim];
            for (int i = 0; i < dim; i++) newRow[i] = UNSET;
            return newRow;
        }

        private static object[] newUnsetRow(object[] pattern) {
            object[] newRow = new object[pattern.Length];
            Array.Copy(pattern, newRow, pattern.Length);
            return newRow;
        }

        private static bool isFilterMatch(object[] filter, int position, object value) {
            if (filter[position] == UNSET) return true;
            if (filter[position] == value) return true;
            if (value != null && value.Equals(filter[position])) return true;
            return false;
        }

        private static bool isFilterHit(object[] data, object[] filter) {
            return isFilterHit(data, new int[] { }, new object[] { }, filter);
        }

        private static bool isFilterHit(object[] origData, int[] pos, object[] candidateValue, object[] filter) {
            #region Asserts
            Debug.Assert(origData.Length == filter.Length);
            Debug.Assert(pos.Length == candidateValue.Length);
            Debug.Assert(pos.All(p => p >= 0));
            Debug.Assert(pos.All(p => p < origData.Length));
            #endregion

            for (int i = 0; i < origData.Length; i++) {
                object data = origData[i];
                for (int j = 0; j < pos.Length; j++) {
                    if (i == pos[j]) {
                        data = candidateValue[j];
                        break;
                    }
                }
                if (!isFilterMatch(filter, i, data)) return false;
            }
            return true;
        }

        public static IEnumerable<object[]> PossibleValues(object[][] data, object[] line) {
            Debug.Assert(line.Length == data.Length);
            int dim = line.Length;
            int[] counters = new int[dim];
            for (int i = 0; i < dim; i++) {
                if (line[i] == UNSET) {
                    //data[i] = Shuffle(data[i], rand);
                }
                else {
                    counters[i] = -1;
                }
            }

            Func<object[]> generateLine = () => {
                object[] retLine = new object[dim];
                for (int j = 0; j < dim; j++) {
                    retLine[j] = counters[j] == -1 ? line[j] : data[j][counters[j]];
                }
                return retLine;
            };

            yield return generateLine();

            while (true) {
                int pos = 0;
                while (true) {
                    if (counters[pos] == -1) {
                        pos++;
                    }
                    else {
                        counters[pos]++;
                        if (counters[pos] % data[pos].Length == 0) {
                            counters[pos] = 0;
                            pos++;
                        }
                        else {
                            break;
                        }
                    }
                    if (pos >= dim) yield break;
                }
                yield return generateLine();
            }
        }

        private void NameFromArray(object[] data, StringBuilder sb) {
            int firstComma = 0;
            sb.Append('{');
            foreach (object d in data) {
                if (firstComma++ > 0) sb.Append(" ,");
                object[] da = d as object[];
                if (da == null) {
                    sb.Append((d ?? "NULL").ToString());
                }
                else {
                    NameFromArray(da, sb);
                }
            }
            sb.Append('}');
        }

        private string GetVariationName(Dictionary<string, int> columnName2PositionMap, object[] combination) {
            StringBuilder sb = new StringBuilder(PrefixName);
            sb.Append(" : ");
            foreach (var l in columnName2PositionMap) {
                sb.Append(l.Key);
                sb.Append(":");
                if (combination[l.Value] is object[]) {
                    NameFromArray(combination[l.Value] as object[], sb);
                }
                else {
                    sb.Append((combination[l.Value] ?? "NULL").ToString());
                }
                sb.Append(", ");
            }
            string name = sb.ToString().Substring(0, sb.ToString().Length - 2);
            return name;
        }
        #endregion
    }

    //[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    //public class MatrixColumnAttribute : Attribute {
    //    public string Name { get; set; }
    //    public object[] Values { get; set; }
    //    public int ParamOrderId { get; set; }
    //}

    //[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    //public class MatrixSkipConstraintAttribute : Attribute {
    //    public string[] SkipColumns { get; set; }
    //    public object[] SkipValues { get; set; }
    //}

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MinPriorityRequirementAttribute : Attribute {
        public string[] FilterColumns { get; set; }
        public object[] FilterValues { get; set; }
        public int Priority { get; set; }
    }
}
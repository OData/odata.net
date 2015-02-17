//---------------------------------------------------------------------
// <copyright file="MatrixVariation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Microsoft.Test.ModuleCore
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MatrixVariationAttribute:Attribute
    {
        //Data
        private string _prefixName;
        private int _startingId = 0;
        private int _priority =2;
		//Constructors
		public			MatrixVariationAttribute() 
			: base()
		{
		}

        public string PrefixName
        {
            get { return _prefixName; }
            set { _prefixName = value; }
        }
        public int StartingId
        {
            get { return _startingId; }
            set { _startingId = value; }
        }
        public virtual int Priority
        {
            get { return _priority;  }
            set { _priority = value; }
        }
        
        private static int CompareMatrixColumnParamId(MatrixColumnAttribute column1, MatrixColumnAttribute column2)
        {
            if (column1.ParamOrderId > column2.ParamOrderId)
                return 1;
            else if (column1.ParamOrderId == column2.ParamOrderId)
                return 0;
            else
                return -1;

        }
        public virtual List<TestVariation> EnumerateVariations(TestCase testcase, MethodInfo method)
        {
            int id= StartingId;
            List<object[]> rows = new List<object[]>();
            List<MatrixColumnAttribute> matrixColumns = new List<MatrixColumnAttribute>();
            object []attributes = method.GetCustomAttributes(false);
            List<MatrixSkipConstraintAttribute> skipAttributes = new List<MatrixSkipConstraintAttribute>();
            List<MatrixPriorityFilterAttribute> priorityAttributes = new List<MatrixPriorityFilterAttribute>();

            foreach (MatrixColumnAttribute matrixColumn in attributes.OfType < MatrixColumnAttribute>())
            {
                matrixColumns.Insert(0,matrixColumn);
            }
            foreach (MatrixSkipConstraintAttribute skipConstraint in attributes.OfType<MatrixSkipConstraintAttribute>())
            {
                skipAttributes.Insert(0, skipConstraint);
            }
            foreach (MatrixPriorityFilterAttribute filter in attributes.OfType<MatrixPriorityFilterAttribute>())
            {
                // TODO: why aren't we just doing add? or .ToList()?
                priorityAttributes.Insert(0, filter);
            }

            //Verify that no ParamOrderId's match
            IEnumerable<IGrouping<int, MatrixColumnAttribute>> grouping = matrixColumns.GroupBy<MatrixColumnAttribute, int>(mc => mc.ParamOrderId);
            foreach (IGrouping<int, MatrixColumnAttribute> group in grouping)
            {
                if (group.Key != -1)
                {
                    if (group.Count() > 1)
                        throw new ArgumentException("For Matrix variation:" + this.PrefixName + " test method:" + method.Name + " Multiple MatrixColumnAttributes have the same ParamOrderId, this is invalid");
                }
            }
            //Sort it by ParamOrderId
            matrixColumns.Sort(CompareMatrixColumnParamId);
            foreach (MatrixColumnAttribute matrixColumn in matrixColumns)
            {
                rows = MultiplyMatrixRowsByColumn(matrixColumn, rows);
            }
            List<TestVariation> variations = new List<TestVariation>();
            foreach(object[] matrixRow in rows)
            {
                //Every method that has a [Variation attribute] = a variation
		        //Add this variation to our array
                TestFunc func = null;
		        try
                {
                    func = (TestFunc)Delegate.CreateDelegate(typeof(TestFunc), testcase, method);
                }
                catch(Exception e)
                {
                    e = new TestFailedException("Variation: '" + method + "' doesn't match expected signature of 'void func()', unable to add that variation.", null, null, e);
                    TestLog.HandleException(e);
                    continue;
                }
                int i = 0;
                string description = null;
                //Calling from the list to reverse the attribute order so its in the order
                //as its in the code
                foreach (MatrixColumnAttribute matrixColumn in matrixColumns)
                {
                    if (description == null)
                        description = this.PrefixName + " " + matrixColumn.Name + ":" + matrixRow[i].ToString();
                    else
                        description = description + " " + matrixColumn.Name + ":" + matrixRow[i].ToString();
                    i++;
                }
                if (!ShouldSkipVariation(testcase, matrixColumns, skipAttributes, matrixRow))
                {
                    TestVariation var = new TestVariation(description, func);

                    var.Params = matrixRow;
                    var.Id = id;
                    var.Desc = description;
                    var.Priority = VariationPriority(testcase, matrixColumns, priorityAttributes, matrixRow);
                    variations.Add(var);
                    id++;
                }
            }
            return variations;
        }

        private bool ShouldSkipVariation(TestCase testCase, List<MatrixColumnAttribute> matrixColumns, List<MatrixSkipConstraintAttribute> skipConstraints, object[] matrixRow)
        {
            foreach (MatrixSkipConstraintAttribute skipConstraint in skipConstraints)
            {
                if (IsMatchAcrossRow(testCase, matrixColumns, matrixRow, skipConstraint.SkipColumns, skipConstraint.SkipValues))
                    return true;
            }
            return false;
        }

        private bool IsMatchAcrossRow(TestCase testCase, List<MatrixColumnAttribute> matrixColumns, object[] matrixRow, string[] filterNames, object[] filterValues)
        {
            bool allEqual = true;
            for (int i = 0; i < filterNames.Count(); i++)
            {
                string columnName = filterNames[i];
                object columnValue = filterValues[i];

                bool found = false;
                for (int j = 0; j < matrixColumns.Count; j++)
                {
                    if (matrixColumns[j].Name == columnName)
                    {
                        found = true;
                        if (!matrixRow[j].Equals(columnValue))
                            allEqual = false;
                        break;
                    }
                }
                if (!found)
                {
                    if (columnName.StartsWith("Parent.") && columnName.EndsWith(".Param"))
                    {
                        TestItem test = testCase;
                        string name = columnName;
                        while ((name = name.Remove(0, "Parent.".Length)).StartsWith("Parent."))
                            test = test.Parent;
                        if (name == "Param")
                        {
                            if (!test.Param.Equals(columnValue))
                                allEqual = false;
                        }
                        else
                        {
                            throw new TestFailedException("Could not parse inherited param name: " + columnName);
                        }
                    }
                    else
                    {
                        throw new TestFailedException("Could not find column: " + columnName);
                    }
                }
            }
            return allEqual;
        }

        private int VariationPriority(TestCase testCase, List<MatrixColumnAttribute> matrixColumns, List<MatrixPriorityFilterAttribute> priorityFilters, object[] matrixRow)
        {
            int maxPriority = this.Priority;
            foreach (MatrixPriorityFilterAttribute filter in priorityFilters)
            {
                if (IsMatchAcrossRow(testCase, matrixColumns, matrixRow, filter.FilterColumns, filter.FilterValues))
                    maxPriority = Math.Max(maxPriority, filter.Priority);
            }
            return maxPriority;
        }


        private List<object[]> MultiplyMatrixRowsByColumn(MatrixColumnAttribute columnAttribute, List<object[]> existingRows)
        {
            if (existingRows == null)
                existingRows = new List<object[]>();
            List<object[]> newRows = new List<object[]>();
            foreach (object o in columnAttribute.Values)
            {
                if (existingRows.Count == 0)
                {
                    newRows.Add(new object[] { o });
                }
                else
                {
                    foreach (object[] oldRow in existingRows)
                    {
                        List<object> newRow = new List<object>(oldRow);
                        newRow.Add(o);
                        newRows.Add(newRow.ToArray());
                    }
                }
            }
            return newRows;
        }
    }
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MatrixColumnAttribute : Attribute
    {
        private int _paramOrderId =-1;
        private string _columnName;
        private object[] _values;

        public string Name
        {
            get { return _columnName; }
            set { _columnName = value; }
        }

        public object[] Values
        {
            get { return _values; }
            set { _values = value; }
        }

        public int ParamOrderId
        {
            get { return _paramOrderId; }
            set { _paramOrderId = value; }
        }
    }
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MatrixSkipConstraintAttribute : Attribute
    {
        private object[] _skipValues = null;
        private string[] _skipColumns = null;

        public object[] SkipValues
        {
            get { return _skipValues; }
            set { _skipValues = value; }
        }

        public string[] SkipColumns
        {
            get { return _skipColumns; }
            set { _skipColumns = value; }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MatrixPriorityFilterAttribute : Attribute
    {
        public object[] FilterValues
        {
            get;
            set;
        }

        public string[] FilterColumns
        {
            get;
            set;
        }

        public int Priority
        {
            get;
            set;
        }
    }
}

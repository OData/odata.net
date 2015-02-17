//---------------------------------------------------------------------
// <copyright file="LinqToAstoriaQuerySpanGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.LinqToAstoria
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Applies span operator to given collections. Creates a copy of collection tree with all objects
    /// reachable through given span paths.
    /// </summary>
    public class LinqToAstoriaQuerySpanGenerator : ILinqToAstoriaQuerySpanGenerator
    {
        /// <summary>
        /// Creates a copy of the provided value which includes all objects reachable through given span paths.
        /// </summary>
        /// <param name="valueToSpan">Value to apply span for.</param>
        /// <param name="expandedPaths">The expanded paths.</param>
        /// <param name="selectedPaths">The selected paths needed to match up with the expanded paths.</param>
        /// <returns>
        /// Copy of the value trimmed to given span paths.
        /// </returns>
        public QueryValue GenerateSpan(QueryValue valueToSpan, IEnumerable<string> expandedPaths, IEnumerable<string> selectedPaths)
        {
            ExceptionUtilities.CheckArgumentNotNull(valueToSpan, "valueToSpan");
            ExceptionUtilities.CheckArgumentNotNull(expandedPaths, "expandedPaths");
            ExceptionUtilities.CheckArgumentNotNull(selectedPaths, "selectedPaths");

            var collection = valueToSpan as QueryCollectionValue;

            if (collection == null)
            {
                return valueToSpan;
            }

            // split each span path into a string array, which we can efficiently analyze later without 
            // having to deal with string parsing all the time
            var identityMap = new Dictionary<QueryStructuralValue, QueryStructuralValue>();

            var result = this.CloneCollectionValue(identityMap, collection);

            foreach (var spanPath in expandedPaths)
            {
                var splitPath = spanPath.Split('.');
                foreach (var item in collection.Elements)
                {
                    this.ExpandRelated(identityMap, item, splitPath, 0, expandedPaths, selectedPaths);
                }
            }

            return result;
        }

        private void ExpandRelated(Dictionary<QueryStructuralValue, QueryStructuralValue> identityMap, QueryValue item, string[] splitPath, int position, IEnumerable<string> expandedPath, IEnumerable<string> selectedPath)
        {
            QueryStructuralValue qsv = item as QueryStructuralValue;
            if (qsv == null)
            {
                return;
            }

            if (position >= splitPath.Length)
            {
                return;
            }

            var memberName = splitPath[position];
            var member = qsv.Type.Properties.SingleOrDefault(c => c.Name == memberName);
            if (member == null)
            {
                return;
            }

            // do not expand if the selected paths don't contain the expand value
            if (selectedPath.Count() > 0 && !selectedPath.Contains(member.Name))
            {
                return;
            }

            var clone = this.CloneStructuralValue(identityMap, qsv);

            if (member.PropertyType is QueryCollectionType)
            {
                var oldValue = qsv.GetCollectionValue(memberName);

                clone.SetValue(memberName, this.CloneCollectionValue(identityMap, oldValue));
                if (!oldValue.IsNull)
                {
                    foreach (var e in oldValue.Elements)
                    {
                        this.ExpandRelated(identityMap, e, splitPath, position + 1, expandedPath, selectedPath);
                    }
                }
            }
            else if (member.PropertyType is QueryStructuralType)
            {
                var oldValue = qsv.GetStructuralValue(memberName);
                var newValue = this.CloneStructuralValue(identityMap, oldValue);
                clone.SetValue(memberName, newValue);
                this.ExpandRelated(identityMap, oldValue, splitPath, position + 1, expandedPath, selectedPath);
            }
        }

        /// <summary>
        /// Clones the collection (returns a new collection where all elements are clones of existing elements in the source collection)
        /// </summary>
        /// <param name="identityMap">The identity map.</param>
        /// <param name="collectionValue">The collection value.</param>
        /// <returns>New collection where all elements are clones of existing elements in the source collection.</returns>
        private QueryCollectionValue CloneCollectionValue(Dictionary<QueryStructuralValue, QueryStructuralValue> identityMap, QueryCollectionValue collectionValue)
        {
            if (collectionValue.IsNull)
            {
                return collectionValue.Type.NullValue;
            }

            var clonedValues = collectionValue.Elements.Select(c => this.CloneValue(identityMap, c)).ToList();
            return collectionValue.Type.CreateCollectionWithValues(clonedValues);
        }

        private QueryValue CloneValue(Dictionary<QueryStructuralValue, QueryStructuralValue> identityMap, QueryValue value)
        {
            // no need to clone scalar values
            if (value is QueryScalarValue)
            {
                return value;
            }

            if (value is AstoriaQueryStreamValue)
            {
                return value;
            }

            QueryStructuralValue qsv = value as QueryStructuralValue;
            if (qsv != null)
            {
                return this.CloneStructuralValue(identityMap, qsv);
            }

            QueryCollectionValue qcv = value as QueryCollectionValue;
            ExceptionUtilities.Assert(qcv != null, "Expected collection value.");

            return this.CloneCollectionValue(identityMap, qcv);
        }

        private QueryStructuralValue CloneStructuralValue(Dictionary<QueryStructuralValue, QueryStructuralValue> identityMap, QueryStructuralValue qsv)
        {
            if (qsv.IsNull)
            {
                return qsv;
            }

            QueryStructuralValue clonedValue;

            if (identityMap.TryGetValue(qsv, out clonedValue))
            {
                return clonedValue;
            }

            clonedValue = qsv.Type.CreateNewInstance();
            identityMap.Add(qsv, clonedValue);
      
            foreach (var m in qsv.Type.Properties)
            {
                // copy scalar properties
                if (m.PropertyType is QueryScalarType)
                {
                    clonedValue.SetValue(m.Name, qsv.GetScalarValue(m.Name));
                    continue;
                }

                // copy stream properties
                if (m.PropertyType is AstoriaQueryStreamType)
                {
                    if (m.Name.Contains("DefaultStream"))
                    {
                        clonedValue.SetDefaultStreamValue(qsv.GetDefaultStreamValue());
                    }
                    else
                    {
                        clonedValue.SetStreamValue(m.Name, qsv.GetStreamValue(m.Name));
                    }

                    continue;
                }

                var qst = m.PropertyType as QueryStructuralType;
                if (m.PropertyType is QueryStructuralType)
                {
                    if (!qst.IsValueType)
                    {
                        // skip reference types, clone everything else
                        continue;
                    }

                    clonedValue.SetValue(m.Name, this.CloneStructuralValue(identityMap, qsv.GetStructuralValue(m.Name)));
                }

                var qct = m.PropertyType as QueryCollectionType;
                if (qct != null)
                {
                    var elementStructuralType = qct.ElementType as QueryStructuralType;
                    if (elementStructuralType != null)
                    {
                        if (!elementStructuralType.IsValueType)
                        {
                            // skip collections of reference types, clone everything else
                            continue;
                        }
                    }

                    clonedValue.SetValue(m.Name, this.CloneCollectionValue(identityMap, qsv.GetCollectionValue(m.Name)));
                }
            }

            return clonedValue;
        }
    }
}
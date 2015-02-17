//---------------------------------------------------------------------
// <copyright file="NamedValueToQueryValueUpdater.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Internal helpers for setting values on QueryValues from NamedValues
    /// </summary>
    [ImplementationName(typeof(INamedValueToQueryValueUpdater), "Default")]
    public class NamedValueToQueryValueUpdater : INamedValueToQueryValueUpdater
    {
        private List<string> unusedNamedValuePaths = new List<string>();

        /// <summary>
        /// Sets the given values on the given structural value instance
        /// </summary>
        /// <param name="queryValue">The instance to set the values on</param>
        /// <param name="namedValues">The values to set</param>
        public void UpdateValues(QueryValue queryValue, IEnumerable<NamedValue> namedValues)
        {
            ExceptionUtilities.CheckArgumentNotNull(queryValue, "queryValue");
            ExceptionUtilities.CheckArgumentNotNull(namedValues, "namedValues");

            this.unusedNamedValuePaths = new List<string>(namedValues.Select(s => s.Name).ToArray());

            var queryStructuralValue = queryValue as QueryStructuralValue;
            if (queryStructuralValue != null)
            {
                this.UpdateValues(queryStructuralValue, namedValues, null);
            }
            else
            {
                var queryCollectionValue = queryValue as QueryCollectionValue;
                ExceptionUtilities.CheckObjectNotNull(queryCollectionValue, "Can only update QueryValues of Collection or Structural type not '{0}'", queryValue.Type);
                this.UpdateValues(queryCollectionValue, namedValues);
            }

            var errorStringBuilder = new StringBuilder();
            foreach (NamedValue unusedNamedValue in namedValues.Where(nv => this.unusedNamedValuePaths.Contains(nv.Name)))
            {
                errorStringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0}={1}", unusedNamedValue.Name, Convert.ToString(unusedNamedValue.Value, CultureInfo.InvariantCulture)));
            }

            if (errorStringBuilder.Length > 0)
            {
                throw new TaupoInfrastructureException("All properties have not been set:\r\n" + errorStringBuilder.ToString());
            }
        }
        
        internal void UpdateValues(QueryCollectionValue queryCollectionValue, IEnumerable<NamedValue> namedValues)
        {
            ExceptionUtilities.CheckArgumentNotNull(queryCollectionValue, "queryCollectionValue");
            ExceptionUtilities.CheckArgumentNotNull(namedValues, "namedValues");

            var primitiveElementDataType = queryCollectionValue.Type.ElementType as QueryClrPrimitiveType;
            if (primitiveElementDataType != null)
            {
                this.UpdateRootScalarBag(queryCollectionValue, namedValues, primitiveElementDataType);
            }
            else
            {
                var complexElementDataType = queryCollectionValue.Type.ElementType as QueryComplexType;
                ExceptionUtilities.CheckObjectNotNull(complexElementDataType, "Expected QueryComplex Type, actual query type {0}", queryCollectionValue.Type);

                this.UpdateRootComplexBag(queryCollectionValue, namedValues, complexElementDataType);
            }
        }

        internal void UpdateValues(QueryStructuralValue instance, IEnumerable<NamedValue> namedValues, string propertyPath)
        {
            foreach (QueryProperty property in instance.Type.Properties)
            {
                string childPropertyPath = property.Name;
                if (propertyPath != null)
                {
                    childPropertyPath = propertyPath + "." + property.Name;
                }

                // Skip if its an EntityType, this only handles structural types
                var queryEntityType = property.PropertyType as QueryEntityType;
                if (queryEntityType != null)
                {
                    continue;
                }

                var collectionType = property.PropertyType as QueryCollectionType;
                var scalarType = property.PropertyType as QueryScalarType;
                var complexDataType = property.PropertyType as QueryComplexType;

                QueryEntityType collectionQueryElementType = null;
                if (collectionType != null)
                {
                    collectionQueryElementType = collectionType.ElementType as QueryEntityType;
                }

                // Skip if its a collection of QueryEntityType
                if (collectionQueryElementType != null)
                {
                    continue;
                }

                if (scalarType != null)
                {
                    NamedValue primitivePropertyNamedValue = namedValues.SingleOrDefault(nv => nv.Name == childPropertyPath);
                    if (primitivePropertyNamedValue != null)
                    {
                        instance.SetPrimitiveValue(property.Name, primitivePropertyNamedValue.Value);
                        this.unusedNamedValuePaths.Remove(childPropertyPath);
                    }
                }
                else if (collectionType != null)
                {
                    List<NamedValue> bagNamedValues = namedValues.Where(nv => nv.Name.StartsWith(childPropertyPath + ".", StringComparison.Ordinal)).ToList();
                    if (bagNamedValues.Any())
                    {
                        this.UpdateBagProperty(instance, property, collectionType.ElementType, childPropertyPath, bagNamedValues);
                    }
                    else
                    {
                        this.UpdateBagPropertyWithNullOrEmpty(instance, property, childPropertyPath, namedValues);
                    }
                }
                else if (complexDataType != null)
                {
                    // NOTE: we cannot assert that it is complex/primitive/bag, because there may be new query types added in other assemblies that we know nothing about here
                    QueryStructuralValue complexTypeValue = instance.GetStructuralValue(property.Name);

                    List<NamedValue> complexInstanceNamedValues = namedValues.Where(nv => nv.Name.StartsWith(childPropertyPath + ".", StringComparison.Ordinal)).ToList();
                    if (complexInstanceNamedValues.Any())
                    {
                        if (complexTypeValue.IsNull)
                        {
                            complexTypeValue = complexDataType.CreateNewInstance();
                        }

                        this.UpdateValues(complexTypeValue, complexInstanceNamedValues, childPropertyPath);
                        instance.SetValue(property.Name, complexTypeValue);
                    }
                    else
                    {
                        // Check for null case
                        List<NamedValue> exactMatches = namedValues.Where(nv => nv.Name == childPropertyPath).ToList();
                        ExceptionUtilities.Assert(exactMatches.Count < 2, "Should only find at most one property path {0} when looking for null value", childPropertyPath);
                        if (exactMatches.Count == 1)
                        {
                            ExceptionUtilities.Assert(
                                exactMatches[0].Value == null, 
                                "Named value at path '{0}' was unexpectedly non-null. Value was '{1}'", 
                                childPropertyPath,
                                exactMatches[0].Value);

                            instance.SetValue(property.Name, complexDataType.NullValue);
                            this.unusedNamedValuePaths.Remove(childPropertyPath);
                        }
                    }
                }
            }
        }

        private void UpdateBagProperty(QueryStructuralValue instance, QueryProperty memberProperty, QueryType elementType, string propertyPath, IEnumerable<NamedValue> namedValues)
        {
            var scalarElementDataType = elementType as QueryScalarType;
            var complexTypeElementDataType = elementType as QueryComplexType;

            if (scalarElementDataType != null)
            {
                this.UpdateScalarBag(instance, memberProperty, propertyPath, namedValues, scalarElementDataType);
            }
            else
            {
                ExceptionUtilities.CheckObjectNotNull(complexTypeElementDataType, "PropertyPath '{0}' is an invalid type '{1}'", propertyPath, memberProperty.PropertyType);

                this.UpdateComplexBag(instance, memberProperty, propertyPath, namedValues, complexTypeElementDataType);
            }
        }

        private void UpdateComplexBag(QueryStructuralValue instance, QueryProperty memberProperty, string propertyPath, IEnumerable<NamedValue> namedValues, QueryComplexType complexTypeElementDataType)
        {
            int i = 0;

            var complexCollection = new List<QueryValue>();
            List<NamedValue> complexInstanceNamedValues = namedValues.Where(pp => pp.Name.StartsWith(propertyPath + "." + i + ".", StringComparison.Ordinal)).ToList();
            while (complexInstanceNamedValues.Any())
            {
                QueryStructuralValue complexValue = complexTypeElementDataType.CreateNewInstance();
                this.UpdateValues(complexValue, complexInstanceNamedValues, propertyPath + "." + i);
                complexCollection.Add(complexValue);

                i++;
                complexInstanceNamedValues = namedValues.Where(pp => pp.Name.StartsWith(propertyPath + "." + i + ".", StringComparison.Ordinal)).ToList();
            }

            if (complexCollection.Any())
            {
                this.SetCollectionProperty(instance, memberProperty, complexCollection);
            }
        }

        private void UpdateRootComplexBag(QueryCollectionValue queryCollectionValue, IEnumerable<NamedValue> namedValues, QueryComplexType complexTypeElementDataType)
        {
            int i = 0;

            var complexCollection = new List<QueryValue>();
            List<NamedValue> complexInstanceNamedValues = namedValues.Where(pp => pp.Name.StartsWith(i + ".", StringComparison.Ordinal)).ToList();
            while (complexInstanceNamedValues.Any())
            {
                QueryStructuralValue complexValue = complexTypeElementDataType.CreateNewInstance();
                this.UpdateValues(complexValue, complexInstanceNamedValues, i.ToString(CultureInfo.InvariantCulture));
                complexCollection.Add(complexValue);

                i++;
                complexInstanceNamedValues = namedValues.Where(pp => pp.Name.StartsWith(i + ".", StringComparison.Ordinal)).ToList();
            }

            if (complexCollection.Any())
            {
                this.SetCollectionValue(queryCollectionValue, complexCollection);
            }
        }

        private void SetCollectionValue(QueryCollectionValue queryCollectionValue, List<QueryValue> collectionElements)
        {
            queryCollectionValue.Elements.Clear();
            foreach (QueryValue queryValue in collectionElements)
            {
                queryCollectionValue.Elements.Add(queryValue);
            }
        }

        private void SetCollectionProperty(QueryStructuralValue instance, QueryProperty memberProperty, List<QueryValue> collectionElements)
        {
            QueryCollectionValue queryCollectionValue = instance.GetCollectionValue(memberProperty.Name);

            queryCollectionValue.Elements.Clear();
            foreach (QueryValue queryValue in collectionElements)
            {
                queryCollectionValue.Elements.Add(queryValue);
            }

            instance.SetValue(memberProperty.Name, queryCollectionValue);
        }

        private void UpdateScalarBag(QueryStructuralValue instance, QueryProperty memberProperty, string propertyPath, IEnumerable<NamedValue> namedValues, QueryScalarType scalarElementDataType)
        {
            int i = 0;

            var scalarCollection = new List<QueryValue>();
            List<NamedValue> scalarItemNamedValues = namedValues.Where(pp => pp.Name == propertyPath + "." + i).ToList();
            while (scalarItemNamedValues.Any())
            {
                ExceptionUtilities.Assert(scalarItemNamedValues.Count() < 2, "Should not get more than one value for a scalar Bag item for path '{0}'", propertyPath + "." + i);
                var value = scalarItemNamedValues.Single();
                scalarCollection.Add(scalarElementDataType.CreateValue(value.Value));
                this.unusedNamedValuePaths.Remove(value.Name);

                i++;
                scalarItemNamedValues = namedValues.Where(pp => pp.Name == propertyPath + "." + i).ToList();
            }

            if (scalarCollection.Any())
            {
                this.SetCollectionProperty(instance, memberProperty, scalarCollection);
            }
        }

        private void UpdateRootScalarBag(QueryCollectionValue instance, IEnumerable<NamedValue> namedValues, QueryScalarType scalarElementDataType)
        {
            int i = 0;

            var scalarCollection = new List<QueryValue>();
            List<NamedValue> scalarItemNamedValues = namedValues.Where(pp => pp.Name == i.ToString(CultureInfo.InvariantCulture)).ToList();
            while (scalarItemNamedValues.Any())
            {
                ExceptionUtilities.Assert(scalarItemNamedValues.Count() < 2, "Should not get more than one value for a scalar Bag item for path '{0}'", i.ToString(CultureInfo.InvariantCulture));
                var value = scalarItemNamedValues.Single();
                scalarCollection.Add(scalarElementDataType.CreateValue(value.Value));
                this.unusedNamedValuePaths.Remove(value.Name);

                i++;
                scalarItemNamedValues = namedValues.Where(pp => pp.Name == i.ToString(CultureInfo.InvariantCulture)).ToList();
            }

            if (scalarCollection.Any())
            {
                this.SetCollectionValue(instance, scalarCollection);
            }
        }

        private void UpdateBagPropertyWithNullOrEmpty(QueryStructuralValue instance, QueryProperty memberProperty, string propertyPath, IEnumerable<NamedValue> namedValues)
        {
            List<NamedValue> exactMatches = namedValues.Where(nv => nv.Name == propertyPath).ToList();
            if (!exactMatches.Any())
            {
                return;
            }

            ExceptionUtilities.Assert(exactMatches.Count == 1, "Should only find at most one property path {0} when looking for null or empty value", propertyPath);

            NamedValue expectedBagValue = exactMatches.Single();
            var bagType = memberProperty.PropertyType as QueryCollectionType;

            if (expectedBagValue.Value == null)
            {
                instance.SetValue(memberProperty.Name, bagType.NullValue);
                this.unusedNamedValuePaths.Remove(expectedBagValue.Name);
            }
            else if (expectedBagValue.Value == EmptyData.Value)
            {
                QueryCollectionValue value = bagType.CreateCollectionWithValues(new QueryValue[] { });
                instance.SetValue(memberProperty.Name, value);
                this.unusedNamedValuePaths.Remove(expectedBagValue.Name);
            }
        }
    }
}

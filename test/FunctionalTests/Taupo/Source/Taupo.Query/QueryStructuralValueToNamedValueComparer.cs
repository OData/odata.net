//---------------------------------------------------------------------
// <copyright file="QueryStructuralValueToNamedValueComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Internal helpers for comparing NamedValues to QueryStructuralValues
    /// </summary>
    [ImplementationName(typeof(IQueryStructuralValueToNamedValueComparer), "Default")]
    public class QueryStructuralValueToNamedValueComparer : IQueryStructuralValueToNamedValueComparer
    {
        private List<string> errors;
        private List<string> unusedNamedValuePaths = new List<string>();
        private IQueryScalarValueToClrValueComparer comparer;

        /// <summary>
        /// Gets or sets the assertion handler to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public AssertionHandler Assert { get; set; }

        /// <summary>
        /// Compares a queryStructualValue to namedValues
        /// Throws a DataComparisonException if values don't match
        /// </summary>
        /// <param name="queryStructuralValue">QueryStructural Value to compare</param>
        /// <param name="namedValues">NamedValues to compare</param>
        /// <param name="scalarComparer">The comparer to use for scalar values</param>
        public void Compare(QueryStructuralValue queryStructuralValue, IEnumerable<NamedValue> namedValues, IQueryScalarValueToClrValueComparer scalarComparer)
        {
            ExceptionUtilities.CheckArgumentNotNull(queryStructuralValue, "queryStructuralValue");
            ExceptionUtilities.CheckArgumentNotNull(namedValues, "namedValues");
            ExceptionUtilities.CheckArgumentNotNull(scalarComparer, "scalarComparer");

            this.errors = new List<string>();
            this.unusedNamedValuePaths = new List<string>(namedValues.Select(s => s.Name).ToArray());
            this.comparer = scalarComparer;

            this.CompareValues(queryStructuralValue, namedValues, null);
            
            foreach (string propertyPath in this.unusedNamedValuePaths)
            {
                this.errors.Add(string.Format(CultureInfo.InvariantCulture, "Value for propertyPath {0} was not compared against queryStructuralValue", propertyPath));
            }

            string errorMessage = null;
            foreach (string error in this.errors)
            {
                if (errorMessage == null)
                {
                    errorMessage = error;
                }
                else
                {
                    errorMessage = errorMessage + ", \r\n" + error;
                }
            }

            if (errorMessage != null)
            {
                throw new DataComparisonException(errorMessage);
            }
        }

        private bool WriteErrorIfNotNull(string propertyPath, QueryValue queryValue)
        {
            if (!queryValue.IsNull)
            {
                this.errors.Add(string.Format(CultureInfo.InvariantCulture, "Error: expected null value for propertyPath '{0}' instead recieved '{1}'", propertyPath, queryValue.ToString()));
                return true;
            }

            return false;
        }

        private bool WriteErrorIfNull(string propertyPath, QueryValue queryValue)
        {
            if (queryValue.IsNull)
            {
                this.errors.Add(string.Format(CultureInfo.InvariantCulture, "Error: expected a value for propertyPath '{0}' instead recieved null", propertyPath));
                return true;
            }

            return false;
        }

        private bool WriteErrorIfNotEqual(string propertyPath, object expected, QueryScalarValue value)
        {
            try
            {
                this.comparer.Compare(expected, value, this.Assert);
            }
            catch (TestFailedException e)
            {
                this.errors.Add(string.Format(CultureInfo.InvariantCulture, "Error: value at property '{0}' did not match. {1}", propertyPath, e.Message));
                return false;
            }

            return true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyPath", Justification = "The parameter is used")]
        private bool WriteErrorIfNotEqual(string propertyPath, object expected, object actual, string errorMessage, params object[] errorParams)
        {
            ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
            ExceptionUtilities.CheckArgumentNotNull(actual, "actual");
            bool equal = ValueComparer.Instance.Equals(expected, actual);

            if (!equal)
            {
                this.errors.Add(string.Format(CultureInfo.InvariantCulture, errorMessage, errorParams));
            }

            return !equal;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Need to streams towards end of if-else constructs")]
        private void CompareValues(QueryStructuralValue instance, IEnumerable<NamedValue> namedValues, string propertyPath)
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
                    // The following code block handles the case where the value is server generated.
                    // For instance, server generated Keys would fail so if a property is marked as being server generated
                    // we make sure to remove that property from the list of properties to verify
                    MemberProperty memberProperty;
                    var instanceEntityType = instance.Type as QueryEntityType;
                    if (instanceEntityType != null)
                    {
                        memberProperty = instanceEntityType.EntityType.AllProperties.SingleOrDefault(p => p.Name == property.Name);
                    }
                    else
                    {
                        memberProperty = ((QueryComplexType)instance.Type).ComplexType.Properties.SingleOrDefault(p => p.Name == property.Name);
                    }

                    if (memberProperty != null && memberProperty.Annotations.OfType<StoreGeneratedPatternAnnotation>().Any())
                    {
                        // TODO: be more fine-grained about whether this is an update or insert (ie, look at the annotation's property values)
                        this.unusedNamedValuePaths.Remove(childPropertyPath);
                        continue;
                    }

                    NamedValue primitivePropertyNamedValue = namedValues.SingleOrDefault(nv => nv.Name == childPropertyPath);
                    if (primitivePropertyNamedValue != null)
                    {
                        var queryValue = instance.GetScalarValue(property.Name);
                        this.WriteErrorIfNotEqual(childPropertyPath, primitivePropertyNamedValue.Value, queryValue);
                        this.unusedNamedValuePaths.Remove(childPropertyPath);
                    }
                }
                else if (collectionType != null)
                {
                    List<NamedValue> bagNamedValues = namedValues.Where(nv => nv.Name.StartsWith(childPropertyPath + ".", StringComparison.Ordinal)).ToList();
                    if (bagNamedValues.Any())
                    {
                        this.CompareBagProperty(instance, property, collectionType.ElementType, childPropertyPath, bagNamedValues);
                    }
                    else
                    {
                        this.CompareBagPropertyWithNullOrEmpty(instance, property, childPropertyPath, namedValues);
                    }
                }
                else if (complexDataType != null)
                {
                    // NOTE: we cannot assert that it is complex/primitive/bag, because there may be new query types added in other assemblies that we know nothing about here
                    QueryStructuralValue complexTypeValue = instance.GetStructuralValue(property.Name);

                    List<NamedValue> complexInstanceNamedValues = namedValues.Where(nv => nv.Name.StartsWith(childPropertyPath + ".", StringComparison.Ordinal)).ToList();
                    if (complexInstanceNamedValues.Any())
                    {
                        if (!this.WriteErrorIfNull(childPropertyPath, complexTypeValue))
                        {
                            this.CompareValues(complexTypeValue, complexInstanceNamedValues, childPropertyPath);
                        }
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

                            QueryValue queryValue = instance.GetValue(property.Name);
                            this.WriteErrorIfNotNull(childPropertyPath, queryValue);
                            this.unusedNamedValuePaths.Remove(childPropertyPath);
                        }
                    }
                }
            }
        }

        private void CompareBagProperty(QueryStructuralValue instance, QueryProperty memberProperty, QueryType elementType, string propertyPath, IEnumerable<NamedValue> namedValues)
        {
            var scalarElementDataType = elementType as QueryScalarType;
            var complexTypeElementDataType = elementType as QueryComplexType;
     
            if (scalarElementDataType != null)
            {
                this.ComparePrimitiveBag(instance, memberProperty, propertyPath, namedValues);
            }
            else
            {
                ExceptionUtilities.CheckObjectNotNull(complexTypeElementDataType, "PropertyPath '{0}' is an invalid type '{1}'", propertyPath, memberProperty.PropertyType);

                this.CompareComplexBag(instance, memberProperty, propertyPath, namedValues);
            }
        }

        private void CompareComplexBag(QueryStructuralValue instance, QueryProperty memberProperty, string propertyPath, IEnumerable<NamedValue> namedValues)
        {
            int i = 0;

            var collection = instance.GetCollectionValue(memberProperty.Name);

            if (!this.WriteErrorIfNull(propertyPath, collection))
            {
                List<NamedValue> complexInstanceNamedValues = namedValues.Where(pp => pp.Name.StartsWith(propertyPath + "." + i + ".", StringComparison.Ordinal)).ToList();
                while (complexInstanceNamedValues.Any())
                {
                    if (i < collection.Elements.Count)
                    {
                        var complexValue = collection.Elements[i] as QueryStructuralValue;
                        this.CompareValues(complexValue, complexInstanceNamedValues, propertyPath + "." + i);
                    }

                    i++;
                    complexInstanceNamedValues = namedValues.Where(pp => pp.Name.StartsWith(propertyPath + "." + i + ".", StringComparison.Ordinal)).ToList();
                }

                this.WriteErrorIfNotEqual(propertyPath, collection.Elements.Count, i, "The number of expected items '{0}' does not match the actual '{1}' for propertyPath {2}", collection.Elements.Count, i, propertyPath);
            }
        }

        private void ComparePrimitiveBag(QueryStructuralValue instance, QueryProperty memberProperty, string propertyPath, IEnumerable<NamedValue> namedValues)
        {
            int i = 0;

            var collection = instance.GetCollectionValue(memberProperty.Name);

            if (!this.WriteErrorIfNull(propertyPath, collection))
            {
                List<NamedValue> primitiveItemNamedValues = namedValues.Where(pp => pp.Name == propertyPath + "." + i).ToList();
                while (primitiveItemNamedValues.Any())
                {                    
                    if (i < collection.Elements.Count)
                    {
                        var scalarQueryValue = collection.Elements[i] as QueryScalarValue;
                        ExceptionUtilities.Assert(primitiveItemNamedValues.Count() < 2, "Should not get more than one value for a primitive Bag item for path '{0}'", propertyPath + "." + i);
                        var value = primitiveItemNamedValues.Single();
                        this.WriteErrorIfNotEqual(propertyPath, value.Value, scalarQueryValue);
                        this.unusedNamedValuePaths.Remove(value.Name);
                    }

                    i++;
                    primitiveItemNamedValues = namedValues.Where(pp => pp.Name == propertyPath + "." + i).ToList();
                }

                this.WriteErrorIfNotEqual(propertyPath, collection.Elements.Count, i, "The number of expected items '{0}' does not match the actual '{1}' for propertyPath {2}", collection.Elements.Count, i, propertyPath);
            }
        }

        private void CompareBagPropertyWithNullOrEmpty(QueryStructuralValue instance, QueryProperty memberProperty, string propertyPath, IEnumerable<NamedValue> namedValues)
        {
            List<NamedValue> exactMatches = namedValues.Where(nv => nv.Name == propertyPath).ToList();

            if (!exactMatches.Any())
            {
                return;
            }

            ExceptionUtilities.Assert(exactMatches.Count == 1, "Should only find at most one property path {0} when looking for null or empty value", propertyPath);
            
            QueryCollectionValue actualQueryBagValue = instance.GetCollectionValue(memberProperty.Name);
            NamedValue expectedBagValue = exactMatches.Single();

            if (expectedBagValue.Value == null)
            {
                this.WriteErrorIfNotNull(propertyPath, actualQueryBagValue);
                this.unusedNamedValuePaths.Remove(propertyPath);
            }
            else if (expectedBagValue.Value == EmptyData.Value)
            {
                if (!this.WriteErrorIfNull(propertyPath, actualQueryBagValue))
                {
                    this.WriteErrorIfNotEqual(propertyPath, actualQueryBagValue.Elements.Count, 0, "Expected zero elements in BagProperty");
                    this.unusedNamedValuePaths.Remove(propertyPath);
                }
            }
        }
    }
}

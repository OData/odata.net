//---------------------------------------------------------------------
// <copyright file="QueryExpressionToTypedValueExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// Extension methods that make writing astoria tests easier, pieces that connect various 
    /// Expressions and Odata 
    /// </summary>
    public static class QueryExpressionToTypedValueExtensionMethods
    {
        internal static PrimitiveValue ConvertToPrimitiveValue(this QueryConstantExpression queryConstantExpression, PrimitiveDataType primitiveDataType)
        {
            ExceptionUtilities.CheckArgumentNotNull(queryConstantExpression, "queryConstantExpression");
            ExceptionUtilities.CheckArgumentNotNull(primitiveDataType, "primitiveDataType");

            // TODO: Need to understand what to do when values are null, should we omit them or be able to turn them on or off?
            string typeName = primitiveDataType.GetEdmTypeName();
            return new PrimitiveValue(typeName, queryConstantExpression.ScalarValue.Value);
        }

        internal static IList<KeyValuePair<string, ITypedValue>> ConvertActionArgumentsToTypedValues(this Function function, ServiceOperationAnnotation actionAnnotation, IList<KeyValuePair<string, QueryExpression>> parameters)
        {
            var parameterValues = new List<KeyValuePair<string, ITypedValue>>();

            FunctionParameter boundParameter = null;
            if (actionAnnotation.BindingKind.IsBound())
            {
                boundParameter = function.Parameters[0];
            }

            foreach (var parameterPair in parameters)
            {
                var edmFunctionParameterDefinition = function.Parameters.Where(p => p.Name == parameterPair.Key).SingleOrDefault();
                ExceptionUtilities.CheckObjectNotNull(edmFunctionParameterDefinition, "Cannot find parameter '{0}' defined in function '{1}'", parameterPair.Key, function.Name);
                ExceptionUtilities.Assert(edmFunctionParameterDefinition != boundParameter, "Bound parameters MUST not be passed in as they will have already been built for the ODataUri was built");

                var primitiveDataType = edmFunctionParameterDefinition.DataType as PrimitiveDataType;
                var collectionDataType = edmFunctionParameterDefinition.DataType as CollectionDataType;

                if (primitiveDataType != null)
                {
                    ExceptionUtilities.CheckObjectNotNull(primitiveDataType, "Expected a primitive data type not '{0}'", primitiveDataType);
                    var primitiveValue = ConvertToPrimitiveValue(parameterPair.Value, primitiveDataType);
                    parameterValues.Add(new KeyValuePair<string, ITypedValue>(edmFunctionParameterDefinition.Name, primitiveValue));
                }
                else if (collectionDataType != null)
                {
                    ExceptionUtilities.CheckObjectNotNull(collectionDataType, "expected a collection data type, actual '{0}", collectionDataType);
                    var collectionValue = parameterPair.Value.ConvertToMultiValue(collectionDataType.ElementDataType);
                    parameterValues.Add(new KeyValuePair<string, ITypedValue>(edmFunctionParameterDefinition.Name, collectionValue));
                }
                else
                {
                    var complexArgumentValue = parameterPair.Value;
                    var complexInstance = complexArgumentValue.ConvertToComplexInstance();
                    parameterValues.Add(new KeyValuePair<string, ITypedValue>(edmFunctionParameterDefinition.Name, complexInstance));
                }
            }

            return parameterValues;
        }

        internal static PrimitiveValue ConvertToPrimitiveValue(this QueryExpression expression, PrimitiveDataType primitiveDataType)
        {
            var constantExpression = expression as QueryConstantExpression;
            if (constantExpression != null)
            {
                return new PrimitiveValue(primitiveDataType.GetEdmTypeName(), constantExpression.ScalarValue.Value);
            }
            else
            {
                var nullExpression = expression as QueryNullExpression;
                ExceptionUtilities.CheckObjectNotNull(nullExpression, "Expected a QueryNullReference");

                return new PrimitiveValue(primitiveDataType.GetEdmTypeName(), null);
            }
        }

        internal static ITypedValue ConvertToMultiValue(this QueryExpression expression, DataType elementDataType)
        {
            var primitiveDataType = elementDataType as PrimitiveDataType;
            var complexDataType = elementDataType as ComplexDataType;

            var nullExpression = expression as QueryNullExpression;

            if (nullExpression != null)
            {
                if (primitiveDataType != null)
                {
                    return new PrimitiveMultiValue(primitiveDataType.BuildMultiValueTypeName(), true);
                }
                else
                {
                    ExceptionUtilities.CheckObjectNotNull(complexDataType, "Expected a complexDataType");
                    return new ComplexMultiValue(complexDataType.BuildMultiValueTypeName(), true);
                }
            }

            List<ODataPayloadElement> elements = new List<ODataPayloadElement>();
            
            var newArrayExpression = expression as LinqNewArrayExpression;

            ExceptionUtilities.CheckObjectNotNull(newArrayExpression, "Expected a LinqNewArrayExpression");

            foreach (var childExpression in newArrayExpression.Expressions)
            {
                var complexInstanceExpression = childExpression as LinqNewInstanceExpression;

                if (complexInstanceExpression != null)
                {
                    elements.Add(complexInstanceExpression.ConvertToComplexInstance());
                }
                else
                {
                    ExceptionUtilities.CheckObjectNotNull(primitiveDataType, "primitiveDataType");
                    var constantExpression = childExpression as QueryConstantExpression;
                    
                    ExceptionUtilities.CheckObjectNotNull(constantExpression, "Unexpected Expression ArgumentValue '{0}'", constantExpression.ToString());
                    elements.Add(constantExpression.ConvertToPrimitiveValue(primitiveDataType));
                }
            }

            if (primitiveDataType != null)
            {
                var primitiveValues = elements.OfType<PrimitiveValue>().ToArray();
                ExceptionUtilities.Assert(primitiveValues.Length == elements.Count, "Expected elements to only be of complexInstances");
                return new PrimitiveMultiValue(primitiveDataType.BuildMultiValueTypeName(), false, primitiveValues);
            }
            else
            {
                ExceptionUtilities.CheckObjectNotNull(complexDataType, "Expected collection to be of a scalar or complexType, unknowntype, {0}", elementDataType);
                var complexInstances = elements.OfType<ComplexInstance>().ToArray();
                ExceptionUtilities.Assert(complexInstances.Length == elements.Count, "Expected elements to only be of complexInstances");
                return new ComplexMultiValue(complexDataType.BuildMultiValueTypeName(), false, complexInstances);
            }
        }

        /// <summary>
        /// Converts a LinqNewExpression to a Complex Instance
        /// </summary>
        /// <param name="expression">Expression to convert to a ComplexInstance</param>
        /// <returns>Complex Instance</returns>
        internal static ComplexInstance ConvertToComplexInstance(this QueryExpression expression)
        {
            var nullConstantExpression = expression as QueryNullExpression;

            var queryComplexType = (QueryComplexType)expression.ExpressionType;
            if (nullConstantExpression != null)
            {
                return new ComplexInstance(queryComplexType.ComplexType.FullName, true);
            }

            var structuralExpression = expression as LinqNewInstanceExpression;
            var newComplexInstance = new ComplexInstance(queryComplexType.ComplexType.FullName, false);
            ExceptionUtilities.Assert(structuralExpression.MemberNames.Count == structuralExpression.Members.Count, "MemberNames and Members count are not equal");

            for (int i = 0; i < structuralExpression.MemberNames.Count; i++)
            {
                string memberName = structuralExpression.MemberNames[i];
                var memberExpression = structuralExpression.Members[i];
                var memberProperty = queryComplexType.ComplexType.Properties.Single(p => p.Name == memberName);

                var complexDataType = memberProperty.PropertyType as ComplexDataType;
                var collectionDataType = memberProperty.PropertyType as CollectionDataType;

                if (complexDataType != null)
                {
                    var childComplexInstance = memberExpression.ConvertToComplexInstance();
                    var complexProperty = new ComplexProperty() { Name = memberName, Value = childComplexInstance };
                    newComplexInstance.Add(complexProperty);
                }
                else if (collectionDataType != null)
                {
                    var collectionPropertyType = memberProperty.PropertyType as CollectionDataType;
                    var convertedValue = memberExpression.ConvertToMultiValue(collectionPropertyType.ElementDataType);
                    if (collectionPropertyType.ElementDataType is ComplexDataType)
                    {
                        newComplexInstance.Add(new ComplexMultiValueProperty(memberName, convertedValue as ComplexMultiValue));
                    }
                    else
                    {
                        var primitiveDataType = collectionPropertyType.ElementDataType as PrimitiveDataType;
                        ExceptionUtilities.CheckObjectNotNull(primitiveDataType, "Not a primitiveDataType '{0}'", collectionPropertyType.ElementDataType);
                        newComplexInstance.Add(new PrimitiveMultiValueProperty(memberName, convertedValue as PrimitiveMultiValue));
                    }
                }
                else
                {
                    var primitiveDataType = memberProperty.PropertyType as PrimitiveDataType;
                    ExceptionUtilities.CheckObjectNotNull(primitiveDataType, "Expected a PrimitiveDataType");
                    var primitiveValue = memberExpression.ConvertToPrimitiveValue(primitiveDataType);
                    newComplexInstance.Add(new PrimitiveProperty(memberName, primitiveDataType.GetEdmTypeName(), primitiveValue.ClrValue));
                }
            }

            return newComplexInstance;
        }

        internal static IList<KeyValuePair<string, QueryExpression>> GetNonBoundFunctionArgments(this QueryCustomFunctionCallExpression queryCustomFunctionCallExpression)
        {
            List<KeyValuePair<string, QueryExpression>> nonBoundParameterArguments = new List<KeyValuePair<string, QueryExpression>>();
            var actionAnnotation = queryCustomFunctionCallExpression.Function.Annotations.OfType<ServiceOperationAnnotation>().SingleOrDefault();
            ExceptionUtilities.CheckObjectNotNull(actionAnnotation, "Expected Action Annotation to exist");

            string bindingParameterName = null;
            if (actionAnnotation.BindingKind != OperationParameterBindingKind.Never)
            {
                bindingParameterName = queryCustomFunctionCallExpression.Function.Parameters[0].Name;
            }

            for (int i = 0; i < queryCustomFunctionCallExpression.Arguments.Count; i++)
            {
                var parameter = queryCustomFunctionCallExpression.Function.Parameters[i];
                ExceptionUtilities.CheckObjectNotNull(parameter, "Expected parameter to exist");

                if (parameter.Name != bindingParameterName)
                {
                    nonBoundParameterArguments.Add(new KeyValuePair<string, QueryExpression>(parameter.Name, queryCustomFunctionCallExpression.Arguments.ElementAt(i)));
                }
            }

            return nonBoundParameterArguments;
        }
    }
}
//---------------------------------------------------------------------
// <copyright file="QueryToODataPayloadConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.LinqToAstoria
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;

    /// <summary>
    /// Visitor to build an OData payload from a query when a procedure is called
    /// </summary>
    [ImplementationName(typeof(IQueryToODataPayloadConverter), "Default")]
    public class QueryToODataPayloadConverter : IQueryToODataPayloadConverter
    {
        /// <summary>
        /// Converts a queryExpression to a payload where applicable, returns null if not required
        /// </summary>
        /// <param name="expression">Expression to convert</param>
        /// <returns>OData Payload</returns>
        public ODataPayloadElement ComputePayload(QueryExpression expression)
        {
            LinqToAstoriaProcedureExpressionLocatorVisitor visitor = new LinqToAstoriaProcedureExpressionLocatorVisitor();
            var procedureExpression = visitor.FindProcedure(expression);

            if (procedureExpression == null)
            {
                return null;
            }

            var actionAnnotation = procedureExpression.Function.Annotations.OfType<ServiceOperationAnnotation>().SingleOrDefault();
            
            ExceptionUtilities.CheckObjectNotNull(actionAnnotation, "Should have an action annotation");
            ExceptionUtilities.Assert(procedureExpression.Function.IsAction(), "Visitor did not find a procedure");

            var nonboundArguments = procedureExpression.GetNonBoundFunctionArgments();

            var parameterKeyValues = procedureExpression.Function.ConvertActionArgumentsToTypedValues(actionAnnotation, nonboundArguments);

            var complexInstance = new ComplexInstance(null, false);
            foreach (var parameterKeyValuePair in parameterKeyValues)
            {
                var parameterPrimitiveValue = parameterKeyValuePair.Value as PrimitiveValue;
                var primitiveMultiValue = parameterKeyValuePair.Value as PrimitiveMultiValue;
                var complexMultiValue = parameterKeyValuePair.Value as ComplexMultiValue; 
                if (parameterPrimitiveValue != null)
                {
                    complexInstance.Add(new PrimitiveProperty(parameterKeyValuePair.Key, parameterPrimitiveValue.FullTypeName, parameterPrimitiveValue.ClrValue));
                }
                else if (primitiveMultiValue != null)
                {
                    complexInstance.Add(new PrimitiveMultiValueProperty(parameterKeyValuePair.Key, primitiveMultiValue));
                }
                else if (complexMultiValue != null)
                {
                    complexInstance.Add(new ComplexMultiValueProperty(parameterKeyValuePair.Key, complexMultiValue));
                }
                else
                {
                    var parameterComplexInstance = parameterKeyValuePair.Value as ComplexInstance;
                    ExceptionUtilities.CheckObjectNotNull(parameterComplexInstance, "Unsupported type");
                    complexInstance.Add(new ComplexProperty(parameterKeyValuePair.Key, parameterComplexInstance));
                }
            }

            return complexInstance;
        }

        private class LinqToAstoriaProcedureExpressionLocatorVisitor : LinqToAstoriaExpressionReplacingVisitor
        {
            private QueryCustomFunctionCallExpression procedureFunctionCall;

            public override QueryExpression Visit(QueryCustomFunctionCallExpression expression)
            {
                if (expression.Function.IsAction())
                {
                    this.procedureFunctionCall = expression;
                }

                return base.Visit(expression);
            }

            public QueryCustomFunctionCallExpression FindProcedure(QueryExpression expression)
            {
                expression.Accept(this);

                return this.procedureFunctionCall;
            }
        }
    }
}

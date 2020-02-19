//---------------------------------------------------------------------
// <copyright file="EdmToClrEvaluator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Expression evaluator capable of producing CLR values.
    /// </summary>
    public class EdmToClrEvaluator : EdmExpressionEvaluator
    {
        private EdmToClrConverter edmToClrConverter = new EdmToClrConverter();

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmToClrEvaluator"/> class.
        /// </summary>
        /// <param name="builtInFunctions">Builtin functions dictionary to the evaluators for the functions.</param>
        public EdmToClrEvaluator(IDictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>> builtInFunctions)
            : base(builtInFunctions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmToClrEvaluator"/> class.
        /// </summary>
        /// <param name="builtInFunctions">Builtin functions dictionary to the evaluators for the functions.</param>
        /// <param name="lastChanceOperationApplier">Function to call to evaluate an application of a function with no static binding.</param>
        public EdmToClrEvaluator(IDictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>> builtInFunctions, Func<string, IEdmValue[], IEdmValue> lastChanceOperationApplier)
            : base(builtInFunctions, lastChanceOperationApplier)
        {
        }

#if !ORCAS
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmToClrEvaluator"/> class.
        /// </summary>
        /// <param name="builtInFunctions">Builtin functions dictionary to the evaluators for the functions.</param>
        /// <param name="lastChanceOperationApplier">Function to call to evaluate an application of a function with no static binding.</param>
        /// <param name="getAnnotationExpressionForType">Function to get the <see cref="IEdmExpression"/> of an annotation of an <see cref="IEdmType"/>.</param>
        /// <param name="getAnnotationExpressionForProperty">Function to get the <see cref="IEdmExpression"/> of an annotation of a property or navigation property in <see cref="IEdmType"/>.</param>
        /// <param name="edmModel">The edm model.</param>
        public EdmToClrEvaluator(
            IDictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>> builtInFunctions,
            Func<string, IEdmValue[], IEdmValue> lastChanceOperationApplier,
            Func<IEdmModel, IEdmType, string, string, IEdmExpression> getAnnotationExpressionForType,
            Func<IEdmModel, IEdmType, string, string, string, IEdmExpression> getAnnotationExpressionForProperty,
            IEdmModel edmModel)
            : base(builtInFunctions, lastChanceOperationApplier, getAnnotationExpressionForType, getAnnotationExpressionForProperty, edmModel)
        {
            this.ResolveTypeFromName = this.ResolveEdmTypeFromName;
        }
#endif

        /// <summary>
        /// Gets or sets an instance of <see cref="EdmToClrConverter"/> that is used to produce CLR values during evaluation.
        /// </summary>
        public EdmToClrConverter EdmToClrConverter
        {
            get
            {
                return this.edmToClrConverter;
            }

            set
            {
                EdmUtil.CheckArgumentNull(value, "value");
                this.edmToClrConverter = value;
            }
        }

        /// <summary>
        /// Evaluates an expression with no value context.
        /// </summary>
        /// <typeparam name="T">The CLR type of the value to be returned.</typeparam>
        /// <param name="expression">Expression to evaluate. The expression must not contain paths, because no context for evaluating a path is supplied.</param>
        /// <returns>The value that results from evaluating the expression in the context of the supplied value.</returns>
        public T EvaluateToClrValue<T>(IEdmExpression expression)
        {
            IEdmValue edmValue = this.Evaluate(expression);
            return this.edmToClrConverter.AsClrValue<T>(edmValue);
        }

        /// <summary>
        /// Evaluates an expression in the context of a value.
        /// </summary>
        /// <typeparam name="T">The CLR type of the value to be returned.</typeparam>
        /// <param name="expression">Expression to evaluate.</param>
        /// <param name="context">Value to use as context in evaluating the expression.</param>
        /// <returns>The value that results from evaluating the expression in the context of the supplied value.</returns>
        public T EvaluateToClrValue<T>(IEdmExpression expression, IEdmStructuredValue context)
        {
            IEdmValue edmValue = this.Evaluate(expression, context);
            return this.edmToClrConverter.AsClrValue<T>(edmValue);
        }

        /// <summary>
        /// Evaluates an expression in the context of a value and a target type.
        /// </summary>
        /// <typeparam name="T">The CLR type of the value to be returned.</typeparam>
        /// <param name="expression">Expression to evaluate.</param>
        /// <param name="context">Value to use as context in evaluating the expression.</param>
        /// <param name="targetType">Type to which the result value is expected to conform.</param>
        /// <returns>The value that results from evaluating the expression in the context of the supplied value, asserted to be of the targetType.</returns>
        public T EvaluateToClrValue<T>(IEdmExpression expression, IEdmStructuredValue context, IEdmTypeReference targetType)
        {
            IEdmValue edmValue = this.Evaluate(expression, context, targetType);
            return this.edmToClrConverter.AsClrValue<T>(edmValue);
        }

        /// <summary>
        /// Find the Edm type from an <see cref="IEdmModel"/> by edm type name.
        /// </summary>
        /// <param name="edmTypeName">The edm type name.</param>
        /// <param name="edmModel">The edm model.</param>
        /// <returns>If the <see cref="IEdmType"/> exists, return it. Or return false.</returns>
        internal IEdmType ResolveEdmTypeFromName(string edmTypeName, IEdmModel edmModel)
        {
            string typeName = null;
            if (this.edmToClrConverter.TryGetClrTypeNameDelegate(edmModel, edmTypeName, out typeName))
            {
                return FindEdmType(typeName, edmModel);
            }

            return null;
        }
    }
}

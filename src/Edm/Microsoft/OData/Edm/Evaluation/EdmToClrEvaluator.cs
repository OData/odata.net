//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.EdmToClrConversion;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Edm.Evaluation
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
    }
}

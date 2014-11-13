//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using Microsoft.Data.Edm.EdmToClrConversion;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Values;

namespace Microsoft.Data.Edm.Evaluation
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
        public EdmToClrEvaluator(IDictionary<IEdmFunction, Func<IEdmValue[], IEdmValue>> builtInFunctions)
            : base(builtInFunctions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmToClrEvaluator"/> class.
        /// </summary>
        /// <param name="builtInFunctions">Builtin functions dictionary to the evaluators for the functions.</param>
        /// <param name="lastChanceFunctionApplier">Function to call to evaluate an application of a function with no static binding.</param>
        public EdmToClrEvaluator(IDictionary<IEdmFunction, Func<IEdmValue[], IEdmValue>> builtInFunctions, Func<string, IEdmValue[], IEdmValue> lastChanceFunctionApplier)
            : base(builtInFunctions, lastChanceFunctionApplier)
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

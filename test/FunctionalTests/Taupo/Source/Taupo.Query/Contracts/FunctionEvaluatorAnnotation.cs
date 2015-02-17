//---------------------------------------------------------------------
// <copyright file="FunctionEvaluatorAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Annotation for the function evaluation.
    /// </summary>
    public class FunctionEvaluatorAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the FunctionEvaluatorAnnotation class.
        /// </summary>
        /// <param name="functionEvaluator">The function evaluator which evaluates the function result based on the result type and argument values.</param>
        internal FunctionEvaluatorAnnotation(Func<QueryType, QueryValue[], QueryValue> functionEvaluator)
        {
            ExceptionUtilities.CheckArgumentNotNull(functionEvaluator, "functionEvaluator");
            this.FunctionEvaluator = functionEvaluator;
        }

        /// <summary>
        /// Gets the function evaluator which evaluates the function result based on the result type and argument values.
        /// </summary>
        /// <remarks>
        /// The evaluator takes query result type as the first parameter and array of the query values as second parameter
        /// and returns query value which is the restult of the evaluation of the function call for the given query values as input arguments.
        /// </remarks>
        /// <example>
        ///     var expression = myfunction.Call(arg1, arg2);
        ///     ... resolve expression
        ///     ... evaluate arguments
        ///     var result = functionEvaluator(expression.ExpressionType, evaluatedValueForArg1, evaluatedValueForArg2)
        /// </example>
        public Func<QueryType, QueryValue[], QueryValue> FunctionEvaluator { get; private set; }
    }
}

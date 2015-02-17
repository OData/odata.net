//---------------------------------------------------------------------
// <copyright file="LinqToAstoriaReplaceFunctionParameterReferenceVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.LinqToAstoria
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;
    
    /// <summary>
    /// Visitor that replaces function parameter references with arguments
    /// </summary>
    internal class LinqToAstoriaReplaceFunctionParameterReferenceVisitor : LinqToAstoriaExpressionReplacingVisitor
    {
        /// <summary>
        /// Initializes a new instance of the LinqToAstoriaReplaceFunctionParameterReferenceVisitor class
        /// </summary>
        /// <param name="function">The function for which to replace parameters</param>
        /// <param name="arguments">Argumnets for the function call</param>
        public LinqToAstoriaReplaceFunctionParameterReferenceVisitor(Function function, IEnumerable<QueryExpression> arguments)
        {
            this.PushFunctionCallToStack(function, arguments);
        }
        
        /// <summary>
        /// Replaces parameter reference with a corresponding argument of a function call
        /// </summary>
        /// <param name="expression">The expression to replace</param>
        /// <returns>The expression which represents corresponding argument for the parameter reference</returns>
        public override QueryExpression Visit(QueryFunctionParameterReferenceExpression expression)
        {
            return this.ReplaceFunctionParameterReference(expression);
        }
    }
}

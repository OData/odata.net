//---------------------------------------------------------------------
// <copyright file="ILinqQueryCodeGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq
{
    using System.CodeDom;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;

    /// <summary>
    /// Generates the tree of Linq.Expression nodes that represents Linq query from the given <see cref="QueryExpression"/> tree.
    /// </summary>
    [ImplementationSelector("LinqQueryCodeGenerator", HelpText = "Generator used to generate Linq query code", DefaultImplementation = "MethodSyntax")]
    public interface ILinqQueryCodeGenerator
    {
        /// <summary>
        /// Generates CodeExpression tree from the given <see cref="QueryExpression"/> tree.
        /// </summary>
        /// <param name="expression">The root node of the expression tree that the resulting tree will be built from.</param>
        /// <param name="context">Context from which all root queries are derived.</param>
        /// <returns>The tree that represents code to build a Linq query.</returns>
        CodeExpressionWithFreeVariables Generate(QueryExpression expression, CodeExpression context);
    }
}

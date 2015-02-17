//---------------------------------------------------------------------
// <copyright file="IClientQueryCodeGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System.CodeDom;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Generates a CodeDom Expression that represents a Linq Query produced from a Test QueryExpression
    /// </summary>
    [ImplementationSelector("LinqToAstoriaQueryCodeGenerator", HelpText = "Generator used to generate Linq to Astoria expression code", DefaultImplementation = "MethodSyntax")]
    public interface IClientQueryCodeGenerator
    {
        /// <summary>
        /// Generates CodeExpression tree from the given <see cref="QueryExpression"/> tree.
        /// </summary>
        /// <param name="expression">The root node of the expression tree that the resulting tree will be built from.</param>
        /// <param name="context">Context from which all root queries are derived.</param>
        /// <returns>The tree that represents code to build a Linq expression.</returns>
        CodeExpressionWithFreeVariables Generate(QueryExpression expression, CodeExpression context);
    }
}

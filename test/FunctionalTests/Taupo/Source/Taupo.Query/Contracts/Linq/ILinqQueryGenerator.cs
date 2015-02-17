//---------------------------------------------------------------------
// <copyright file="ILinqQueryGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq
{
    using System.Linq.Expressions;

    /// <summary>
    /// Generates the tree of Linq.Expression nodes that represents Linq query from the given <see cref="QueryExpression"/> tree.
    /// </summary>
    public interface ILinqQueryGenerator
    {
        /// <summary>
        /// Generates the tree of Linq.Expression nodes from the given <see cref="QueryExpression"/> tree.
        /// </summary>
        /// <param name="expression">The root node of the expression tree that the resulting tree will be built from.</param>
        /// <param name="context">Context for the query.</param>
        /// <param name="closures">Closures that are in scope. Properties of the type should match free variable names.</param>
        /// <returns>The tree that represents a Linq query.</returns>
        Expression Generate(QueryExpression expression, object context, params object[] closures);
    }
}

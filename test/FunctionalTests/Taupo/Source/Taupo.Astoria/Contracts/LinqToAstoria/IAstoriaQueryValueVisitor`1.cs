//---------------------------------------------------------------------
// <copyright file="IAstoriaQueryValueVisitor`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Visits QueryValue tree nodes specific to Astoria, following the double dispatch visitor pattern. 
    /// </summary>
    /// <typeparam name="TResult">The type which the visitor will return.</typeparam>
    public interface IAstoriaQueryValueVisitor<TResult> : IQueryValueVisitor<TResult>
    {
        /// <summary>
        /// Visits a QueryValue tree whose root node is the AstoriaQueryStreamValue.
        /// </summary>
        /// <param name="value">Query value being visited.</param>
        /// <returns>The result of visiting this query value.</returns>
        TResult Visit(AstoriaQueryStreamValue value);
    }
}
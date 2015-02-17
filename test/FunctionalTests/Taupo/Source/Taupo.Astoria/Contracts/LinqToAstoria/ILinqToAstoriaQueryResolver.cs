//---------------------------------------------------------------------
// <copyright file="ILinqToAstoriaQueryResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Resolves Linq to Astoria query expressions.
    /// </summary>
    [ImplementationSelector("LinqToAstoriaQueryResolver", DefaultImplementation = "Default")]
    public interface ILinqToAstoriaQueryResolver
    {
        /// <summary>
        /// Resolves the specified expression using Linq to Astoria resolution rules.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Resolved expression</returns>
        QueryExpression Resolve(QueryExpression expression);
    }
}
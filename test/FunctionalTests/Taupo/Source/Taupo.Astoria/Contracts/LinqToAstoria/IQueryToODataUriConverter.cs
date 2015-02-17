//---------------------------------------------------------------------
// <copyright file="IQueryToODataUriConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Contract for a component used to build OData uri's out of a query expression
    /// </summary>
    [ImplementationSelector("QueryToODataUriConverter", DefaultImplementation = "Default")]
    public interface IQueryToODataUriConverter
    {
        /// <summary>
        /// Builds an OData uri from the given query expression
        /// </summary>
        /// <param name="expression">The query expression</param>
        /// <returns>An OData uri built from the given expression</returns>
        ODataUri ComputeUri(QueryExpression expression);
    }
}

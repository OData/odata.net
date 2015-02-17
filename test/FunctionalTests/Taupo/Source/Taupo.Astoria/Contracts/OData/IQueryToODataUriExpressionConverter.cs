//---------------------------------------------------------------------
// <copyright file="IQueryToODataUriExpressionConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Contract for a component used to build OData string uri out of a query expression
    /// </summary>
    [ImplementationSelector("QueryToODataUriExpressionConverter", DefaultImplementation = "Default")]
    public interface IQueryToODataUriExpressionConverter
    {
        /// <summary>
        /// Builds an uri string from the given query expression
        /// </summary>
        /// <param name="expression">The query expression</param>
        /// <returns>An uri string built from the given expression</returns>
        string Convert(QueryExpression expression);
    }
}

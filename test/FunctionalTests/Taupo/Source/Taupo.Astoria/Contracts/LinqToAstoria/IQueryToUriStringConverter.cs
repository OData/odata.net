//---------------------------------------------------------------------
// <copyright file="IQueryToUriStringConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Contract for a component that builds a uri string from a query expression
    /// </summary>
    [ImplementationSelector("QueryToUriStringConverter", DefaultImplementation = "Default")]
    public interface IQueryToUriStringConverter
    {
        /// <summary>
        /// Gets the $expand query option string from the last uri computed
        /// </summary>
        string ExpandString { get; }

        /// <summary>
        /// Gets the $select query option string from the last uri computed
        /// </summary>
        string SelectString { get; }

        /// <summary>
        /// Builds a uri from the given query expression
        /// </summary>
        /// <param name="expression">The query expression</param>
        /// <returns>A uri built from the given expression</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings",
            Justification = "We want this to be a string")]
        string ComputeUri(QueryExpression expression);
    }
}

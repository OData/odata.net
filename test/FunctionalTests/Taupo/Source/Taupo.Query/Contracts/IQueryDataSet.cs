//---------------------------------------------------------------------
// <copyright file="IQueryDataSet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Data set for query evaluation.
    /// </summary>
    [ImplementationSelector("QueryDataSet", DefaultImplementation = "Default")]
    public interface IQueryDataSet
    {
        /// <summary>
        /// Gets the value of the root query with the specified name.
        /// </summary>
        /// <param name="rootQueryName">Name of the root query.</param>
        QueryCollectionValue this[string rootQueryName]
        {
            get;
        }
    }
}

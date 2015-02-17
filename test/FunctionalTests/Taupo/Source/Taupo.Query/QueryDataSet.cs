//---------------------------------------------------------------------
// <copyright file="QueryDataSet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Data set for query baseline evaluation.
    /// </summary>
    [ImplementationName(typeof(IQueryDataSet), "Default")]
    public class QueryDataSet : IQueryDataSet
    {
        /// <summary>
        /// Initializes a new instance of the QueryDataSet class.
        /// </summary>
        public QueryDataSet()
        {
            this.RootQueryData = new Dictionary<string, QueryCollectionValue>();
        }

        /// <summary>
        /// Gets the dictionary of root query data.
        /// </summary>
        public IDictionary<string, QueryCollectionValue> RootQueryData { get; private set; }

        /// <summary>
        /// Gets the <see cref="QueryCollectionValue"/> for the root query with the specified name.
        /// </summary>
        /// <param name="rootQueryName">Name of the root query.</param>
        public QueryCollectionValue this[string rootQueryName]
        {
            get { return this.RootQueryData[rootQueryName]; }
        }
    }
}
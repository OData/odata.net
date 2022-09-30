// <copyright file="ODataUriSlim.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.OData.UriParser.Aggregation;
using Microsoft.OData.UriParser;

namespace Microsoft.OData
{
    /// <summary>
    /// A lightweight version of <see cref="ODataUri"/> that is
    /// used by the writer to efficiently create a copy of the URI when entering
    /// each scope. This is used to avoid cloning <see cref="ODataUri"/>
    /// excessively.
    /// </summary>
    internal struct ODataUriSlim
    {
        private readonly ODataUri odataUri;

        /// <summary>
        /// Creates an instance of <see cref="ODataUriSlim"/>
        /// by copying the provided <see cref="ODataUri"/> instance.
        /// </summary>
        /// <param name="odataUri">The URI to copy.</param>
        public ODataUriSlim(ODataUri odataUri)
        {
            Debug.Assert(odataUri != null, "odataUri != null");
            this.odataUri = odataUri;
            this.SelectAndExpand = odataUri.SelectAndExpand;
            this.Path = odataUri.Path;
        }

        /// <summary>
        /// Creates an instance of <see cref="ODataUriSlim"/> by
        /// cloning the provided <see cref="ODataUriSlim"/>.
        /// </summary>
        /// <param name="odataUriSlim">The URI to copy.</param>
        public ODataUriSlim(ODataUriSlim odataUriSlim)
        {
            this.odataUri = odataUriSlim.odataUri;
            this.SelectAndExpand = odataUriSlim.SelectAndExpand;
            this.Path = odataUriSlim.Path;
        }

        /// <summary>
        /// Gets or sets the top level path for this uri.
        /// </summary>
        public ODataPath Path { get; set; }

        /// <summary>
        /// Gets or sets any $select or $expand option for this uri.
        /// </summary>
        public SelectExpandClause SelectAndExpand { get; set; }

        /// <summary>
        /// Gets the request Uri.
        /// </summary>
        public Uri RequestUri => this.odataUri.RequestUri;

        /// <summary>
        /// Gets the service root Uri.
        /// </summary>
        public Uri ServiceRoot => this.odataUri.ServiceRoot;

        /// <summary>
        /// Get the parameter alias nodes info.
        /// </summary>
        public IDictionary<string, SingleValueNode> ParameterAliasNodes => this.odataUri.ParameterAliasNodes;

        /// <summary>
        /// Gets any custom query options for this uri.
        /// </summary>
        public IEnumerable<QueryNode> CustomQueryOptions => this.odataUri.CustomQueryOptions;

        /// <summary>
        /// Gets any $filter option for this uri.
        /// </summary>
        public FilterClause Filter => this.odataUri.Filter;

        /// <summary>
        /// Gets any $orderby option for this uri.
        /// </summary>
        public OrderByClause OrderBy => this.odataUri.OrderBy;

        /// <summary>
        /// Gets any $search option for this uri.
        /// </summary>
        public SearchClause Search => this.odataUri.Search;

        /// <summary>
        /// Gets any $apply option for this uri.
        /// </summary>
        public ApplyClause Apply => this.odataUri.Apply;

        /// <summary>
        /// Gets any $compute option for this uri.
        /// </summary>
        public ComputeClause Compute => this.odataUri.Compute;

        /// <summary>
        /// Gets any $skip option for this uri.
        /// </summary>
        public long? Skip => this.odataUri.Skip;

        /// <summary>
        /// Gets any $top option for this uri.
        /// </summary>
        public long? Top => this.odataUri.Top;

        /// <summary>
        /// Gets any $index option for this uri.
        /// </summary>
        public long? Index => this.odataUri.Index;

        /// <summary>
        /// Get or sets any query $count option for this uri.
        /// </summary>
        public bool? QueryCount => this.odataUri.QueryCount;

        /// <summary>
        /// Gets any $skiptoken option for this uri.
        /// </summary>
        public string SkipToken => this.odataUri.SkipToken;

        /// <summary>
        /// Gets any $deltatoken option for this uri.
        /// </summary>
        public string DeltaToken => this.odataUri.DeltaToken;

        /// <summary>
        /// Get or sets the MetadataDocumentUri, which is always ServiceRoot + $metadata
        /// </summary>
        internal Uri MetadataDocumentUri => this.odataUri.MetadataDocumentUri;
    }
}

//---------------------------------------------------------------------
// <copyright file="ODataUri.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.OData.UriParser.Aggregation;
    using Microsoft.OData.UriParser;
    #endregion Namespaces

    /// <summary>
    /// The root node of a query. Holds the query itself plus additional metadata about the query.
    /// </summary>
    public sealed class ODataUri
    {
        /// <summary>
        /// Cache MetadataSegment as relative Uri
        /// </summary>
        private static readonly Uri MetadataSegment = new Uri(ODataConstants.UriMetadataSegment, UriKind.Relative);

        /// <summary>
        /// service Root Uri
        /// </summary>
        private Uri serviceRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataUri"/> class.
        /// </summary>
        public ODataUri()
        {
            IDictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.Ordinal);
            this.ParameterAliasValueAccessor = new ParameterAliasValueAccessor(dictionary);
        }

        /// <summary>
        /// Create a new ODataUri. This contains the semantic meaning of the
        /// entire uri.
        /// </summary>
        /// <param name="parameterAliasValueAccessor">The ParameterAliasValueAccessor.</param>
        /// <param name="path">The top level path for this uri.</param>
        /// <param name="customQueryOptions">Any custom query options for this uri. Can be null.</param>
        /// <param name="selectAndExpand">Any $select or $expand option for this uri. Can be null.</param>
        /// <param name="filter">Any $filter option for this uri. Can be null.</param>
        /// <param name="orderby">Any $orderby option for this uri. Can be null</param>
        /// <param name="search">Any $search option for this uri. Can be null</param>
        /// <param name="apply">Any $apply option for this uri. Can be null</param>
        /// <param name="skip">Any $skip option for this uri. Can be null.</param>
        /// <param name="top">Any $top option for this uri. Can be null.</param>
        /// <param name="queryCount">Any query $count option for this uri. Can be null.</param>
        /// <param name="compute">Any query $compute option for this uri. Can be null.</param>
        internal ODataUri(
            ParameterAliasValueAccessor parameterAliasValueAccessor,
            ODataPath path,
            IEnumerable<QueryNode> customQueryOptions,
            SelectExpandClause selectAndExpand,
            FilterClause filter,
            OrderByClause orderby,
            SearchClause search,
            ApplyClause apply,
            long? skip,
            long? top,
            bool? queryCount,
            ComputeClause compute = null)
        {
            this.ParameterAliasValueAccessor = parameterAliasValueAccessor;
            this.Path = path;
            this.CustomQueryOptions = new ReadOnlyCollection<QueryNode>(customQueryOptions.ToList());
            this.SelectAndExpand = selectAndExpand;
            this.Filter = filter;
            this.OrderBy = orderby;
            this.Search = search;
            this.Apply = apply;
            this.Skip = skip;
            this.Top = top;
            this.QueryCount = queryCount;
            this.Compute = compute;
        }

        /// <summary>
        /// Gets or sets the request Uri.
        /// </summary>
        public Uri RequestUri { get; set; }

        /// <summary>
        /// Gets or sets the service root Uri.
        /// </summary>
        public Uri ServiceRoot
        {
            get
            {
                return this.serviceRoot;
            }

            set
            {
                if (value == null)
                {
                    this.serviceRoot = null;
                    this.MetadataDocumentUri = null;
                    return;
                }

                if (!value.IsAbsoluteUri)
                {
                    throw new ODataException(Strings.WriterValidationUtils_MessageWriterSettingsServiceDocumentUriMustBeNullOrAbsolute(UriUtils.UriToString(value)));
                }

                this.serviceRoot = UriUtils.EnsureTaillingSlash(value);
                this.MetadataDocumentUri = new Uri(this.serviceRoot, MetadataSegment);
            }
        }

        /// <summary>
        /// Get the parameter alias nodes info.
        /// </summary>
        public IDictionary<string, SingleValueNode> ParameterAliasNodes
        {
            get
            {
                if (this.ParameterAliasValueAccessor == null)
                {
                    return null;
                }

                return this.ParameterAliasValueAccessor.ParameterAliasValueNodesCached;
            }
        }

        /// <summary>
        /// Gets or sets the top level path for this uri.
        /// </summary>
        public ODataPath Path { get; set; }

        /// <summary>
        /// Gets or sets any custom query options for this uri.
        /// </summary>
        public IEnumerable<QueryNode> CustomQueryOptions { get; set; }

        /// <summary>
        /// Gets or sets any $select or $expand option for this uri.
        /// </summary>
        public SelectExpandClause SelectAndExpand { get; set; }

        /// <summary>
        /// Gets or sets any $filter option for this uri.
        /// </summary>
        public FilterClause Filter { get; set; }

        /// <summary>
        /// Gets or sets any $orderby option for this uri.
        /// </summary>
        public OrderByClause OrderBy { get; set; }

        /// <summary>
        /// Gets or sets any $search option for this uri.
        /// </summary>
        public SearchClause Search { get; set; }

        /// <summary>
        /// Gets or sets any $apply option for this uri.
        /// </summary>
        public ApplyClause Apply { get; set; }

        /// <summary>
        /// Gets or sets any $compute option for this uri.
        /// </summary>
        public ComputeClause Compute { get; set; }

        /// <summary>
        /// Gets or sets any $skip option for this uri.
        /// </summary>
        public long? Skip { get; set; }

        /// <summary>
        /// Gets or sets any $top option for this uri.
        /// </summary>
        public long? Top { get; set; }

        /// <summary>
        /// Get or sets any query $count option for this uri.
        /// </summary>
        public bool? QueryCount { get; set; }

        /// <summary>
        /// Gets or sets any $skiptoken option for this uri.
        /// </summary>
        public string SkipToken { get; set; }

        /// <summary>
        /// Gets or sets any $deltatoken option for this uri.
        /// </summary>
        public string DeltaToken { get; set; }

        /// <summary>
        /// Get or sets the MetadataDocumentUri, which is always ServiceRoot + $metadata
        /// </summary>
        internal Uri MetadataDocumentUri { get; private set; }

        /// <summary>
        /// Gets or sets the ParameterAliasValueAccessor.
        /// </summary>
        internal ParameterAliasValueAccessor ParameterAliasValueAccessor { get; set; }

        /// <summary>
        /// Return a copy of current ODataUri.
        /// </summary>
        /// <returns>A copy of current ODataUri.</returns>
        public ODataUri Clone()
        {
            return new ODataUri()
            {
                RequestUri = RequestUri,
                serviceRoot = ServiceRoot, // Use field instead of property for perf.
                MetadataDocumentUri = MetadataDocumentUri,
                ParameterAliasValueAccessor = ParameterAliasValueAccessor,
                Path = Path,
                CustomQueryOptions = CustomQueryOptions,
                SelectAndExpand = SelectAndExpand,
                Apply = Apply,
                Filter = Filter,
                OrderBy = OrderBy,
                Search = Search,
                Skip = Skip,
                Top = Top,
                QueryCount = QueryCount,
                SkipToken = SkipToken,
                DeltaToken = DeltaToken,
            };
        }
    }
}

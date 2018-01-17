//---------------------------------------------------------------------
// <copyright file="ODataQueryOptionParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using Microsoft.OData.UriParser.Aggregation;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Parser for query options
    /// </summary>
    public class ODataQueryOptionParser
    {
        #region private fields
        /// <summary>Target Edm type. </summary>
        private readonly IEdmType targetEdmType;

        /// <summary> Dictionary of query options </summary>
        private readonly IDictionary<string, string> queryOptions;

        /// <summary>Filter clause.</summary>
        private FilterClause filterClause;

        /// <summary>SelectAndExpand clause.</summary>
        private SelectExpandClause selectExpandClause;

        /// <summary>Orderby clause.</summary>
        private OrderByClause orderByClause;

        /// <summary>Search clause.</summary>
        private SearchClause searchClause;

        /// <summary>
        /// Apply clause for aggregation queries
        /// </summary>
        private ApplyClause applyClause;

        /// <summary>
        /// Compute clause for computation queries
        /// </summary>
        private ComputeClause computeClause;

        /// <summary>
        /// The path info about parsed segments and target edm type and navigation source.
        /// </summary>
        private ODataPathInfo odataPathInfo;

        #endregion private fields

        #region constructor
        /// <summary>
        /// Constructor for ODataQueryOptionParser
        /// </summary>
        /// <param name="model">Model to use for metadata binding.</param>
        /// <param name="targetEdmType">The target EdmType to apply the query option on.</param>
        /// <param name="targetNavigationSource">The target navigation source to apply the query option on.</param>
        /// <param name="queryOptions">The dictionary storing query option key-value pairs.</param>
        public ODataQueryOptionParser(IEdmModel model, IEdmType targetEdmType, IEdmNavigationSource targetNavigationSource, IDictionary<string, string> queryOptions)
            : this(model, targetEdmType, targetNavigationSource, queryOptions, null)
        {
        }

        /// <summary>
        /// Constructor for ODataQueryOptionParser
        /// </summary>
        /// <param name="model">Model to use for metadata binding.</param>
        /// <param name="targetEdmType">The target EdmType to apply the query option on.</param>
        /// <param name="targetNavigationSource">The target navigation source to apply the query option on.</param>
        /// <param name="queryOptions">The dictionary storing query option key-value pairs.</param>
        /// <param name="container">The optional dependency injection container to get related services for URI parsing.</param>
        public ODataQueryOptionParser(IEdmModel model, IEdmType targetEdmType, IEdmNavigationSource targetNavigationSource, IDictionary<string, string> queryOptions, IServiceProvider container)
        {
            ExceptionUtils.CheckArgumentNotNull(queryOptions, "queryOptions");

            this.odataPathInfo = new ODataPathInfo(targetEdmType, targetNavigationSource);
            this.targetEdmType = this.odataPathInfo.TargetEdmType;
            this.queryOptions = queryOptions;
            this.Configuration = new ODataUriParserConfiguration(model, container)
            {
                ParameterAliasValueAccessor = new ParameterAliasValueAccessor(queryOptions.Where(_ => _.Key.StartsWith("@", StringComparison.Ordinal)).ToDictionary(_ => _.Key, _ => _.Value))
            };
        }

        /// <summary>
        /// Constructor for ODataQueryOptionParser
        /// </summary>
        /// <param name="model">Model to use for metadata binding.</param>
        /// <param name="odataPath">The odata path to apply the query option on.</param>
        /// <param name="queryOptions">The dictionary storing query option key-value pairs.</param>
        public ODataQueryOptionParser(IEdmModel model, ODataPath odataPath, IDictionary<string, string> queryOptions)
            : this(model, odataPath, queryOptions, null)
        {
        }

        /// <summary>
        /// Constructor for ODataQueryOptionParser
        /// </summary>
        /// <param name="model">Model to use for metadata binding.</param>
        /// <param name="odataPath">The odata path to apply the query option on.</param>
        /// <param name="queryOptions">The dictionary storing query option key-value pairs.</param>
        /// <param name="container">The optional dependency injection container to get related services for URI parsing.</param>
        public ODataQueryOptionParser(IEdmModel model, ODataPath odataPath, IDictionary<string, string> queryOptions, IServiceProvider container)
        {
            ExceptionUtils.CheckArgumentNotNull(odataPath, "odataPath");
            ExceptionUtils.CheckArgumentNotNull(queryOptions, "queryOptions");

            this.odataPathInfo = new ODataPathInfo(odataPath);
            this.targetEdmType = this.odataPathInfo.TargetEdmType;

            this.queryOptions = queryOptions;
            this.Configuration = new ODataUriParserConfiguration(model, container)
            {
                ParameterAliasValueAccessor = new ParameterAliasValueAccessor(queryOptions.Where(_ => _.Key.StartsWith("@", StringComparison.Ordinal)).ToDictionary(_ => _.Key, _ => _.Value))
            };
        }

        #endregion constructor

        #region properties
        /// <summary>
        /// The settings for this instance of <see cref="ODataQueryOptionParser"/>. Refer to the documentation for the individual properties of <see cref="ODataUriParserSettings"/> for more information.
        /// </summary>
        public ODataUriParserSettings Settings
        {
            get { return this.Configuration.Settings; }
        }

        /// <summary>
        /// Get the parameter alias nodes info.
        /// </summary>
        public IDictionary<string, SingleValueNode> ParameterAliasNodes
        {
            get { return this.Configuration.ParameterAliasValueAccessor.ParameterAliasValueNodesCached; }
        }

        /// <summary>
        /// Gets or sets the <see cref="ODataUriResolver"/> for <see cref="ODataUriParser"/>.
        /// </summary>
        public ODataUriResolver Resolver
        {
            get { return this.Configuration.Resolver; }
            set { this.Configuration.Resolver = value; }
        }

        /// <summary>The parser's configuration. </summary>
        internal ODataUriParserConfiguration Configuration { get; set; }
        #endregion properties

        #region public methods
        /// <summary>
        /// Parses a filter clause on the given full Uri, binding
        /// the text into semantic nodes using the constructed mode.
        /// </summary>
        /// <returns>A <see cref="FilterClause"/> representing the metadata bound filter expression.</returns>
        public FilterClause ParseFilter()
        {
            if (this.filterClause != null)
            {
                return this.filterClause;
            }

            string filterQuery;

            if (!this.TryGetQueryOption(UriQueryConstants.FilterQueryOption, out filterQuery)
                || string.IsNullOrEmpty(filterQuery)
                || this.targetEdmType == null)
            {
                return null;
            }

            this.filterClause = ParseFilterImplementation(filterQuery, this.Configuration, this.odataPathInfo);
            return this.filterClause;
        }

        /// <summary>
        /// Parses a apply clause on the given full Uri, binding
        /// the text into semantic nodes using the constructed mode.
        /// </summary>
        /// <returns>A <see cref="ApplyClause"/> representing the aggregation query.</returns>
        public ApplyClause ParseApply()
        {
            if (this.applyClause != null)
            {
                return this.applyClause;
            }

            string applyQuery;

            if (!this.TryGetQueryOption(UriQueryConstants.ApplyQueryOption, out applyQuery)
                || string.IsNullOrEmpty(applyQuery)
                || this.targetEdmType == null)
            {
                return null;
            }

            this.applyClause = ParseApplyImplementation(applyQuery, this.Configuration, this.odataPathInfo);
            return this.applyClause;
        }

        /// <summary>
        /// ParseSelectAndExpand from an instantiated class
        /// </summary>
        /// <returns>A SelectExpandClause with the semantic representation of select and expand terms</returns>
        public SelectExpandClause ParseSelectAndExpand()
        {
            if (this.selectExpandClause != null)
            {
                return this.selectExpandClause;
            }

            string selectQuery, expandQuery;

            // Intended to use bitwise AND & instead of logic AND && here, prevent short-circuiting.
            if ((!this.TryGetQueryOption(UriQueryConstants.SelectQueryOption, out selectQuery) || selectQuery == null)
                & (!this.TryGetQueryOption(UriQueryConstants.ExpandQueryOption, out expandQuery) || expandQuery == null)
                || this.targetEdmType == null)
            {
                return null;
            }

            IEdmStructuredType structuredType = this.targetEdmType as IEdmStructuredType;
            if (structuredType == null)
            {
                throw new ODataException(Strings.UriParser_TypeInvalidForSelectExpand(this.targetEdmType));
            }

            this.selectExpandClause = ParseSelectAndExpandImplementation(selectQuery, expandQuery, this.Configuration, this.odataPathInfo);
            return this.selectExpandClause;
        }

        /// <summary>
        /// Parses an orderBy clause on the given full Uri, binding
        /// the text into semantic nodes using the constructed mode.
        /// </summary>
        /// <returns>A <see cref="OrderByClause"/> representing the metadata bound orderby expression.</returns>
        public OrderByClause ParseOrderBy()
        {
            if (this.orderByClause != null)
            {
                return this.orderByClause;
            }

            string orderByQuery;
            if (!this.TryGetQueryOption(UriQueryConstants.OrderByQueryOption, out orderByQuery)
                || string.IsNullOrEmpty(orderByQuery)
                || this.targetEdmType == null)
            {
                return null;
            }

            this.orderByClause = ParseOrderByImplementation(orderByQuery, this.Configuration, this.odataPathInfo);
            return this.orderByClause;
        }

        /// <summary>
        /// Parses a $top query option
        /// </summary>
        /// <returns>A value representing that top option, null if $top query does not exist.</returns>
        public long? ParseTop()
        {
            string topQuery;
            return this.TryGetQueryOption(UriQueryConstants.TopQueryOption, out topQuery) ? ParseTop(topQuery) : null;
        }

        /// <summary>
        /// Parses a $skip query option
        /// </summary>
        /// <returns>A value representing that skip option, null if $skip query does not exist.</returns>
        public long? ParseSkip()
        {
            string skipQuery;
            return this.TryGetQueryOption(UriQueryConstants.SkipQueryOption, out skipQuery) ? ParseSkip(skipQuery) : null;
        }

        /// <summary>
        /// Parses a $count query option
        /// </summary>
        /// <returns>A count representing that count option, null if $count query does not exist.</returns>
        public bool? ParseCount()
        {
            string countQuery;
            return this.TryGetQueryOption(UriQueryConstants.CountQueryOption, out countQuery) ? ParseCount(countQuery) : null;
        }

        /// <summary>
        /// Parses the $search.
        /// </summary>
        /// <returns>SearchClause representing $search.</returns>
        public SearchClause ParseSearch()
        {
            if (this.searchClause != null)
            {
                return this.searchClause;
            }

            string searchQuery;
            if (!this.TryGetQueryOption(UriQueryConstants.SearchQueryOption, out searchQuery)
                || searchQuery == null)
            {
                return null;
            }

            this.searchClause = ParseSearchImplementation(searchQuery, this.Configuration);
            return searchClause;
        }

        /// <summary>
        /// Parses a $skiptoken query option
        /// </summary>
        /// <returns>A value representing that skip token option, null if $skiptoken query does not exist.</returns>
        public string ParseSkipToken()
        {
            string skipTokenQuery;
            return this.TryGetQueryOption(UriQueryConstants.SkipTokenQueryOption, out skipTokenQuery) ? skipTokenQuery : null;
        }

        /// <summary>
        /// Parses a $deltatoken query option
        /// </summary>
        /// <returns>A value representing that delta token option, null if $deltatoken query does not exist.</returns>
        public string ParseDeltaToken()
        {
            string deltaTokenQuery;
            return this.TryGetQueryOption(UriQueryConstants.DeltaTokenQueryOption, out deltaTokenQuery) ? deltaTokenQuery : null;
        }

        /// <summary>
        /// Parses a compute clause on the given full Uri, binding
        /// the text into semantic nodes using the constructed mode.
        /// </summary>
        /// <returns>A <see cref="ComputeClause"/> representing the computed properties.</returns>
        public ComputeClause ParseCompute()
        {
            if (this.computeClause != null)
            {
                return this.computeClause;
            }

            string computeQuery;

            if (!this.TryGetQueryOption(UriQueryConstants.ComputeQueryOption, out computeQuery)
                || string.IsNullOrEmpty(computeQuery)
                || this.targetEdmType == null)
            {
                return null;
            }

            this.computeClause = ParseComputeImplementation(computeQuery, this.Configuration, this.odataPathInfo);
            return this.computeClause;
        }
        #endregion public methods

        #region private methods
        /// <summary>
        /// Parses a <paramref name="filter"/> clause, binding
        /// the text into semantic nodes using the provided model.
        /// </summary>
        /// <param name="filter">String representation of the filter expression.</param>
        /// <param name="configuration">The configuration used for binding.</param>
        /// <param name="odataPathInfo">The path info from Uri path.</param>
        /// <returns>A <see cref="FilterClause"/> representing the metadata bound filter expression.</returns>
        private FilterClause ParseFilterImplementation(string filter, ODataUriParserConfiguration configuration, ODataPathInfo odataPathInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(configuration, "configuration");
            ExceptionUtils.CheckArgumentNotNull(odataPathInfo, "odataPathInfo");
            ExceptionUtils.CheckArgumentNotNull(filter, "filter");

            // Get the syntactic representation of the filter expression
            UriQueryExpressionParser expressionParser = new UriQueryExpressionParser(configuration.Settings.FilterLimit, configuration.EnableCaseInsensitiveUriFunctionIdentifier);
            QueryToken filterToken = expressionParser.ParseFilter(filter);

            // Bind it to metadata
            BindingState state = new BindingState(configuration, odataPathInfo.Segments.ToList());
            state.ImplicitRangeVariable = NodeFactory.CreateImplicitRangeVariable(odataPathInfo.TargetEdmType.ToTypeReference(), odataPathInfo.TargetNavigationSource);
            state.RangeVariables.Push(state.ImplicitRangeVariable);
            if (applyClause != null)
            {
                state.AggregatedPropertyNames = applyClause.GetLastAggregatedPropertyNames();
            }

            MetadataBinder binder = new MetadataBinder(state);
            FilterBinder filterBinder = new FilterBinder(binder.Bind, state);
            FilterClause boundNode = filterBinder.BindFilter(filterToken);

            return boundNode;
        }

        /// <summary>
        /// Parses an <paramref name="apply"/> clause, binding
        /// the text into a metadata-bound or dynamic properties to be applied using the provided model.
        /// </summary>
        /// <param name="apply">String representation of the apply expression.</param>
        /// <param name="configuration">The configuration used for binding.</param>
        /// <param name="odataPathInfo">The path info from Uri path.</param>
        /// <returns>A <see cref="ApplyClause"/> representing the metadata bound apply expression.</returns>
        private static ApplyClause ParseApplyImplementation(string apply, ODataUriParserConfiguration configuration, ODataPathInfo odataPathInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(configuration, "configuration");
            ExceptionUtils.CheckArgumentNotNull(apply, "apply");

            // Get the syntactic representation of the apply expression
            UriQueryExpressionParser expressionParser = new UriQueryExpressionParser(configuration.Settings.FilterLimit, configuration.EnableCaseInsensitiveUriFunctionIdentifier);
            var applyTokens = expressionParser.ParseApply(apply);

            // Bind it to metadata
            BindingState state = new BindingState(configuration, odataPathInfo.Segments.ToList());
            state.ImplicitRangeVariable = NodeFactory.CreateImplicitRangeVariable(odataPathInfo.TargetEdmType.ToTypeReference(), odataPathInfo.TargetNavigationSource);
            state.RangeVariables.Push(state.ImplicitRangeVariable);
            MetadataBinder binder = new MetadataBinder(state);
            ApplyBinder applyBinder = new ApplyBinder(binder.Bind, state);
            ApplyClause boundNode = applyBinder.BindApply(applyTokens);

            return boundNode;
        }

        /// <summary>
        /// Parses the <paramref name="select"/> and <paramref name="expand"/> clauses, binding
        /// the text into a metadata-bound list of properties to be selected using the provided model.
        /// </summary>
        /// <param name="select">String representation of the select expression from the URI.</param>
        /// <param name="expand">String representation of the expand expression from the URI.</param>
        /// <param name="configuration">The configuration used for binding.</param>
        /// <param name="odataPathInfo">The path info from Uri path.</param>
        /// <returns>A <see cref="SelectExpandClause"/> representing the metadata bound select and expand expression.</returns>
        private static SelectExpandClause ParseSelectAndExpandImplementation(string select, string expand, ODataUriParserConfiguration configuration, ODataPathInfo odataPathInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(configuration, "configuration");
            ExceptionUtils.CheckArgumentNotNull(configuration.Model, "model");

            ExpandToken expandTree;
            SelectToken selectTree;

            // syntactic pass , pass in the expand parent entity type name, in case expand option contains star, will get all the parent entity navigation properties (both declared and dynamical).
            SelectExpandSyntacticParser.Parse(select, expand, odataPathInfo.TargetStructuredType, configuration, out expandTree, out selectTree);

            // semantic pass
            return SelectExpandSemanticBinder.Bind(odataPathInfo, expandTree, selectTree, configuration);
        }

        /// <summary>
        /// Parses an <paramref name="orderBy "/> clause, binding
        /// the text into semantic nodes using the provided model.
        /// </summary>
        /// <param name="orderBy">String representation of the orderby expression.</param>
        /// <param name="configuration">The configuration used for binding.</param>
        /// <param name="odataPathInfo">The path info from Uri path.</param>
        /// <returns>An <see cref="OrderByClause"/> representing the metadata bound orderby expression.</returns>
        private OrderByClause ParseOrderByImplementation(string orderBy, ODataUriParserConfiguration configuration, ODataPathInfo odataPathInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(configuration, "configuration");
            ExceptionUtils.CheckArgumentNotNull(configuration.Model, "model");
            ExceptionUtils.CheckArgumentNotNull(orderBy, "orderBy");

            // Get the syntactic representation of the orderby expression
            UriQueryExpressionParser expressionParser = new UriQueryExpressionParser(configuration.Settings.OrderByLimit, configuration.EnableCaseInsensitiveUriFunctionIdentifier);
            var orderByQueryTokens = expressionParser.ParseOrderBy(orderBy);

            // Bind it to metadata
            BindingState state = new BindingState(configuration, odataPathInfo.Segments.ToList());
            state.ImplicitRangeVariable = NodeFactory.CreateImplicitRangeVariable(odataPathInfo.TargetEdmType.ToTypeReference(), odataPathInfo.TargetNavigationSource);
            state.RangeVariables.Push(state.ImplicitRangeVariable);
            if (applyClause != null)
            {
                state.AggregatedPropertyNames = applyClause.GetLastAggregatedPropertyNames();
            }

            MetadataBinder binder = new MetadataBinder(state);
            OrderByBinder orderByBinder = new OrderByBinder(binder.Bind);
            OrderByClause orderByClause = orderByBinder.BindOrderBy(state, orderByQueryTokens);

            return orderByClause;
        }

        /// <summary>
        /// Parses a $top query option
        /// </summary>
        /// <param name="topQuery">The topQuery from the query</param>
        /// <returns>A value representing that top option, null if $top query does not exist.</returns>
        /// <exception cref="ODataException">Throws if the input count is not a valid $top value.</exception>
        private static long? ParseTop(string topQuery)
        {
            if (topQuery == null)
            {
                return null;
            }

            long topValue;
            if (!long.TryParse(topQuery, out topValue) || topValue < 0)
            {
                throw new ODataException(Strings.SyntacticTree_InvalidTopQueryOptionValue(topQuery));
            }

            return topValue;
        }

        /// <summary>
        /// Parses a $skip query option
        /// </summary>
        /// <param name="skipQuery">The count skipQuery from the query</param>
        /// <returns>A value representing that skip option, null if $skip query does not exist.</returns>
        /// <exception cref="ODataException">Throws if the input count is not a valid $skip value.</exception>
        private static long? ParseSkip(string skipQuery)
        {
            if (skipQuery == null)
            {
                return null;
            }

            long skipValue;
            if (!long.TryParse(skipQuery, out skipValue) || skipValue < 0)
            {
                throw new ODataException(Strings.SyntacticTree_InvalidSkipQueryOptionValue(skipQuery));
            }

            return skipValue;
        }

        /// <summary>
        /// Parses a query count option
        /// Valid Samples: $count=true; $count=false
        /// Invalid Samples: $count=True; $count=ture
        /// </summary>
        /// <param name="count">The count string from the query</param>
        /// <returns>query count true of false</returns>
        /// <exception cref="ODataException">Throws if the input count is not a valid $count value.</exception>
        private static bool? ParseCount(string count)
        {
            if (count == null)
            {
                return null;
            }

            switch (count.Trim())
            {
                case ExpressionConstants.KeywordTrue:
                    return true;
                case ExpressionConstants.KeywordFalse:
                    return false;
                default:
                    throw new ODataException(Strings.ODataUriParser_InvalidCount(count));
            }
        }

        /// <summary>
        /// Parses the <paramref name="search"/> clause, binding
        /// the text into a metadata-bound list of properties to be selected using the provided model.
        /// </summary>
        /// <param name="search">String representation of the search expression from the URI.</param>
        /// <param name="configuration">The configuration used for binding.</param>
        /// <returns>A <see cref="SearchClause"/> representing the metadata bound search expression.</returns>
        private static SearchClause ParseSearchImplementation(string search, ODataUriParserConfiguration configuration)
        {
            ExceptionUtils.CheckArgumentNotNull(configuration, "configuration");
            ExceptionUtils.CheckArgumentNotNull(search, "search");

            SearchParser searchParser = new SearchParser(configuration.Settings.SearchLimit);
            QueryToken queryToken = searchParser.ParseSearch(search);

            // Bind it to metadata
            BindingState state = new BindingState(configuration);
            MetadataBinder binder = new MetadataBinder(state);
            SearchBinder searchBinder = new SearchBinder(binder.Bind);

            return searchBinder.BindSearch(queryToken);
        }

        /// <summary>
        /// Parses the <paramref name="compute"/> clause, binding
        /// the text into a metadata-bound list of compuations using the provided model.
        /// </summary>
        /// <param name="compute">String representation of the compute expression from the URI.</param>
        /// <param name="configuration">The configuration used for binding.</param>
        /// <param name="odataPathInfo">The path info from Uri path.</param>
        /// <returns>A <see cref="ComputeClause"/> representing the metadata bound compute expression.</returns>
        private static ComputeClause ParseComputeImplementation(string compute, ODataUriParserConfiguration configuration, ODataPathInfo odataPathInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(configuration, "configuration");
            ExceptionUtils.CheckArgumentNotNull(compute, "compute");

            // Get the syntactic representation of the apply expression
            UriQueryExpressionParser expressionParser = new UriQueryExpressionParser(configuration.Settings.FilterLimit, configuration.EnableCaseInsensitiveUriFunctionIdentifier);
            ComputeToken computeToken = expressionParser.ParseCompute(compute);

            // Bind it to metadata
            BindingState state = new BindingState(configuration);
            state.ImplicitRangeVariable = NodeFactory.CreateImplicitRangeVariable(odataPathInfo.TargetEdmType.ToTypeReference(), odataPathInfo.TargetNavigationSource);
            state.RangeVariables.Push(state.ImplicitRangeVariable);
            MetadataBinder binder = new MetadataBinder(state);
            ComputeBinder computeBinder = new ComputeBinder(binder.Bind);
            ComputeClause boundNode = computeBinder.BindCompute(computeToken);

            return boundNode;
        }

        /// <summary>
        /// Gets query options according to case sensitivity and whether no dollar query options is enabled.
        /// </summary>
        /// <param name="name">The query option name with $ prefix.</param>
        /// <param name="value">The value of the query option.</param>
        /// <returns>Whether value successfully retrived.</returns>
        private bool TryGetQueryOption(string name, out string value)
        {
            value = null;
            if (name == null)
            {
                return false;
            }

            // Trim name to prevent caller from passing in untrimmed name for comparison with already trimmed keys in queryOptions dictionary.
            string trimmedName = name.Trim();

            bool isCaseInsensitiveEnabled = this.Resolver.EnableCaseInsensitive;
            bool isNoDollarQueryOptionsEnabled = this.Configuration.EnableNoDollarQueryOptions;

            if (!isCaseInsensitiveEnabled && !isNoDollarQueryOptionsEnabled)
            {
                return this.queryOptions.TryGetValue(trimmedName, out value);
            }

            StringComparison stringComparison = isCaseInsensitiveEnabled ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            string nameWithoutDollarPrefix = (isNoDollarQueryOptionsEnabled && trimmedName.StartsWith(UriQueryConstants.DollarSign, StringComparison.Ordinal)) ?
                trimmedName.Substring(1) : null;

            var list = this.queryOptions
                .Where(pair => string.Equals(trimmedName, pair.Key, stringComparison)
                || (nameWithoutDollarPrefix != null && string.Equals(nameWithoutDollarPrefix, pair.Key, stringComparison)))
                .ToList();

            if (list.Count == 0)
            {
                return false;
            }
            else if (list.Count == 1)
            {
                value = list.First().Value;
                return true;
            }

            throw new ODataException(Strings.QueryOptionUtils_QueryParameterMustBeSpecifiedOnce(
                isNoDollarQueryOptionsEnabled ? string.Format(CultureInfo.InvariantCulture, "${0}/{0}", nameWithoutDollarPrefix ?? trimmedName) : trimmedName));
        }
        #endregion private methods
    }
}
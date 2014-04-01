//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Main Public API to parse an ODataURI.
    /// </summary>
    public sealed class ODataUriParser
    {
        #region Fields
        /// <summary>The parser's configuration. </summary>
        private readonly ODataUriParserConfiguration configuration;

        /// <summary>The full Uri to be parsed. </summary>
        private readonly Uri fullUri;

        /// <summary>Query option list</summary>
        private readonly List<CustomQueryOptionToken> queryOptions;

        /// <summary>Target Edm type. </summary>
        private IEdmType targetEdmType;

        /// <summary>Target Edm navigation source. </summary>
        private IEdmNavigationSource targetNavigationSource;

        /// <summary>OData Path.</summary>
        private ODataPath odataPath;

        /// <summary>Filter clause.</summary>
        private FilterClause filterClause;

        /// <summary>Orderby clause.</summary>
        private OrderByClause orderByClause;

        /// <summary>SelectAndExpand clause.</summary>
        private SelectExpandClause selectExpandClause;

        /// <summary>EntityId Segment.</summary>
        private EntityIdSegment entityIdSegment;

        /// <summary>Search clause.</summary>
        private SearchClause searchClause;
        #endregion

        /// <summary>
        /// Build an ODataUriParser
        /// </summary>
        /// <param name="model">Model to use for metadata binding.</param>
        /// <param name="serviceRoot">Absolute URI of the service root.</param>
        /// <param name="fullUri">full Uri to be parsed</param>
        public ODataUriParser(IEdmModel model, Uri serviceRoot, Uri fullUri)
        {
            ExceptionUtils.CheckArgumentNotNull(fullUri, "fullUri");
            if (serviceRoot == null)
            {
                throw new ODataException(ODataErrorStrings.UriParser_NeedServiceRootForThisOverload);
            }

            if (!serviceRoot.IsAbsoluteUri)
            {
                throw new ODataException(ODataErrorStrings.UriParser_UriMustBeAbsolute(serviceRoot));
            }

            this.configuration = new ODataUriParserConfiguration(model, serviceRoot);
            this.fullUri = fullUri.IsAbsoluteUri ? fullUri : Core.UriUtils.UriToAbsoluteUri(this.ServiceRoot, fullUri);
            this.queryOptions = UriUtils.ParseQueryOptions(this.fullUri);
        }

        /// <summary>
        /// Build an ODataUriParser
        /// </summary>
        /// <param name="model">Model to use for metadata binding.</param>
        /// <param name="serviceRoot">Absolute URI of the service root.</param>
        /// <exception cref="System.ArgumentNullException">Throws if input model is null.</exception>
        /// <exception cref="ArgumentException">Throws if the input serviceRoot is not an AbsoluteUri</exception>
        internal ODataUriParser(IEdmModel model, Uri serviceRoot)
        {
            this.configuration = new ODataUriParserConfiguration(model, serviceRoot);
        }

        /// <summary>
        /// The settings for this instance of <see cref="ODataUriParser"/>. Refer to the documentation for the individual properties of <see cref="ODataUriParserSettings"/> for more information.
        /// </summary>
        public ODataUriParserSettings Settings
        {
            get { return this.configuration.Settings; }
        }

        /// <summary>
        /// Gets the model for this ODataUriParser
        /// </summary>
        public IEdmModel Model
        {
            get { return this.configuration.Model; }
        }

        /// <summary>
        /// Gets the absolute URI of the service root.
        /// </summary>
        public Uri ServiceRoot
        {
            get { return this.configuration.ServiceRoot; }
        }

        /// <summary>
        /// Gets or Sets the <see cref="ODataUrlConventions"/> to use while parsing, specifically
        /// whether to recognize keys as segments or not.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Throws if the input value is null.</exception>
        public ODataUrlConventions UrlConventions
        {
            get { return this.configuration.UrlConventions; }
            set { this.configuration.UrlConventions = value; }
        }

        /// <summary>
        /// Gets or Sets a callback that returns a BatchReferenceSegment (to be used for $0 in batch)
        /// </summary>
        public Func<string, BatchReferenceSegment> BatchReferenceCallback
        {
            get { return this.configuration.BatchReferenceCallback; }
            set { this.configuration.BatchReferenceCallback = value; }
        }

        /// <summary>
        /// Get the parameter alias nodes info.
        /// </summary>
        public IDictionary<string, SingleValueNode> ParameterAliasNodes
        {
            get { return this.ParameterAliasValueAccessor.ParameterAliasValueNodesCached; }
        }

        /// <summary>
        /// Gets or sets the parameter aliases info for MetadataBinder to resolve parameter alias' metadata type.
        /// </summary>
        internal ParameterAliasValueAccessor ParameterAliasValueAccessor
        {
            get { return this.configuration.ParameterAliasValueAccessor; }
            set { this.configuration.ParameterAliasValueAccessor = value; }
        }

        /// <summary>
        /// Parses the odata path on the given full Uri
        /// </summary>
        /// <returns>An <see cref="ODataPath"/> representing OData path.</returns>
        public ODataPath ParsePath()
        {
            this.Initialize();
            return this.odataPath;
        }

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

            this.Initialize();
            string filterQuery = queryOptions.GetQueryOptionValue(UriQueryConstants.FilterQueryOption);
            if (string.IsNullOrEmpty(filterQuery) || this.targetEdmType == null)
            {
                return null;
            }

            this.filterClause = this.ParseFilterImplementation(filterQuery, this.targetEdmType, this.targetNavigationSource);
            return this.filterClause;
        }

        /// <summary>
        /// Parses a orderBy clause on the given full Uri, binding
        /// the text into semantic nodes using the constructed mode.
        /// </summary>
        /// <returns>A <see cref="OrderByClause"/> representing the metadata bound orderby expression.</returns>
        public OrderByClause ParseOrderBy()
        {
            if (this.orderByClause != null)
            {
                return this.orderByClause;
            }

            this.Initialize();
            string orderByQuery = queryOptions.GetQueryOptionValue(UriQueryConstants.OrderByQueryOption);
            if (string.IsNullOrEmpty(orderByQuery) || this.targetEdmType == null)
            {
                return null;
            }

            this.orderByClause = this.ParseOrderByImplementation(orderByQuery, this.targetEdmType, this.targetNavigationSource);
            return this.orderByClause;
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

            this.Initialize();
            string selectQuery = queryOptions.GetQueryOptionValue(UriQueryConstants.SelectQueryOption);
            string expandQuery = queryOptions.GetQueryOptionValue(UriQueryConstants.ExpandQueryOption);
            if (selectQuery == null && expandQuery == null || this.targetEdmType == null)
            {
                return null;
            }

            this.selectExpandClause = this.ParseSelectAndExpandImplementation(selectQuery, expandQuery, this.targetEdmType as IEdmStructuredType, this.targetNavigationSource);
            return this.selectExpandClause;
        }

        /// <summary>
        /// Parses the entity identifier.
        /// </summary>
        /// <returns>EntityIdSegment contained absolute Uri representing $id</returns>
        public EntityIdSegment ParseEntityId()
        {
            if (this.entityIdSegment != null)
            {
                return this.entityIdSegment;
            }

            string idQuery = queryOptions.GetQueryOptionValue(UriQueryConstants.IdQueryOption);

            if (idQuery == null)
            {
                return null;
            }

            Uri idUri = new Uri(idQuery, UriKind.RelativeOrAbsolute);

            if (!idUri.IsAbsoluteUri)
            {
                idUri = new Uri(this.fullUri, idUri);
            }

            this.entityIdSegment = new EntityIdSegment(idUri);
            return this.entityIdSegment;
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

            this.Initialize();
            string searchQuery = queryOptions.GetQueryOptionValue(UriQueryConstants.SearchQueryOption);

            if (searchQuery == null)
            {
                return null;
            }

            this.searchClause = this.ParseSearchImplementation(searchQuery);
            return searchClause;
        }

        /// <summary>
        /// Parses a <paramref name="filter"/> clause on the given <paramref name="elementType"/>, binding
        /// the text into semantic nodes using the provided <paramref name="model"/>.
        /// </summary>
        /// <param name="filter">String representation of the filter expression.</param>
        /// <param name="model">Model to use for metadata binding.</param>
        /// <param name="elementType">Type that the filter clause refers to.</param>
        /// <returns>A <see cref="FilterClause"/> representing the metadata bound filter expression.</returns>
        internal static FilterClause ParseFilter(string filter, IEdmModel model, IEdmType elementType)
        {
            ODataUriParser parser = new ODataUriParser(model, null);
            return parser.ParseFilter(filter, elementType, null);
        }

        /// <summary>
        /// Parses a <paramref name="filter"/> clause on the given <paramref name="elementType"/>, binding
        /// the text into semantic nodes using the provided <paramref name="model"/>.
        /// </summary>
        /// <param name="filter">String representation of the filter expression.</param>
        /// <param name="model">Model to use for metadata binding.</param>
        /// <param name="elementType">Type that the filter clause refers to.</param>
        /// <param name="navigationSource">NavigationSource that the elements beign filtered are from.</param>
        /// <returns>A <see cref="FilterClause"/> representing the metadata bound filter expression.</returns>
        internal static FilterClause ParseFilter(string filter, IEdmModel model, IEdmType elementType, IEdmNavigationSource navigationSource)
        {
            ODataUriParser parser = new ODataUriParser(model, null);
            return parser.ParseFilter(filter, elementType, navigationSource);
        }

        /// <summary>
        /// Parses a <paramref name="orderBy"/> clause on the given <paramref name="elementType"/>, binding
        /// the text into semantic nodes using the provided <paramref name="model"/>.
        /// </summary>
        /// <param name="orderBy">String representation of the orderby expression.</param>
        /// <param name="model">Model to use for metadata binding.</param>
        /// <param name="elementType">Type that the orderby clause refers to.</param>
        /// <returns>A <see cref="OrderByClause"/> representing the metadata bound orderby expression.</returns>
        internal static OrderByClause ParseOrderBy(string orderBy, IEdmModel model, IEdmType elementType)
        {
            ODataUriParser parser = new ODataUriParser(model, null);
            return parser.ParseOrderBy(orderBy, elementType, null);
        }

        /// <summary>
        /// Parses a <paramref name="orderBy "/> clause on the given <paramref name="elementType"/>, binding
        /// the text into semantic nodes using the provided <paramref name="model"/>.
        /// </summary>
        /// <param name="orderBy">String representation of the orderby expression.</param>
        /// <param name="model">Model to use for metadata binding.</param>
        /// <param name="elementType">Type that the orderby clause refers to.</param>
        /// <param name="navigationSource">Navigation source that the elements beign filtered are from.</param>
        /// <returns>A <see cref="OrderByClause"/> representing the metadata bound orderby expression.</returns>
        internal static OrderByClause ParseOrderBy(string orderBy, IEdmModel model, IEdmType elementType, IEdmNavigationSource navigationSource)
        {
            ODataUriParser parser = new ODataUriParser(model, null);
            return parser.ParseOrderBy(orderBy, elementType, navigationSource);
        }

        /// <summary>
        /// Parse a filter clause from an instantiated class.
        /// </summary>
        /// <param name="filter">the filter clause to parse</param>
        /// <param name="elementType">Type that the select and expand clauses are projecting.</param>
        /// <param name="navigationSource">Navigation source that the elements being filtered are from.</param>
        /// <returns>A FilterClause representing the metadata bound filter expression.</returns>
        internal FilterClause ParseFilter(string filter, IEdmType elementType, IEdmNavigationSource navigationSource)
        {
            return this.ParseFilterImplementation(filter, elementType, navigationSource);
        }

        /// <summary>
        /// Parse an orderby clause from an instance of this class
        /// </summary>
        /// <param name="orderBy">the orderby clause to parse</param>
        /// <param name="elementType">Type that the select and expand clauses are projecting.</param>
        /// <param name="navigationSource">Navigation source that the elements being filtered are from.</param>
        /// <returns>An OrderByClause representing the metadata bound orderby expression.</returns>
        internal OrderByClause ParseOrderBy(string orderBy, IEdmType elementType, IEdmNavigationSource navigationSource)
        {
            return this.ParseOrderByImplementation(orderBy, elementType, navigationSource);
        }

        /// <summary>
        /// Parses a <paramref name="pathUri"/> into a semantic <see cref="ODataPath"/> object. 
        /// </summary>
        /// <remarks>
        /// This is designed to parse the Path of a URL. If it is used to parse paths that are contained
        /// within other places, such as $filter expressions, then it may not enforce correct rules.
        /// </remarks>
        /// <param name="pathUri">The absolute URI which holds the path to parse.</param>
        /// <returns>An <see cref="ODataPath"/> representing the metadata-bound path expression.</returns>
        /// <exception cref="ODataException">Throws if the input path is not an absolute uri.</exception>
        internal ODataPath ParsePath(Uri pathUri)
        {
            ExceptionUtils.CheckArgumentNotNull(pathUri, "pathUri");

            if (!pathUri.IsAbsoluteUri)
            {
                throw new ODataException(ODataErrorStrings.UriParser_UriMustBeAbsolute(pathUri));
            }

            UriPathParser pathParser = new UriPathParser(this.Settings.PathLimit);
            var rawSegments = pathParser.ParsePathIntoSegments(pathUri, this.configuration.ServiceRoot);
            return ODataPathFactory.BindPath(rawSegments, this.configuration);
        }

        /// <summary>
        /// ParseSelectAndExpand from an instantiated class
        /// </summary>
        /// <param name="select">the select to parse</param>
        /// <param name="expand">the expand to parse</param>
        /// <param name="elementType">Type that the select and expand clauses are projecting.</param>
        /// <param name="navigationSource">Navigation source that the elements being filtered are from. This can be null, if so that null will propagate through the resulting SelectExpandClause.</param>
        /// <returns>A SelectExpandClause with the semantic representation of select and expand terms</returns>
        internal SelectExpandClause ParseSelectAndExpand(string select, string expand, IEdmStructuredType elementType, IEdmNavigationSource navigationSource)
        {
            return this.ParseSelectAndExpandImplementation(select, expand, elementType, navigationSource);
        }

        /// <summary>
        /// Parse a full Uri into its contingent parts with semantic meaning attached to each part. 
        /// See <see cref="ODataUri"/>.
        /// </summary>
        /// <returns>An <see cref="ODataUri"/> representing the full uri.</returns>
        internal ODataUri ParseUri()
        {
            return this.ParseUriImplementation();
        }

        /// <summary>
        /// Parses a query count option
        /// </summary>
        /// <param name="count">The count string from the query</param>
        /// <returns>An count representing that count option.</returns>
        internal bool ParseCount(string count)
        {
            return this.ParseQueryCountImplementation(count);
        }

        /// <summary>
        /// Initialize a UriParser. We have to initialize UriParser seperately for parsing path, because we may set BatchReferenceCallback before ParsePath.
        /// </summary>
        private void Initialize()
        {
            if (this.odataPath != null)
            {
                return;
            }

            this.ParameterAliasValueAccessor = new ParameterAliasValueAccessor(queryOptions.GetParameterAliases());
            this.odataPath = this.ParsePath(this.fullUri);
            ODataPathSegment lastSegment = this.odataPath.LastSegment;
            ODataPathSegment previous = null;
            var segs = odataPath.GetEnumerator();
            int count = 0;
            while (++count < odataPath.Count && segs.MoveNext())
            {
            }

            previous = segs.Current;
            if (lastSegment != null)
            {
                // use previous segment if the last one is Key or Count Segment
                if (lastSegment is KeySegment || lastSegment is CountSegment)
                {
                    lastSegment = previous;
                }

                this.targetNavigationSource = lastSegment.TargetEdmNavigationSource;
                this.targetEdmType = lastSegment.TargetEdmType;
                if (this.targetEdmType != null)
                {
                    IEdmEntityType type;
                    if (this.targetEdmType.IsEntityOrEntityCollectionType(out type))
                    {
                        this.targetEdmType = type;
                    }
                }
            }
        }

        /// <summary>
        /// Parses the full Uri.
        /// </summary>
        /// <returns>An ODataUri representing the full uri</returns>
        private ODataUri ParseUriImplementation()
        {
            ExceptionUtils.CheckArgumentNotNull(this.configuration.Model, "model");
            ExceptionUtils.CheckArgumentNotNull(this.configuration.ServiceRoot, "serviceRoot");
            ExceptionUtils.CheckArgumentNotNull(this.fullUri, "fullUri");

            SyntacticTree syntax = SyntacticTree.ParseUri(this.fullUri, this.configuration.ServiceRoot, this.Settings.FilterLimit);
            ExceptionUtils.CheckArgumentNotNull(syntax, "syntax");
            BindingState state = new BindingState(this.configuration);
            MetadataBinder binder = new MetadataBinder(state);
            ODataUriSemanticBinder uriBinder = new ODataUriSemanticBinder(state, binder.Bind);
            return uriBinder.BindTree(syntax);
        }

        /// <summary>
        /// Parses a <paramref name="filter"/> clause on the given <paramref name="elementType"/>, binding
        /// the text into semantic nodes using the provided.
        /// </summary>
        /// <param name="filter">String representation of the filter expression.</param>
        /// <param name="elementType">Type that the filter clause refers to.</param>
        /// <param name="navigationSource">Navigation source that the elements beign filtered are from.</param>
        /// <returns>A <see cref="FilterClause"/> representing the metadata bound filter expression.</returns>
        private FilterClause ParseFilterImplementation(string filter, IEdmType elementType, IEdmNavigationSource navigationSource)
        {
            ExceptionUtils.CheckArgumentNotNull(this.configuration, "this.configuration");
            ExceptionUtils.CheckArgumentNotNull(elementType, "elementType");
            ExceptionUtils.CheckArgumentNotNull(filter, "filter");

            // Get the syntactic representation of the filter expression
            UriQueryExpressionParser expressionParser = new UriQueryExpressionParser(this.Settings.FilterLimit);
            QueryToken filterToken = expressionParser.ParseFilter(filter);

            // Bind it to metadata
            BindingState state = new BindingState(this.configuration);
            state.ImplicitRangeVariable = NodeFactory.CreateImplicitRangeVariable(elementType.ToTypeReference(), navigationSource);
            state.RangeVariables.Push(state.ImplicitRangeVariable);
            MetadataBinder binder = new MetadataBinder(state);
            FilterBinder filterBinder = new FilterBinder(binder.Bind, state);
            FilterClause boundNode = filterBinder.BindFilter(filterToken);

            return boundNode;
        }

        /// <summary>
        /// Parses a <paramref name="orderBy "/> clause on the given <paramref name="elementType"/>, binding
        /// the text into semantic nodes using the provided model.
        /// </summary>
        /// <param name="orderBy">String representation of the orderby expression.</param>
        /// <param name="elementType">Type that the orderby clause refers to.</param>
        /// <param name="navigationSource">avigationSource that the elements beign filtered are from.</param>
        /// <returns>A <see cref="OrderByClause"/> representing the metadata bound orderby expression.</returns>
        private OrderByClause ParseOrderByImplementation(string orderBy, IEdmType elementType, IEdmNavigationSource navigationSource)
        {
            ExceptionUtils.CheckArgumentNotNull(this.configuration.Model, "model");
            ExceptionUtils.CheckArgumentNotNull(elementType, "elementType");
            ExceptionUtils.CheckArgumentNotNull(orderBy, "orderBy");

            // Get the syntactic representation of the filter expression
            UriQueryExpressionParser expressionParser = new UriQueryExpressionParser(this.Settings.OrderByLimit);
            var orderByQueryTokens = expressionParser.ParseOrderBy(orderBy);

            // Bind it to metadata
            BindingState state = new BindingState(this.configuration);
            state.ImplicitRangeVariable = NodeFactory.CreateImplicitRangeVariable(elementType.ToTypeReference(), navigationSource);
            state.RangeVariables.Push(state.ImplicitRangeVariable);
            MetadataBinder binder = new MetadataBinder(state);
            OrderByBinder orderByBinder = new OrderByBinder(binder.Bind);
            OrderByClause orderByClause = orderByBinder.BindOrderBy(state, orderByQueryTokens);

            return orderByClause;
        }

        /// <summary>
        /// Parses the <paramref name="select"/> and <paramref name="expand"/> clauses on the given <paramref name="elementType"/>, binding
        /// the text into a metadata-bound list of properties to be selected using the provided model.
        /// </summary>
        /// <param name="select">String representation of the select expression from the URI.</param>
        /// <param name="expand">String representation of the expand expression from the URI.</param>
        /// <param name="elementType">Type that the select and expand clauses are projecting.</param>
        /// <param name="navigationSource">Navigation source that the elements being filtered are from.</param>
        /// <returns>A <see cref="SelectExpandClause"/> representing the metadata bound select and expand expression.</returns>
        private SelectExpandClause ParseSelectAndExpandImplementation(string select, string expand, IEdmStructuredType elementType, IEdmNavigationSource navigationSource)
        {
            ExceptionUtils.CheckArgumentNotNull(this.configuration.Model, "model");
            ExceptionUtils.CheckArgumentNotNull(elementType, "elementType");

            ExpandToken expandTree;
            SelectToken selectTree;

            // syntactic pass
            SelectExpandSyntacticParser.Parse(select, expand, this.configuration, out expandTree, out selectTree);

            // semantic pass
            SelectExpandSemanticBinder binder = new SelectExpandSemanticBinder();
            return binder.Bind(elementType, navigationSource, expandTree, selectTree, this.configuration);
        }

        /// <summary>
        /// Parses a query count option
        /// Valid Samples: $count=true; $count=false
        /// Invalid Samples: $count=True; $count=ture
        /// </summary>
        /// <param name="count">The count string from the query</param>
        /// <returns>query count true of false</returns>
        /// <exception cref="ODataException">Throws if the input count is not a valid $count value.</exception>
        private bool ParseQueryCountImplementation(string count)
        {
            count = count.Trim();

            switch (count)
            {
                case ExpressionConstants.KeywordTrue:
                    return true;
                case ExpressionConstants.KeywordFalse:
                    return false;
                default:
                    throw new ODataException(ODataErrorStrings.ODataUriParser_InvalidCount(count));
            }
        }

        /// <summary>
        /// Parses the <paramref name="search"/> clause, binding
        /// the text into a metadata-bound list of properties to be selected using the provided model.
        /// </summary>
        /// <param name="search">String representation of the search expression from the URI.</param>
        /// <returns>A <see cref="SearchClause"/> representing the metadata bound search expression.</returns>
        private SearchClause ParseSearchImplementation(string search)
        {
            ExceptionUtils.CheckArgumentNotNull(this.configuration.Model, "model");
            ExceptionUtils.CheckArgumentNotNull(search, "searh");

            SearchParser searchParser = new SearchParser(this.Settings.SearchLimit);
            QueryToken queryToken = searchParser.ParseSearch(search);

            // Bind it to metadata
            BindingState state = new BindingState(this.configuration);
            MetadataBinder binder = new MetadataBinder(state);
            SearchBinder searchBinder = new SearchBinder(binder.Bind);
            
            return searchBinder.BindSearch(queryToken);
        }
    }
}

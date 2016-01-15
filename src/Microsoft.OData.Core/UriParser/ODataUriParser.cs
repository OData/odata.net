﻿//---------------------------------------------------------------------
// <copyright file="ODataUriParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Microsoft.OData.Core.UriParser.Extensions.Semantic;
    using Microsoft.OData.Core.UriParser.Metadata;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Main Public API to parse an ODataURI.
    /// </summary>
    public sealed class ODataUriParser
    {
        #region Fields
        /// <summary>The parser's configuration. </summary>
        private readonly ODataUriParserConfiguration configuration;

        /// <summary>
        /// Absolute URI of the service root.
        /// </summary>
        private readonly Uri serviceRoot;

        /// <summary>The full Uri to be parsed. </summary>
        private readonly Uri fullUri;

        /// <summary>Query option list</summary>
        private readonly List<CustomQueryOptionToken> queryOptions;

        /// <summary>Store query option dictionary.</summary>
        private IDictionary<string, string> queryOptionDic;

        /// <summary>Parser for query option.</summary>
        private ODataQueryOptionParser queryOptionParser;

        /// <summary>Target Edm type. </summary>
        private IEdmType targetEdmType;

        /// <summary>Target Edm navigation source. </summary>
        private IEdmNavigationSource targetNavigationSource;

        /// <summary>OData Path.</summary>
        private ODataPath odataPath;

        /// <summary>EntityId Segment.</summary>
        private EntityIdSegment entityIdSegment;
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

            this.configuration = new ODataUriParserConfiguration(model);
            this.serviceRoot = Core.UriUtils.EnsureTaillingSlash(serviceRoot);
            this.fullUri = fullUri.IsAbsoluteUri ? fullUri : Core.UriUtils.UriToAbsoluteUri(this.ServiceRoot, fullUri);
            this.queryOptions = UriUtils.ParseQueryOptions(this.fullUri);
        }

        /// <summary>
        /// Build an ODataUriParser
        /// </summary>
        /// <param name="model">Model to use for metadata binding.</param>
        /// <param name="fullUri">full Uri to be parsed, it should be a relative Uri.</param>
        public ODataUriParser(IEdmModel model, Uri fullUri)
        {
            ExceptionUtils.CheckArgumentNotNull(fullUri, "fullUri");

            if (fullUri.IsAbsoluteUri)
            {
                throw new ODataException(Strings.UriParser_FullUriMustBeRelative);
            }

            this.configuration = new ODataUriParserConfiguration(model);
            this.fullUri = fullUri;
            this.queryOptions = UriUtils.ParseQueryOptions(UriUtils.CreateMockAbsoluteUri(this.fullUri));
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
            get { return this.serviceRoot; }
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
        /// Whether Uri template parsing is enabled. Uri template for keys and function parameters are supported.
        /// See <see cref="UriTemplateExpression"/> class for detail.
        /// </summary>
        public bool EnableUriTemplateParsing
        {
            get { return this.configuration.EnableUriTemplateParsing; }
            set { this.configuration.EnableUriTemplateParsing = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="ODataUriResolver"/> for <see cref="ODataUriParser"/>.
        /// </summary>
        public ODataUriResolver Resolver
        {
            get { return this.configuration.Resolver; }
            set { this.configuration.Resolver = value; }
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
                    this.Initialize();
                }

                return this.ParameterAliasValueAccessor.ParameterAliasValueNodesCached;
            }
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
            this.Initialize();
            return queryOptionParser.ParseFilter();
        }

        /// <summary>
        /// Parses a orderBy clause on the given full Uri, binding
        /// the text into semantic nodes using the constructed mode.
        /// </summary>
        /// <returns>A <see cref="OrderByClause"/> representing the metadata bound orderby expression.</returns>
        public OrderByClause ParseOrderBy()
        {
            this.Initialize();
            return this.queryOptionParser.ParseOrderBy();
        }

        /// <summary>
        /// ParseSelectAndExpand from an instantiated class
        /// </summary>
        /// <returns>A SelectExpandClause with the semantic representation of select and expand terms</returns>
        public SelectExpandClause ParseSelectAndExpand()
        {
            this.Initialize();
            return this.queryOptionParser.ParseSelectAndExpand();
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

            InitQueryOptionDic();
            string idQuery = null;

            if (!this.Resolver.EnableCaseInsensitive)
            {
                if (!this.queryOptionDic.TryGetValue(UriQueryConstants.IdQueryOption, out idQuery))
                {
                    return null;
                }
            }
            else
            {
                var list = this.queryOptionDic
                    .Where(pair => string.Equals(UriQueryConstants.IdQueryOption, pair.Key, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (list.Count == 0)
                {
                    return null;
                }
                else if (list.Count == 1)
                {
                    idQuery = list.First().Value;
                }
                else
                {
                    throw new ODataException(Strings.QueryOptionUtils_QueryParameterMustBeSpecifiedOnce(UriQueryConstants.IdQueryOption));
                }
            }

            Uri idUri = new Uri(idQuery, UriKind.RelativeOrAbsolute);

            if (!idUri.IsAbsoluteUri)
            {
                if (!this.fullUri.IsAbsoluteUri)
                {
                    Uri baseUri = UriUtils.CreateMockAbsoluteUri();
                    Uri c = new Uri(UriUtils.CreateMockAbsoluteUri(this.fullUri), idUri);
                    idUri = baseUri.MakeRelativeUri(c);
                }
                else
                {
                    idUri = new Uri(this.fullUri, idUri);
                }
            }

            this.entityIdSegment = new EntityIdSegment(idUri);
            return this.entityIdSegment;
        }

        /// <summary>
        /// Parses a $top query option
        /// </summary>
        /// <returns>A value representing that top option, null if $top query does not exist.</returns>
        public long? ParseTop()
        {
            this.Initialize();
            return this.queryOptionParser.ParseTop();
        }

        /// <summary>
        /// Parses a $skip query option
        /// </summary>
        /// <returns>A value representing that skip option, null if $skip query does not exist.</returns>
        public long? ParseSkip()
        {
            this.Initialize();
            return this.queryOptionParser.ParseSkip();
        }

        /// <summary>
        /// Parses a $count query option
        /// </summary>
        /// <returns>An count representing that count option, null if $count query does not exist.</returns>
        public bool? ParseCount()
        {
            this.Initialize();
            return this.queryOptionParser.ParseCount();
        }

        /// <summary>
        /// Parses the $search.
        /// </summary>
        /// <returns>SearchClause representing $search.</returns>
        public SearchClause ParseSearch()
        {
            this.Initialize();
            return this.queryOptionParser.ParseSearch();
        }

        /// <summary>
        /// Parses the $apply.
        /// </summary>
        /// <returns>ApplyClause representing $apply.</returns>
        public ApplyClause ParseApply()
        {
            this.Initialize();
            return this.queryOptionParser.ParseApply();
        }

        /// <summary>
        /// Parses a $skiptoken query option
        /// </summary>
        /// <returns>A value representing that skip token option, null if $skiptoken query does not exist.</returns>
        public string ParseSkipToken()
        {
            this.Initialize();
            return this.queryOptionParser.ParseSkipToken();
        }

        /// <summary>
        /// Parses a $deltatoken query option
        /// </summary>
        /// <returns>A value representing that delta token option, null if $deltatoken query does not exist.</returns>
        public string ParseDeltaToken()
        {
            this.Initialize();
            return this.queryOptionParser.ParseDeltaToken();
        }

        /// <summary>
        /// Parse a full Uri into its contingent parts with semantic meaning attached to each part. 
        /// See <see cref="ODataUri"/>.
        /// </summary>
        /// <returns>An <see cref="ODataUri"/> representing the full uri.</returns>
        public ODataUri ParseUri()
        {
            ExceptionUtils.CheckArgumentNotNull(this.configuration.Model, "model");
            ExceptionUtils.CheckArgumentNotNull(this.fullUri, "fullUri");

            ODataPath path = this.ParsePath();
            SelectExpandClause selectExpand = this.ParseSelectAndExpand();
            FilterClause filter = this.ParseFilter();
            OrderByClause orderBy = this.ParseOrderBy();
            SearchClause search = this.ParseSearch();
            ApplyClause apply = this.ParseApply();
            long? top = this.ParseTop();
            long? skip = this.ParseSkip();
            bool? count = this.ParseCount();
            string skipToken = this.ParseSkipToken();
            string deltaToken = this.ParseDeltaToken();

            // TODO:  check it shouldn't be empty
            List<QueryNode> boundQueryOptions = new List<QueryNode>();

            ODataUri odataUri = new ODataUri(this.ParameterAliasValueAccessor, path, boundQueryOptions, selectExpand, filter, orderBy, search, apply, skip, top, count);
            odataUri.ServiceRoot = this.serviceRoot;
            odataUri.SkipToken = skipToken;
            odataUri.DeltaToken = deltaToken;
            return odataUri;
        }

        /// <summary>
        /// Parses a the fullUri into a semantic <see cref="ODataPath"/> object. 
        /// </summary>
        /// <remarks>
        /// This is designed to parse the Path of a URL. If it is used to parse paths that are contained
        /// within other places, such as $filter expressions, then it may not enforce correct rules.
        /// </remarks>
        /// <returns>An <see cref="ODataPath"/> representing the metadata-bound path expression.</returns>
        /// <exception cref="ODataException">Throws if the input path is not an absolute uri.</exception>
        private ODataPath ParsePathImplementation()
        {
            Uri pathUri = this.fullUri;
            ExceptionUtils.CheckArgumentNotNull(pathUri, "pathUri");

            UriPathParser pathParser = new UriPathParser(this.Settings.PathLimit);
            var rawSegments = pathParser.ParsePathIntoSegments(pathUri, this.ServiceRoot);
            return ODataPathFactory.BindPath(rawSegments, this.configuration);
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

            // When parse path failed first time, the alias node would be not null, but already removed from query options, so do not set it in this case.
            if (this.ParameterAliasValueAccessor == null)
            {
                this.ParameterAliasValueAccessor = new ParameterAliasValueAccessor(queryOptions.GetParameterAliases());
            }

            this.odataPath = this.ParsePathImplementation();
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
                    IEdmCollectionType collectionType = this.targetEdmType as IEdmCollectionType;
                    if (collectionType != null)
                    {
                        this.targetEdmType = collectionType.ElementType.Definition;
                    }
                }
            }

            InitQueryOptionDic();

            this.queryOptionParser = new ODataQueryOptionParser(this.Model, this.targetEdmType, this.targetNavigationSource, queryOptionDic)
                                        {
                                            Configuration = this.configuration
                                        };
        }

        /// <summary>
        /// Resolve query options to dictionary.
        /// </summary>
        private void InitQueryOptionDic()
        {
            if (queryOptionDic != null)
            {
                return;
            }

            queryOptionDic = new Dictionary<string, string>(StringComparer.Ordinal);

            if (queryOptions != null)
            {
                foreach (var queryOption in queryOptions)
                {
                    if (queryOption.Name == null)
                    {
                        continue;
                    }

                    if (queryOptionDic.ContainsKey(queryOption.Name))
                    {
                        throw new ODataException(Strings.QueryOptionUtils_QueryParameterMustBeSpecifiedOnce(queryOption.Name));
                    }

                    queryOptionDic.Add(queryOption.Name, queryOption.Value);
                }
            }
        }
    }
}

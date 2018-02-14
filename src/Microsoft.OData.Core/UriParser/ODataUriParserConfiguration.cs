﻿//---------------------------------------------------------------------
// <copyright file="ODataUriParserConfiguration.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Internal class for storing all the configuration information about the URI parser. Allows us to flow these values around without passing an actual parser.
    /// </summary>
    internal sealed class ODataUriParserConfiguration
    {
        /// <summary>The conventions to use when parsing URLs.</summary>
        private ODataUrlKeyDelimiter urlKeyDelimiter;

        /// <summary>The resolver to use when parsing URLs.</summary>
        private ODataUriResolver uriResolver;

        /// <summary>
        /// Initializes a new instance of <see cref="ODataUriParserConfiguration"/>.
        /// </summary>
        /// <param name="model">Model to use for metadata binding.</param>
        /// <param name="container">The optional dependency injection container to get related services for URI parsing.</param>
        /// <exception cref="System.ArgumentNullException">Throws if input model is null.</exception>
        /// <exception cref="ArgumentException">Throws if the input serviceRoot is not an AbsoluteUri</exception>
        public ODataUriParserConfiguration(IEdmModel model, IServiceProvider container)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            this.Model = model;
            this.Container = container;
            this.Resolver = ODataUriResolver.GetUriResolver(container);
            this.urlKeyDelimiter = ODataUrlKeyDelimiter.GetODataUrlKeyDelimiter(container);

            if (this.Container == null)
            {
                this.Settings = new ODataUriParserSettings();
            }
            else
            {
                this.Settings = this.Container.GetRequiredService<ODataUriParserSettings>();
            }

            this.EnableUriTemplateParsing = false;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ODataUriParserConfiguration"/>.
        /// </summary>
        /// <param name="model">Model to use for metadata binding.</param>
        /// <exception cref="System.ArgumentNullException">Throws if input model is null.</exception>
        /// <exception cref="ArgumentException">Throws if the input serviceRoot is not an AbsoluteUri</exception>
        internal ODataUriParserConfiguration(IEdmModel model)
            : this(model, null)
        {
        }

        /// <summary>
        /// The settings for this instance of <see cref="ODataUriParser"/>. Refer to the documentation for the individual properties of <see cref="ODataUriParserSettings"/> for more information.
        /// </summary>
        public ODataUriParserSettings Settings { get; private set; }

        /// <summary>
        /// Gets the model for this ODataUriParser
        /// </summary>
        public IEdmModel Model { get; private set; }

        /// <summary>
        /// The optional dependency injection container to get related services for URI parsing.
        /// </summary>
        public IServiceProvider Container { get; private set; }

        /// <summary>
        /// Gets or Sets the <see cref="ODataUrlKeyDelimiter"/> to use while parsing, specifically
        /// whether to recognize keys as segments or not.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Throws if the input value is null.</exception>
        public ODataUrlKeyDelimiter UrlKeyDelimiter
        {
            get
            {
                return this.urlKeyDelimiter;
            }

            set
            {
                ExceptionUtils.CheckArgumentNotNull(value, "UrlKeyDelimiter");
                this.urlKeyDelimiter = value;
            }
        }

        /// <summary>
        /// Gets or Sets a callback that returns a BatchReferenceSegment (to be used for $0 in batch)
        /// </summary>
        public Func<string, BatchReferenceSegment> BatchReferenceCallback { get; set; }

        /// <summary>
        /// Gets or sets the function which can be used to parse an unknown path segment or an open property segment.
        /// </summary>
        public ParseDynamicPathSegment ParseDynamicPathSegmentFunc { get; set; }

        /// <summary>
        /// Whether to allow case insensitive for builtin identifier.
        /// </summary>
        internal bool EnableCaseInsensitiveUriFunctionIdentifier
        {
            get
            {
                return Resolver.EnableCaseInsensitive;
            }

            set
            {
                Resolver.EnableCaseInsensitive = value;
            }
        }

        /// <summary>
        /// Gets or Sets an option whether no dollar query options is enabled.
        /// If it is enabled, the '$' prefix of system query options becomes optional.
        /// For example, "select" and "$select" are equivalent in this case.
        /// </summary>
        internal bool EnableNoDollarQueryOptions
        {
            get { return this.Resolver.EnableNoDollarQueryOptions; }
            set { this.Resolver.EnableNoDollarQueryOptions = value; }
        }

        /// <summary>
        /// Whether Uri template parsing is enabled. See <see cref="UriTemplateExpression"/> class for detail.
        /// </summary>
        internal bool EnableUriTemplateParsing { get; set; }

        /// <summary>
        /// Gets or sets the parameter aliases info for MetadataBinder to resolve parameter alias' metadata type.
        /// </summary>
        internal ParameterAliasValueAccessor ParameterAliasValueAccessor { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ODataUriResolver"/>.
        /// </summary>
        internal ODataUriResolver Resolver
        {
            get
            {
                return this.uriResolver;
            }

            set
            {
                ExceptionUtils.CheckArgumentNotNull(value, "Resolver");
                this.uriResolver = value;
            }
        }
    }
}
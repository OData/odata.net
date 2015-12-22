//---------------------------------------------------------------------
// <copyright file="ODataUriParserConfiguration.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser
{
    using System;
    using Microsoft.OData.Core.UriParser.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Internal class for storing all the configuration information about the URI parser. Allows us to flow these values around without passing an actual parser.
    /// </summary>
    internal sealed class ODataUriParserConfiguration
    {
        /// <summary>
        /// Model to use for metadata binding.
        /// </summary>
        private readonly IEdmModel model;

        /// <summary>The conventions to use when parsing URLs.</summary>
        private ODataUrlConventions urlConventions = ODataUrlConventions.Default;

        /// <summary>The resolver to use when parsing URLs.</summary>
        private ODataUriResolver uriResolver = new ODataUriResolver();

        /// <summary>
        /// Initializes a new instance of <see cref="ODataUriParserConfiguration"/>.
        /// </summary>
        /// <param name="model">Model to use for metadata binding.</param>
        /// <exception cref="System.ArgumentNullException">Throws if input model is null.</exception>
        /// <exception cref="ArgumentException">Throws if the input serviceRoot is not an AbsoluteUri</exception>
        public ODataUriParserConfiguration(IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            this.model = model;
            this.Settings = new ODataUriParserSettings();
            this.EnableUriTemplateParsing = false;
            this.EnableCaseInsensitiveUriFunctionIdentifier = false;
        }

        /// <summary>
        /// The settings for this instance of <see cref="ODataUriParser"/>. Refer to the documentation for the individual properties of <see cref="ODataUriParserSettings"/> for more information.
        /// </summary>
        public ODataUriParserSettings Settings { get; private set; }

        /// <summary>
        /// Gets the model for this ODataUriParser
        /// </summary>
        public IEdmModel Model
        {
            get { return this.model; }
        }

        /// <summary>
        /// Gets or Sets the <see cref="ODataUrlConventions"/> to use while parsing, specifically
        /// whether to recognize keys as segments or not.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Throws if the input value is null.</exception>
        public ODataUrlConventions UrlConventions
        {
            get
            {
                return this.urlConventions;
            }

            set
            {
                ExceptionUtils.CheckArgumentNotNull(value, "UrlConventions");
                this.urlConventions = value;
            }
        }

        /// <summary>
        /// Gets or Sets a callback that returns a BatchReferenceSegment (to be used for $0 in batch)
        /// </summary>
        public Func<string, BatchReferenceSegment> BatchReferenceCallback { get; set; }

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
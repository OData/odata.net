//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData.Query
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.Query.SemanticAst;
    using Microsoft.Data.OData.Query.SyntacticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

    /// <summary>
    /// Internal class for storing all the configuration information about the URI parser. Allows us to flow these values around without passing an actual parser.
    /// </summary>
    internal sealed class ODataUriParserConfiguration
    {
        /// <summary>
        /// Model to use for metadata binding.
        /// </summary>
        private readonly IEdmModel model;

        /// <summary>
        /// Absolute URI of the service root.
        /// </summary>
        private readonly Uri serviceRoot;

        /// <summary>The conventions to use when parsing URLs.</summary>
        private ODataUrlConventions urlConventions = ODataUrlConventions.Default;

        /// <summary>
        /// Initializes a new instance of <see cref="ODataUriParserConfiguration"/>.
        /// </summary>
        /// <param name="model">Model to use for metadata binding.</param>
        /// <param name="serviceRoot">Absolute URI of the service root.</param>
        /// <exception cref="System.ArgumentNullException">Throws if input model is null.</exception>
        /// <exception cref="ArgumentException">Throws if the input serviceRoot is not an AbsoluteUri</exception>
        public ODataUriParserConfiguration(IEdmModel model, Uri serviceRoot)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            if (serviceRoot != null)
            {
                if (!serviceRoot.IsAbsoluteUri)
                {
                    throw new ArgumentException(ODataErrorStrings.UriParser_UriMustBeAbsolute(serviceRoot), "serviceRoot");
                }
            }

            this.model = model;
            this.serviceRoot = serviceRoot;
            this.Settings = new ODataUriParserSettings();
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
        /// Gets or sets a callback that returns the raw string value for an aliased function parameter.
        /// </summary>
        public Func<string, string> FunctionParameterAliasCallback { get; set; }
    }
}

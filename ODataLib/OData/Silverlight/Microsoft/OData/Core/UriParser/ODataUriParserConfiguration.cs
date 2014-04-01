//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

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

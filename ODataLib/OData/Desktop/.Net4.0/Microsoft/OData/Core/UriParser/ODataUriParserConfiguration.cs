//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser
{
    using System;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.UriParser.Semantic;

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
        /// Whether Uri template parsing is enabled. See <see cref="UriTemplateExpression"/> class for detail.
        /// </summary>
        internal bool EnableUriTemplateParsing { get; set; }

        /// <summary>
        /// Gets or sets the parameter aliases info for MetadataBinder to resolve parameter alias' metadata type.
        /// </summary>
        internal ParameterAliasValueAccessor ParameterAliasValueAccessor { get; set; }
    }
}

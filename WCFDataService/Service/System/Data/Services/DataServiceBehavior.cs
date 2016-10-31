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

namespace System.Data.Services
{
    using System.Data.Services.Common;
    using System.Data.Services.Configuration;
    using System.Diagnostics;

    /// <summary>Use this class to add settings that define service behavior.</summary>
    public sealed class DataServiceBehavior
    {
        /// <summary>The conventions to use when generating and parsing URLs.</summary>
        private DataServiceUrlConventions urlConventions;

        /// <summary>
        /// Initializes a new <see cref="DataServiceBehavior"/>.
        /// </summary>
        internal DataServiceBehavior()
        {
            this.InvokeInterceptorsOnLinkDelete = true;
            this.AcceptCountRequests = true;
            this.AcceptProjectionRequests = true;
            this.AcceptAnyAllRequests = true;
            this.MaxProtocolVersion = DataServiceProtocolVersion.V1;
            this.IncludeAssociationLinksInResponse = false;
            this.UseMetadataKeyOrderForBuiltInProviders = false;
            this.AcceptSpatialLiteralsInQuery = true;
            this.UrlConventions = DataServiceUrlConventions.Default;
        }

        /// <summary>Gets or sets whether to invoke change interceptors when a link is deleted.</summary>
        /// <returns>True when interceptors should be invoked; otherwise false. </returns>
        public bool InvokeInterceptorsOnLinkDelete
        {
            get;
            set;
        }

        /// <summary>Gets or sets whether requests with the $count path segment or the $inlinecount query options are accepted.</summary>
        /// <returns>True if count requests are supported; otherwise false.</returns>
        public bool AcceptCountRequests
        {
            get;
            set;
        }

        /// <summary>Gets or sets whether projection requests should be accepted.</summary>
        /// <returns>True if projection requests are supported; otherwise false.</returns>
        public bool AcceptProjectionRequests
        {
            get;
            set;
        }

        /// <summary>Gets or sets whether the server will accept requests with filters that contain all or any expressions.</summary>
        /// <returns>True when the server accepts all or any expressions; otherwise false.</returns>
        public bool AcceptAnyAllRequests
        {
            get;
            set;
        }

        /// <summary>Gets or sets the maximum protocol version that is supported by the response sent by the data service.</summary>
        /// <returns>The maximum version allowed in the response.</returns>
        public DataServiceProtocolVersion MaxProtocolVersion
        {
            get;
            set;
        }

        /// <summary>Get or sets whether relationship links are included in responses from the data service.</summary>
        /// <returns>True when relationship links are returned; otherwise false. </returns>
        public bool IncludeAssociationLinksInResponse 
        { 
            get; 
            set; 
        }

        /// <summary>Get or sets whether to use the order of key properties as defined in the metadata of an Entity Framework or reflection provider when constructing an implicit OrderBy query.</summary>
        /// <returns>True when the order of key properties is inferred from the provider metadata and false when an alphabetical order is used.</returns>
        public bool UseMetadataKeyOrderForBuiltInProviders
        {
            get;
            set;
        }

        /// <summary>Gets or sets whether spatial literal values are supported in the URI.</summary>
        /// <returns>True when spatial literals are supported in the URI; otherwise false.</returns>
        public bool AcceptSpatialLiteralsInQuery
        {
            get;
            set;
        }

        /// <summary>
        /// Allow replace functions in the request url.
        /// </summary>
        public bool AcceptReplaceFunctionInQuery
        {
            get;
            set;
        }

        /// <summary>
        /// The conventions to use when generating and parsing URLs.
        /// </summary>
        public DataServiceUrlConventions UrlConventions
        {
            get
            {
                return this.urlConventions;
            }

            set
            {
                WebUtil.CheckArgumentNull(value, "UrlConventions");
                this.urlConventions = value;
            }
        }

        /// <summary>
        /// If set to true, then the root element of each payload will be written in the default (non-prefix-qualified) namespace of the document. 
        /// All other elements in the same namespace will also not have prefixes.
        /// </summary>
        public bool AlwaysUseDefaultXmlNamespaceForRootElement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether to generate keys as segments based on the user-specified URL conventions.
        /// </summary>
        internal bool GenerateKeyAsSegment
        {
            get { return this.urlConventions == DataServiceUrlConventions.KeyAsSegment; }
        }

        /// <summary>
        /// Return false for V1/V2 servers because we should ignore the IncludeAssociationLinksInResponse settings for V1/V2 servers.
        /// Otherwise, return whatever value has been set for the knob.
        /// </summary>
        internal bool ShouldIncludeAssociationLinksInResponse
        {
            get { return this.IncludeAssociationLinksInResponse && this.MaxProtocolVersion >= DataServiceProtocolVersion.V3; }
        }

        /// <summary>
        /// Applies the settings from the configuration file.
        /// </summary>
        /// <param name="featuresSection">The features section from the configuration file.</param>
        internal void ApplySettingsFromConfiguration(DataServicesFeaturesSection featuresSection)
        {
            if (featuresSection != null)
            {
                // If the element is specified in the config section, then value in the config
                // wins over the api one. Hence overriding the api value if the value is specified
                // in the config file.
                if (featuresSection.ReplaceFunction.IsPresent)
                {
                    this.AcceptReplaceFunctionInQuery = featuresSection.ReplaceFunction.Enable;
                }
            }
        }
    }
}

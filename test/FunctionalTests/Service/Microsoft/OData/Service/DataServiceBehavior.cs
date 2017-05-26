//---------------------------------------------------------------------
// <copyright file="DataServiceBehavior.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using Microsoft.OData.Client;
    using Microsoft.OData.Service.Configuration;

    /// <summary>Use this class to add settings that define service behavior.</summary>
    public sealed class DataServiceBehavior
    {
        /// <summary>The key delimiter to use when generating and parsing URLs.</summary>
        private DataServiceUrlKeyDelimiter urlKeyDelimiter;

        /// <summary>
        /// Initializes a new <see cref="DataServiceBehavior"/>.
        /// </summary>
        internal DataServiceBehavior()
        {
            this.InvokeInterceptorsOnLinkDelete = true;
            this.AcceptCountRequests = true;
            this.AcceptProjectionRequests = true;
            this.AcceptAnyAllRequests = true;
            this.MaxProtocolVersion = ODataProtocolVersion.V4;
            this.IncludeAssociationLinksInResponse = false;
            this.UseMetadataKeyOrderForBuiltInProviders = false;
            this.AcceptSpatialLiteralsInQuery = true;
            this.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        }

        /// <summary>Gets or sets whether to invoke change interceptors when a link is deleted.</summary>
        /// <returns>True when interceptors should be invoked; otherwise false. </returns>
        public bool InvokeInterceptorsOnLinkDelete
        {
            get;
            set;
        }

        /// <summary>Gets or sets whether requests with the $count path segment or the $count query options are accepted.</summary>
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
        public ODataProtocolVersion MaxProtocolVersion
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
        /// The key delimiter to use when generating and parsing URLs.
        /// </summary>
        public DataServiceUrlKeyDelimiter UrlKeyDelimiter
        {
            get
            {
                return this.urlKeyDelimiter;
            }

            set
            {
                WebUtil.CheckArgumentNull(value, "UrlKeyDelimiter");
                this.urlKeyDelimiter = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to generate keys as segments based on the user-specified URL conventions.
        /// </summary>
        internal bool GenerateKeyAsSegment
        {
            get { return this.urlKeyDelimiter == DataServiceUrlKeyDelimiter.Slash; }
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

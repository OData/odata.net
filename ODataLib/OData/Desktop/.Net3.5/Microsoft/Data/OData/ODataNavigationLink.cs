//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.Data.OData.Evaluation;
    #endregion Namespaces

    /// <summary>
    /// Represents a single link.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public sealed class ODataNavigationLink : ODataItem
    {
        /// <summary>the metadata builder for this navigation link.</summary>
        private ODataEntityMetadataBuilder metadataBuilder;

        /// <summary>URI representing the Unified Resource Locator (Url) of the link as provided by the user or seen on the wire (never computed).</summary>
        private Uri url;

        /// <summary>true if the navigation link has been set by the user or seen on the wire or computed by the metadata builder, false otherwise.</summary>
        private bool hasNavigationLink;

        /// <summary>The association link URL for this navigation link as provided by the user or seen on the wire (never computed).</summary>
        private Uri associationLinkUrl;

        /// <summary>true if the association link has been set by the user or seen on the wire or computed by the metadata builder, false otherwise.</summary>
        private bool hasAssociationUrl;

        /// <summary>Gets or sets a value that indicates whether the navigation link represents a collection or an entry.</summary>
        /// <returns>true if the navigation link represents a collection; false if the navigation represents an entry.</returns>
        /// <remarks>This property is required to have a value for ATOM payloads and is optional for JSON payloads.</remarks>
        public bool? IsCollection
        {
            get;
            set;
        }

        /// <summary>Gets or sets the name of the link.</summary>
        /// <returns>The name of the link.</returns>
        public string Name 
        { 
            get; 
            set; 
        }

        /// <summary>Gets or sets the URI representing the Unified Resource Locator (URL) of the link.</summary>
        /// <returns>The URI representing the Unified Resource Locator (URL) of the link.</returns>
        public Uri Url
        {
            get
            {
                if (this.metadataBuilder != null)
                {
                    this.url = this.metadataBuilder.GetNavigationLinkUri(this.Name, this.url, this.hasNavigationLink);
                    this.hasNavigationLink = true;
                }

                return this.url;
            }

            set
            {
                this.url = value;
                this.hasNavigationLink = true;
            }
        }

        /// <summary>The association link URL for this navigation link. </summary>
        public Uri AssociationLinkUrl
        {
            get
            {
                if (this.metadataBuilder != null)
                {
                    this.associationLinkUrl = this.metadataBuilder.GetAssociationLinkUri(this.Name, this.associationLinkUrl, this.hasAssociationUrl);
                    this.hasAssociationUrl = true;
                }

                return this.associationLinkUrl;
            }

            set
            {
                this.associationLinkUrl = value;
                this.hasAssociationUrl = true;
            }
        }

        /// <summary>
        /// Sets the metadata builder for this navigation link.
        /// </summary>
        /// <param name="builder">The metadata builder used to compute values from model annotations.</param>
        internal void SetMetadataBuilder(ODataEntityMetadataBuilder builder)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(builder != null, "builder != null");

            this.metadataBuilder = builder;
        }
    }
}

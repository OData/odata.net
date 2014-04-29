//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Core.Evaluation;
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

        /// <summary>Gets or sets the context url for this navigation link.</summary>
        /// <returns>The URI representing the context url of the navigation link.</returns>
        internal Uri ContextUrl { get; set; }

        /// <summary>Gets or sets metadata builder for this navigation link.</summary>
        /// <returns>The metadata builder used to compute values from model annotations.</returns>
        internal ODataEntityMetadataBuilder MetadataBuilder
        {
            get
            {
                return this.metadataBuilder;
            }

            set
            {
                Debug.Assert(value != null, "MetadataBuilder != null");

                this.metadataBuilder = value;
            }
        }
    }
}

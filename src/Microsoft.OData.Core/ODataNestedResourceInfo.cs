//---------------------------------------------------------------------
// <copyright file="ODataNestedResourceInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Evaluation;
    #endregion Namespaces

    /// <summary>
    /// Represents a single link.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public sealed class ODataNestedResourceInfo : ODataItem
    {
        /// <summary>the metadata builder for this nested resource info.</summary>
        private ODataResourceMetadataBuilder metadataBuilder;

        /// <summary>URI representing the Unified Resource Locator (Url) of the link as provided by the user or seen on the wire (never computed).</summary>
        private Uri url;

        /// <summary>true if the navigation link has been set by the user or seen on the wire or computed by the metadata builder, false otherwise.</summary>
        private bool hasNavigationLink;

        /// <summary>The association link URL for this navigation link as provided by the user or seen on the wire (never computed).</summary>
        private Uri associationLinkUrl;

        /// <summary>true if the association link has been set by the user or seen on the wire or computed by the metadata builder, false otherwise.</summary>
        private bool hasAssociationUrl;

        /// <summary>Gets or sets a value that indicates whether the nested resource info represents a collection or a resource.</summary>
        /// <returns>true if the nested resource info represents a collection; false if the navigation represents a resource.</returns>
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

        /// <summary>Gets or sets the number of items for this nested resource info.
        /// Be noted, this count property is for nested resource info without content.
        /// For nested resource info with content, please specify the count on ODataResourceSetBase.Count.
        /// </summary>
        /// <returns>The number of items in the resource set.</returns>
        public long? Count
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
                if (this.metadataBuilder != null && !this.IsComplex)
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
                if (this.metadataBuilder != null && !this.IsComplex)
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

        /// <summary>Gets or sets the context url for this nested resource info.</summary>
        /// <returns>The URI representing the context url of the nested resource info.</returns>
        internal Uri ContextUrl { get; set; }

        /// <summary>Gets or sets metadata builder for this nested resource info.</summary>
        /// <returns>The metadata builder used to compute values from model annotations.</returns>
        internal ODataResourceMetadataBuilder MetadataBuilder
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

        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataWriter"/> for this <see cref="ODataNestedResourceInfo"/>.
        /// </summary>
        internal ODataNestedResourceInfoSerializationInfo SerializationInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Whether this is a complex property.
        /// </summary>
        internal bool IsComplex { get; set; }
    }
}

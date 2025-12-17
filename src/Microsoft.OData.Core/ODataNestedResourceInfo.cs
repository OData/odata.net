//---------------------------------------------------------------------
// <copyright file="ODataNestedResourceInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using Microsoft.OData.Evaluation;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
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

        /// <summary>
        /// Collection of custom instance annotations.
        /// Reading process:
        /// A single-valued nested resource could be one of the following three scenarios: Be noted:
        ///  # 1. a resource with a valid json object
        ///  {
        ///      "MyNavProp@ns.annotation": "value",
        ///      "MyNavProp": {...}
        ///  }
        ///  
        ///  In this case, the instance annotations within resource are attached to the ODataResource represented by "MyNavProp".
        ///  meanwhile, the instance annotations as property annotations are attached to the ODataNestedResourceInfo representing "MyNavProp"?
        ///  # 2. a resource with 'null' value
        ///  {
        ///      "MyNavProp@ns.annotation": "value",
        ///      "MyNavProp": null
        ///  }
        ///  In this case, the instance annotations as property annotations are attached to the ODataNestedResourceInfo representing "MyNavProp".
        ///  
        ///  # 3. a resource without value
        ///  {
        ///      "MyNavProp@ns.annotation": "value",
        ///  }
        ///  In this case, the instance annotations as property annotations are attached to the ODataPropertyInfo representing "MyNavProp".
        ///  In reading process, ODataReaderState.NestedProperty is popup.
        ///  
        /// For collection-valued nested resource, the instance annotations are always property annotations, for example:
        /// {
        ///       "MyCollectionNavProp@ns.annotation": "value",
        /// }
        /// The value of collection-valued could be: Empty collection "[]", or collection with items "[{...}, x, {...}]", or without content.
        /// 
        /// Writing process:
        /// Following up OData spec, When annotating a name/value pair for which the value is represented as a JSON object, each annotation is placed within the object and represented as a single name/value pair.
        /// So,dDuring writing process, the instance annotations attached to ODataNestedResourceInfo will be written as property annotations only if the associated nested resource value is null.
        /// That means the instance annotations attached to ODataNestedResourceInfo will be ignored if the associated nested resource value is a JSON object.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "We want to allow the same instance annotation collection instance to be shared across ODataLib OM instances.")]
        public ICollection<ODataInstanceAnnotation> InstanceAnnotations
        {
            get { return this.GetInstanceAnnotations(); }
            set { this.SetInstanceAnnotations(value); }
        }
    }
}

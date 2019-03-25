//---------------------------------------------------------------------
// <copyright file="ODataStreamPropertyInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Evaluation;

    /// <summary>
    /// Represents information about a single stream property of a resource.
    /// </summary>
    public sealed class ODataStreamPropertyInfo : ODataPropertyInfo, IODataStreamReferenceInfo
    {
        /// <summary>The name of the named stream this value belongs to; null for the default media resource.</summary>
        private string edmPropertyName;

        /// <summary>the metadata builder for this OData resource.</summary>
        private ODataResourceMetadataBuilder metadataBuilder;

        /// <summary>Edit link for media resource.</summary>
        private Uri editLink;

        /// <summary>Edit link for media resource.</summary>
        private Uri computedEditLink;

        /// <summary>Read link for media resource.</summary>
        private Uri readLink;

        /// <summary>Read link for media resource.</summary>
        private Uri computedReadLink;

        /// <summary>PrimitiveTypeKind of the media resource.</summary>
        private EdmPrimitiveTypeKind primitiveTypeKind;

        /// <summary>Gets or sets the edit link for media resource.</summary>
        /// <returns>The edit link for media resource.</returns>
        public Uri EditLink
        {
            get
            {
                return this.HasNonComputedEditLink
                    ? this.editLink
                    : (this.computedEditLink ?? (this.metadataBuilder == null ? null : this.computedEditLink = this.metadataBuilder.GetStreamEditLink(edmPropertyName)));
            }

            set
            {
                this.editLink = value;
                this.HasNonComputedEditLink = true;
            }
        }

        /// <summary>Gets or sets the read link for media resource.</summary>
        /// <returns>The read link for media resource.</returns>
        public Uri ReadLink
        {
            get
            {
                return this.HasNonComputedReadLink
                    ? this.readLink
                    : (this.computedReadLink ?? (this.metadataBuilder == null ? null :
                        this.computedReadLink = this.metadataBuilder.GetStreamReadLink(edmPropertyName)));
            }

            set
            {
                this.readLink = value;
                this.HasNonComputedReadLink = true;
            }
        }

        /// <summary>Gets or sets the content media type.</summary>
        /// <returns>The content media type.</returns>
        public string ContentType
        {
            get; set;
        }

        /// <summary>Gets or sets the media resource ETag.</summary>
        /// <returns>The media resource ETag.</returns>
        public string ETag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the kind of primitive type of the property.
        /// The PrimitiveTypeKind of an ODataStreamPropertyInfo must be EdmPrimitiveTypeKind.String, EdmPrimitiveTypeKind.Binary, or EdmPrimitiveTypeKind.None.
        /// </summary>
        /// <returns>The <see cref="EdmPrimitiveTypeKind"/> of the property.</returns>
        public override EdmPrimitiveTypeKind PrimitiveTypeKind
        {
            get
            {
                return this.primitiveTypeKind;
            }

            set
            {
                if (value != EdmPrimitiveTypeKind.Binary &&
                    value != EdmPrimitiveTypeKind.String &&
                    value != EdmPrimitiveTypeKind.None)
                {
                    throw new ODataException(Strings.StreamItemInvalidPrimitiveKind(value));
                }

                this.primitiveTypeKind = value;
            }
        }

        /// <summary>
        /// true if an edit link was provided by the user or seen on the wire, false otherwise.
        /// </summary>
        internal bool HasNonComputedEditLink
        {
            get;
            private set;
        }

        /// <summary>
        /// true if a read link was provided by the user or seen on the wire, false otherwise.
        /// </summary>
        internal bool HasNonComputedReadLink
        {
            get;
            private set;
        }

        /// <summary>
        /// Sets the metadata builder for this stream reference value.
        /// </summary>
        /// <param name="builder">The metadata builder used to compute values from model annotations.</param>
        /// <param name="propertyName">The property name for the named stream; null for the default media resource.</param>
        internal void SetMetadataBuilder(ODataResourceMetadataBuilder builder, string propertyName)
        {
            this.metadataBuilder = builder;
            this.edmPropertyName = propertyName;
            this.computedEditLink = null;
            this.computedReadLink = null;
        }

        /// <summary>
        /// Gets the metadata builder for this stream reference value.
        /// </summary>
        /// <returns>The metadata builder used to compute links.</returns>
        internal ODataResourceMetadataBuilder GetMetadataBuilder()
        {
            return this.metadataBuilder;
        }
    }
}
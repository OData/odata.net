//---------------------------------------------------------------------
// <copyright file="ODataStreamReferenceValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.IO;
    using Microsoft.OData.Evaluation;
    using Edm;

    #endregion

    // todo (mikep): move these to their own files

    /// <summary>
    /// A class to represent a binary stream value
    /// </summary>
    public sealed class ODataBinaryStreamValue : ODataValue
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Input stream</param>
        public ODataBinaryStreamValue(Stream stream)
        {
            this.Stream = stream;
        }

        /// <summary>
        /// The Stream wrapped by the ODataStreamValue
        /// </summary>
        public Stream Stream { get; private set; }
    }

    /// <summary>
    /// A class to represent a stream value
    /// </summary>
    public class ODataStreamValue : ODataValue
    {
        /// <summary>The name of the named stream this value belongs to; null for the default media resource.</summary>
        private string edmPropertyName;

        /// <summary>Primitive Type, if known.  Either String or None.</summary>
        private EdmPrimitiveTypeKind typeKind;

        /// <summary>
        /// Creates an ODataStringValue with a specified typeKind
        /// </summary>
        /// <param name="typeKind">The type of stream value</param>
        internal ODataStreamValue(EdmPrimitiveTypeKind typeKind)
        {
            this.typeKind = typeKind;
        }

        /// <summary>
        /// PrimitiveTypeKind of the stream.
        /// </summary>
        /// <returns>
        /// EdmPrimitiveTypeKind.String, if the contents is a plain string; 
        /// EdmPrimitiveTypeKind.None if it could be a binary string
        /// </returns>
        public EdmPrimitiveTypeKind TypeKind
        {
            get
            {
                return this.typeKind;
            }
        }

        /// <summary>
        /// Gets the name of the property for this string reference value.
        /// </summary>
        /// <returns>The name of the property for the string reference value, or null for a string within a collection.</returns>
        public string PropertyName
        {
            get
            {
                return this.edmPropertyName;
            }

            internal set
            {
                this.edmPropertyName = value;
            }
        }
    }

    public sealed class ODataTextStreamValue : ODataStreamValue
    {
        /// <summary>
        /// Class for writing a string value as text
        /// </summary>
        public ODataTextStreamValue() : base (EdmPrimitiveTypeKind.String)
        {
        }
    }

    /// <summary>
    /// Represents a Stream value.
    /// </summary>
    public sealed class ODataStreamReferenceValue : ODataStreamValue
    {
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

        /// <summary>
        /// Creates an instance of a StreamReferenceValue for writing a
        /// stream property
        /// </summary>
        public ODataStreamReferenceValue() : base(EdmPrimitiveTypeKind.Stream)
        {
        }

        /// <summary>Gets or sets the edit link for media resource.</summary>
        /// <returns>The edit link for media resource.</returns>
        public Uri EditLink
        {
            get
            {
                return this.HasNonComputedEditLink
                    ? this.editLink
                    : (this.computedEditLink ?? (this.metadataBuilder == null ? null : this.computedEditLink = this.metadataBuilder.GetStreamEditLink(this.PropertyName)));
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
                        this.computedReadLink = this.metadataBuilder.GetStreamReadLink(this.PropertyName)));
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
            get;
            set;
        }

        /// <summary>Gets or sets the media resource ETag.</summary>
        /// <returns>The media resource ETag.</returns>
        public string ETag
        {
            get;
            set;
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
            this.PropertyName = propertyName;
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

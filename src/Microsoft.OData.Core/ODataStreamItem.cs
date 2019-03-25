//---------------------------------------------------------------------
// <copyright file="ODataStreamItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using Edm;
    #endregion

    /// <summary>
    /// A class to represent a stream value
    /// </summary>
    public sealed class ODataStreamItem : ODataItem
    {
        /// <summary>Primitive Type, if known.  Either String, Binary, or None.</summary>
        private EdmPrimitiveTypeKind typeKind;

        /// <summary>
        /// Creates an ODataStreamValue with a specified typeKind.
        /// </summary>
        /// <param name="primitiveTypeKind">The type of stream value.</param>
        public ODataStreamItem(EdmPrimitiveTypeKind primitiveTypeKind) : this(primitiveTypeKind, null)
        {
        }

        /// <summary>
        /// Creates an ODataStreamValue with a specified typeKind.
        /// </summary>
        /// <param name="primitiveTypeKind">PrimitiveTypeKind of the stream value.</param>
        /// <param name="contentType">The mime type of the stream value.</param>
        /// <remarks>primitiveTypeKind is overridden if the MimeType is known.</remarks>
        public ODataStreamItem(EdmPrimitiveTypeKind primitiveTypeKind, string contentType)
        {
            this.PrimitiveTypeKind = primitiveTypeKind;
            this.ContentType = contentType;
        }

        /// <summary>
        /// PrimitiveTypeKind of the stream.
        /// </summary>
        /// <returns>
        /// EdmPrimitiveTypeKind.String, if the contents is a plain text string;
        /// EdmPrimitiveTypeKind.Binary, if the contents is binary;
        /// EdmPrimitiveTypeKind.None is returned if the contents could be binary or plain text
        /// </returns>
        public EdmPrimitiveTypeKind PrimitiveTypeKind
        {
            get
            {
                return this.typeKind;
            }

            private set
            {
                if (typeKind != EdmPrimitiveTypeKind.String &&
                    typeKind != EdmPrimitiveTypeKind.Binary &&
                    typeKind != EdmPrimitiveTypeKind.None)
                {
                    throw new ODataException(Strings.StreamItemInvalidPrimitiveKind(value));
                }

                this.typeKind = value;
            }
        }

        /// <summary>
        /// ContentType of the stream, if known.
        /// </summary>
        /// <returns>
        /// The Mime type of the stream, if known.
        /// </returns>
        public string ContentType
        {
            get; private set;
        }
    }
}

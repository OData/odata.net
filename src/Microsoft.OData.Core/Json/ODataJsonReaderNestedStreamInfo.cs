//---------------------------------------------------------------------
// <copyright file="ODataJsonReaderNestedStreamInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces
    using Microsoft.OData.Edm;
    using System;
    #endregion Namespaces
    internal sealed class ODataJsonReaderStreamInfo
    {
        /// <summary>The primitiveTypeKind of the stream, if known.</summary>
        private EdmPrimitiveTypeKind primitiveTypeKind;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="primitiveTypeKind">The primitiveType being streamed.</param>
        internal ODataJsonReaderStreamInfo(EdmPrimitiveTypeKind primitiveTypeKind)
        {
            this.PrimitiveTypeKind = primitiveTypeKind;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="primitiveTypeKind">The primitiveType being streamed.</param>
        /// <param name="contentType">The mime type being streamed.</param>
        internal ODataJsonReaderStreamInfo(EdmPrimitiveTypeKind primitiveTypeKind, string contentType)
        {
            this.PrimitiveTypeKind = primitiveTypeKind;
            this.ContentType = contentType;
            if (contentType.Contains(MimeConstants.MimeApplicationJson, StringComparison.Ordinal))
            {
                // Json should always be read/written as a string
                this.PrimitiveTypeKind = EdmPrimitiveTypeKind.String;
            }
        }

        /// <summary>
        /// The primitiveTypeKind of the stream being read
        /// </summary>
        internal EdmPrimitiveTypeKind PrimitiveTypeKind
        {
            get
            {
                return this.primitiveTypeKind;
            }

            set
            {
                this.primitiveTypeKind =
                    value == EdmPrimitiveTypeKind.Stream ? EdmPrimitiveTypeKind.Binary : value;
            }
        }

        /// <summary>
        /// The Mime content type of the stream being read, if known
        /// </summary>
        internal string ContentType { get; private set; }
    }
}

//---------------------------------------------------------------------
// <copyright file="ODataJsonLightReaderNestedStreamInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using Microsoft.OData.Edm;
    #endregion Namespaces
    internal sealed class ODataJsonLightReaderStreamInfo
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="primitiveTypeKind">The primitiveType being streamed.</param>
        internal ODataJsonLightReaderStreamInfo(EdmPrimitiveTypeKind primitiveTypeKind)
        {
            this.PrimitiveTypeKind = primitiveTypeKind;
        }

        /// <summary>
        /// The primitiveTypeKind of the stream being read
        /// </summary>
        internal EdmPrimitiveTypeKind PrimitiveTypeKind { get; set; }
    }
}

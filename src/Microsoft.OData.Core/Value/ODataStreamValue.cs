//---------------------------------------------------------------------
// <copyright file="ODataStreamValue.cs" company="Microsoft">
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
    public class ODataStreamValue : ODataValue
    {
        /// <summary>Primitive Type, if known.  Either String or None.</summary>
        private EdmPrimitiveTypeKind typeKind;

        /// <summary>
        /// Creates an ODataStreamValue with a specified typeKind
        /// </summary>
        /// <param name="typeKind">The type of stream value</param>
        public ODataStreamValue(EdmPrimitiveTypeKind typeKind)
        {
            this.typeKind = typeKind;
        }

        /// <summary>
        /// PrimitiveTypeKind of the stream.
        /// </summary>
        /// <returns>
        /// EdmPrimitiveTypeKind.String, if the contents is a plain text string;
        /// EdmPrimitiveTypeKind.Binary, if the contents is binary;
        /// EdmPrimitiveTypeKind.Stream, if the contents is stream;
        /// EdmPrimitiveTypeKind.None is returned if the contents could be binary or plain text
        /// </returns>
        public EdmPrimitiveTypeKind TypeKind
        {
            get
            {
                return this.typeKind;
            }
        }
    }
}

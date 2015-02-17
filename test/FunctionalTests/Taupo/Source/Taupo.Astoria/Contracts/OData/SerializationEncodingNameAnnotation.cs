//---------------------------------------------------------------------
// <copyright file="SerializationEncodingNameAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// Represents the name of the encoding that should be used when serializing this payload
    /// and whether an Xml declaration with the encoding's name should be written or not.
    /// </summary>
    public class SerializationEncodingNameAnnotation : ODataPayloadElementAnnotation
    {
        /// <summary>
        /// Gets or sets the name of the encoding to be used for serialization.
        /// </summary>
        public string EncodingName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to emit or omit the Xml declaration when serializing.
        /// </summary>
        public bool OmitDeclaration { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get { return "Encoding: " + this.EncodingName + "; omit declaration: " + this.OmitDeclaration; }
        }

        /// <summary>
        /// Creates a clone of the annotation.
        /// </summary>
        /// <returns>A clone of the annotation.</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new SerializationEncodingNameAnnotation
            {
                EncodingName = this.EncodingName,
                OmitDeclaration = this.OmitDeclaration,
            };
        }
    }
}

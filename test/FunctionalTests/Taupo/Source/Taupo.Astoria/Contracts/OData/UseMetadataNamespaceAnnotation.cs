//---------------------------------------------------------------------
// <copyright file="UseMetadataNamespaceAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// An annotation indicating that the serializer should use the metadata namespace instead of the data services namespace
    /// </summary>
    public class UseMetadataNamespaceAnnotation : ODataPayloadElementAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the UseMetadataNamespaceAnnotation class
        /// </summary>
        internal UseMetadataNamespaceAnnotation()
        {
        }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return "UseMetadataNamespace";
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new UseMetadataNamespaceAnnotation();
        }
    }
}

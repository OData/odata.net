//---------------------------------------------------------------------
// <copyright file="ODataPropertyMetadataAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// Annotation used on the ODataProperty to hint the metadata building about the desired type of the property.
    /// </summary>
    public sealed class ODataPropertyMetadataAnnotation
    {
        /// <summary>
        /// If set to true the property should be declared as an open property on the type.
        /// </summary>
        public bool IsOpenProperty { get; set; }

        /// <summary>
        /// If used this can modify the kind of the property - key, etag and such.
        /// </summary>
        public ODataPropertyKind Kind { get; set; }

        /// <summary>
        /// If set, this value is used for metadata inference instead of the real property value for metadata inference.
        /// This is useful to specify types for null properties.
        /// </summary>
        public object PropertyValueForTypeInference { get; set; }
    }
}
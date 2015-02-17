//---------------------------------------------------------------------
// <copyright file="SerializationTypeNameAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
{
    /// <summary>
    /// Annotation which stores the type name to serialize.
    /// </summary>
    /// <remarks>
    /// This annotation will be recognized on ODataEntry, ODataComplexValue, ODataCollectionValue and ODataPrimitiveValue.
    /// </remarks>
    public sealed class SerializationTypeNameAnnotation
    {
        /// <summary> Gets or sets the type name to serialize, for the annotated item. </summary>
        /// <returns>The type name to serialize, for the annotated item.</returns>
        /// <remarks>
        /// If this property is null, no type name will be written.
        /// If this property is non-null, the property value will be used as the type name written to the payload.
        /// If this annotation is present, it always overrides the type name specified on the annotated item.
        /// If this annotation is not present, the value of the TypeName property on the ODataEntry, ODataComplexValue or ODataCollectionValue
        /// is used as the type name in the payload.
        /// </remarks>
        public string TypeName { get; set; }
    }
}

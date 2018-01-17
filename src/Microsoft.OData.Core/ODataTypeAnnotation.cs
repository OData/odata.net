//---------------------------------------------------------------------
// <copyright file="ODataTypeAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces

    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Annotation which stores the EDM type information of a value.
    /// </summary>
    /// <remarks>
    /// This annotation will be used on ODataResource and ODataCollectionValue.
    /// </remarks>
    public sealed class ODataTypeAnnotation
    {
        /// <summary>
        /// Creates a new instance of the type annotation without a type name.
        /// </summary>
        public ODataTypeAnnotation()
        {
        }

        /// <summary>
        /// Creates a new instance of the type annotation with a type name.
        /// </summary>
        /// <param name="typeName">The type name read from the input.</param>
        public ODataTypeAnnotation(string typeName)
        {
            this.TypeName = typeName;
        }

        /// <summary>
        /// Creates a new instance of the type annotation with a type.
        /// </summary>
        /// <param name="typeName">The type name read from the input.</param>
        /// <param name="type">The type read from the input.</param>
        internal ODataTypeAnnotation(string typeName, IEdmType type)
            : this(typeName)
        {
            ExceptionUtils.CheckArgumentNotNull(type, "type");

            this.Type = type;
        }

        /// <summary>Gets the type name to serialize, for the annotated item. </summary>
        /// <returns>The type name to serialize, for the annotated item.</returns>
        /// <remarks>
        /// If this property is null, no type name will be written.
        /// If this property is non-null, the property value will be used as the type name written to the payload.
        /// If <see cref="ODataTypeAnnotation"/> is present, it always overrides the type name specified on the annotated item.
        /// If <see cref="ODataTypeAnnotation"/> is not present, the value of the TypeName property on the ODataResource, ODataCollectionValue
        /// is used as the type name in the payload.
        /// </remarks>
        public string TypeName { get; private set; }

        /// <summary>
        /// This property is redundant info about TypeName but to improve reader performance.
        /// </summary>
        internal IEdmType Type { get; private set; }
    }
}

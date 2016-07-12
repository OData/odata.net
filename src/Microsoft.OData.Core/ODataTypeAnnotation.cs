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
        /// <summary>The EDM type of the value this annotation is on.</summary>
        private readonly IEdmTypeReference type;

        /// <summary>The navigation source of the value this annotation is on. Only applies to entity values.</summary>
        private readonly IEdmNavigationSource navigationSource;

        /// <summary>
        /// Creates a new instance of the type annotation for an entity value.
        /// </summary>
        /// <param name="navigationSource">The navigation source the entity belongs to (required).</param>
        /// <param name="structuredType">The structured type of the entity value if not the base type of the entity set (optional).</param>
        public ODataTypeAnnotation(IEdmNavigationSource navigationSource, IEdmStructuredType structuredType)
        {
            ExceptionUtils.CheckArgumentNotNull(structuredType, "structuredType");
            this.navigationSource = navigationSource;
            this.type = structuredType.ToTypeReference(/*isNullable*/ true);
        }

        /// <summary>
        /// Creates a new instance of the type annotation for a complex value.
        /// </summary>
        /// <param name="complexType">The type of the complex value (required).</param>
        public ODataTypeAnnotation(IEdmComplexTypeReference complexType)
        {
            ExceptionUtils.CheckArgumentNotNull(complexType, "complexType");
            this.type = complexType;
        }

        /// <summary>
        /// Creates a new instance of the type annotation for a collection value.
        /// </summary>
        /// <param name="collectionType">The type of the collection value (required).</param>
        public ODataTypeAnnotation(IEdmCollectionTypeReference collectionType)
        {
            ExceptionUtils.CheckArgumentNotNull(collectionType, "collectionType");
            this.type = collectionType;
        }

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

        /// <summary> Gets or sets the type name to serialize, for the annotated item. </summary>
        /// <returns>The type name to serialize, for the annotated item.</returns>
        /// <remarks>
        /// If this property is null, no type name will be written.
        /// If this property is non-null, the property value will be used as the type name written to the payload.
        /// If <see cref="ODataTypeAnnotation"/> is present, it always overrides the type name specified on the annotated item.
        /// If <see cref="ODataTypeAnnotation"/> is not present, the value of the TypeName property on the ODataResource, ODataCollectionValue
        /// is used as the type name in the payload.
        /// </remarks>
        public string TypeName { get; set; }

        /// <summary>
        /// The EDM type of the value.
        /// </summary>
        public IEdmTypeReference Type
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>
        /// The navigation source the value belongs to (only applies to entity values).
        /// </summary>
        public IEdmNavigationSource NavigationSource
        {
            get
            {
                return this.navigationSource;
            }
        }
    }
}

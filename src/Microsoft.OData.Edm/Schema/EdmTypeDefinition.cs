//---------------------------------------------------------------------
// <copyright file="EdmTypeDefinition.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents the definition of an Edm type definition.
    /// </summary>
    public class EdmTypeDefinition : EdmType, IEdmTypeDefinition, IEdmFullNamedElement, IEdmFacetedTypeDefinition
    {
        private readonly IEdmPrimitiveType underlyingType;
        private readonly string namespaceName;
        private readonly string name;
        private readonly string fullName;
        private readonly int? maxLength;
        private readonly bool? isUnicode;
        private readonly int? precision;
        private readonly int? scale;
        private readonly int? srid;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTypeDefinition"/> class with <see cref="EdmPrimitiveTypeKind.Int32"/> underlying type.
        /// </summary>
        /// <param name="namespaceName">Namespace this type definition belongs to.</param>
        /// <param name="name">Name of this type definition.</param>
        /// <param name="underlyingType">The underlying type of this type definition.</param>
        public EdmTypeDefinition(string namespaceName, string name, EdmPrimitiveTypeKind underlyingType)
            : this(namespaceName, name, EdmCoreModel.Instance.GetPrimitiveType(underlyingType))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTypeDefinition"/> class.
        /// </summary>
        /// <param name="namespaceName">Namespace this type definition belongs to.</param>
        /// <param name="name">Name of this type definition.</param>
        /// <param name="underlyingType">The underlying type of this type definition.</param>
        public EdmTypeDefinition(string namespaceName, string name, IEdmPrimitiveType underlyingType)
        {
            EdmUtil.CheckArgumentNull(underlyingType, "underlyingType");
            EdmUtil.CheckArgumentNull(namespaceName, "namespaceName");
            EdmUtil.CheckArgumentNull(name, "name");

            this.underlyingType = underlyingType;
            this.name = name;
            this.namespaceName = namespaceName;
            this.fullName = EdmUtil.GetFullNameForSchemaElement(this.namespaceName, this.name);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTypeDefinition"/> class with
        /// <see cref="EdmPrimitiveTypeKind.Int32"/> underlying type and optional type definition facets.
        /// </summary>
        /// <param name="namespaceName">Namespace this type definition belongs to.</param>
        /// <param name="name">Name of this type definition.</param>
        /// <param name="underlyingType">The underlying type of this type definition.</param>
        /// <param name="maxLength">Optional type definition facet indicating the maximum length
        /// of a string property on a type instance.</param>
        /// <param name="isUnicode">Optional type definition facet indicating whether a string
        /// property might contain and accept string values with Unicode characters beyond
        /// the ASCII character set.</param>
        /// <param name="precision">Optional type definition facet indicating the maximum
        /// number of significant decimal digits allowed on a decimal property's value, or the number
        /// of decimal places allowed in the seconds portion on a temporal property's value.</param>
        /// <param name="scale">Optional type definition facet indicating the maximum
        /// digits allowed on the right of the decimal point on a decimal property's value.</param>
        /// <param name="srid">Optional type definition facet indicating the spatial reference system
        /// applied on a geometry or geography property's value on a type instance.</param>
        public EdmTypeDefinition(
            string namespaceName,
            string name,
            IEdmPrimitiveType underlyingType,
            int? maxLength,
            bool? isUnicode,
            int? precision,
            int? scale,
            int? srid)
            : this(namespaceName, name, underlyingType)
        {
            this.maxLength = maxLength;
            this.isUnicode = isUnicode;
            this.precision = precision;
            this.scale = scale;
            this.srid = srid;
        }

        /// <summary>
        /// Gets the kind of this type.
        /// </summary>
        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.TypeDefinition; }
        }

        /// <summary>
        /// Gets the kind of this schema element.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }

        /// <summary>
        /// Gets the namespace this schema element belongs to.
        /// </summary>
        public string Namespace
        {
            get { return this.namespaceName; }
        }

        /// <summary>
        /// Gets the full name of this schema element.
        /// </summary>
        public string FullName
        {
            get { return this.fullName; }
        }

        /// <summary>
        /// Gets the name of this type definition.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the underlying type of this type definition.
        /// </summary>
        public IEdmPrimitiveType UnderlyingType
        {
            get { return this.underlyingType; }
        }

        /// <inheritdoc/>
        public int? MaxLength => this.maxLength;

        /// <inheritdoc/>
        public bool? IsUnicode => this.isUnicode;

        /// <inheritdoc/>
        public int? Precision => this.precision;

        /// <inheritdoc/>
        public int? Scale => this.scale;

        /// <inheritdoc/>
        public int? Srid => this.srid;
    }
}

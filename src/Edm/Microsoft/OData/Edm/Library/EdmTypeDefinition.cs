//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Library
{
    using System;

    /// <summary>
    /// Represents the definition of an Edm type definition.
    /// </summary>
    public class EdmTypeDefinition : EdmType, IEdmTypeDefinitionWithFacets
    {
        private readonly IEdmPrimitiveType underlyingType;
        private readonly int? maxLength;
        private readonly bool? isUnicode;
        private readonly int? precision;
        private readonly int? scale;
        private readonly int? srid;
        private readonly string namespaceName;
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTypeDefinition"/> class.
        /// </summary>
        /// <param name="namespaceName">Namespace this type definition belongs to.</param>
        /// <param name="name">Name of this type definition.</param>
        /// <param name="underlyingType">The underlying type of this type definition.</param>
        public EdmTypeDefinition(string namespaceName, string name, EdmPrimitiveTypeKind underlyingType)
            : this(namespaceName, name, underlyingType, null, null, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTypeDefinition"/> class.
        /// </summary>
        /// <param name="namespaceName">Namespace this type definition belongs to.</param>
        /// <param name="name">Name of this type definition.</param>
        /// <param name="underlyingType">The underlying type of this type definition.</param>
        public EdmTypeDefinition(string namespaceName, string name, IEdmPrimitiveType underlyingType)
            : this(namespaceName, name, underlyingType, null, null, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTypeDefinition"/> class.
        /// </summary>
        /// <param name="namespaceName">Namespace this type definition belongs to.</param>
        /// <param name="name">Name of this type definition.</param>
        /// <param name="underlyingType">The underlying type of this type definition.</param>
        /// <param name="maxLength">Maximum length of a value of this type.</param>
        /// <param name="isUnicode">Denotes if string is encoded using Unicode.</param>
        /// <param name="precision">Precision of values with this type.</param>
        /// <param name="scale">Scale of values with this type.</param>
        /// <param name="srid">Spatial Reference Identifier for the spatial type being created.</param>
        public EdmTypeDefinition(string namespaceName, string name, EdmPrimitiveTypeKind underlyingType, int? maxLength, bool? isUnicode, int? precision, int? scale, int? srid)
            : this(namespaceName, name, EdmCoreModel.Instance.GetPrimitiveType(underlyingType), maxLength, isUnicode, precision, scale, srid)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTypeDefinition"/> class.
        /// </summary>
        /// <param name="namespaceName">Namespace this type definition belongs to.</param>
        /// <param name="name">Name of this type definition.</param>
        /// <param name="underlyingType">The underlying type of this type definition.</param>
        /// <param name="maxLength">Maximum length of a value of this type.</param>
        /// <param name="isUnicode">Denotes if string is encoded using Unicode.</param>
        /// <param name="precision">Precision of values with this type.</param>
        /// <param name="scale">Scale of values with this type.</param>
        /// <param name="srid">Spatial Reference Identifier for the spatial type being created.</param>
        public EdmTypeDefinition(string namespaceName, string name, IEdmPrimitiveType underlyingType, int? maxLength, bool? isUnicode, int? precision, int? scale, int? srid)
        {
            EdmUtil.CheckArgumentNull(underlyingType, "underlyingType");
            EdmUtil.CheckArgumentNull(namespaceName, "namespaceName");
            EdmUtil.CheckArgumentNull(name, "name");

            this.underlyingType = underlyingType;
            this.name = name;
            this.namespaceName = namespaceName;

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

        /// <summary>
        /// Gets the maximum length of the underlying type.
        /// </summary>
        public int? MaxLength
        {
            get { return this.maxLength; }
        }

        /// <summary>
        /// Gets a value indicating whether the underlying string type supports unicode encoding.
        /// </summary>
        public bool? IsUnicode
        {
            get { return this.isUnicode; }
        }

        /// <summary>
        /// Gets the precision of the underlying type.
        /// </summary>
        public int? Precision
        {
            get { return this.precision; }
        }

        /// <summary>
        /// Gets the scale of the underlying decimal type.
        /// </summary>
        public int? Scale
        {
            get { return this.scale; }
        }

        /// <summary>
        /// Gets the Spatial Reference Identifier of the underlying spatial type.
        /// </summary>
        public int? Srid
        {
            get { return srid; }
        }
    }
}

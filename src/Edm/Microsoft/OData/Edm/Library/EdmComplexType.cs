//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a definition of an EDM complex type.
    /// </summary>
    public class EdmComplexType : EdmStructuredType, IEdmComplexType
    {
        private readonly string namespaceName;
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmComplexType"/> class.
        /// </summary>
        /// <param name="namespaceName">The namespace this type belongs to.</param>
        /// <param name="name">The name of this type within its namespace.</param>
        public EdmComplexType(string namespaceName, string name)
            : this(namespaceName, name, null, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmComplexType"/> class.
        /// </summary>
        /// <param name="namespaceName">The namespace this type belongs to.</param>
        /// <param name="name">The name of this type within its namespace.</param>
        /// <param name="baseType">The base type of this complex type.</param>
        public EdmComplexType(string namespaceName, string name, IEdmComplexType baseType)
            : this(namespaceName, name, baseType, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmComplexType"/> class.
        /// </summary>
        /// <param name="namespaceName">The namespace this type belongs to.</param>
        /// <param name="name">The name of this type within its namespace.</param>
        /// <param name="baseType">The base type of this complex type.</param>
        /// <param name="isAbstract">Denotes whether this complex type is abstract.</param>
        public EdmComplexType(string namespaceName, string name, IEdmComplexType baseType, bool isAbstract)
            : this(namespaceName, name, baseType, isAbstract, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmComplexType"/> class.
        /// Note: Complex type inheritance is not supported in EDM version 3.0 and above.
        /// </summary>
        /// <param name="namespaceName">The namespace this type belongs to.</param>
        /// <param name="name">The name of this type within its namespace.</param>
        /// <param name="baseType">The base type of this complex type.</param>
        /// <param name="isAbstract">Denotes whether this complex type is abstract.</param>
        /// <param name="isOpen">Denotes if the type is open.</param>
        public EdmComplexType(string namespaceName, string name, IEdmComplexType baseType, bool isAbstract, bool isOpen)
            : base(isAbstract, isOpen, baseType)
        {
            EdmUtil.CheckArgumentNull(namespaceName, "namespaceName");
            EdmUtil.CheckArgumentNull(name, "name");

            this.namespaceName = namespaceName;
            this.name = name;
        }

        /// <summary>
        /// Gets the schema element kind of this element.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }

        /// <summary>
        /// Gets the namespace of this element.
        /// </summary>
        public string Namespace
        {
            get { return this.namespaceName; }
        }

        /// <summary>
        /// Gets the name of this element.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the kind of this type.
        /// </summary>
        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Complex; }
        }

        /// <summary>
        /// Gets the kind of this term.
        /// </summary>
        public EdmTermKind TermKind
        {
            get { return EdmTermKind.Type; }
        }
    }
}

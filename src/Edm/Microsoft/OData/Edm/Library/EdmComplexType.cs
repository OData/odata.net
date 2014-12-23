//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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

//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents a definition of an EDM complex type.
    /// </summary>
    public class EdmComplexType : EdmStructuredType, IEdmComplexType
    {
        private string namespaceName;
        private string name;

        /// <summary>
        /// Initializes a new instance of the EdmComplexType class.
        /// </summary>
        /// <param name="isAbstract">Denotes whether this complex type is abstract.</param>
        /// <param name="isOpen">Denotes whether this type is open.</param>
        /// <param name="baseType">The base type of this complex type.</param>
        /// <param name="namespaceName">The namespace this type belongs to.</param>
        /// <param name="name">The name of this type within its namespace.</param>
        public EdmComplexType(bool isAbstract, bool isOpen, IEdmComplexType baseType, string namespaceName, string name)
            : base(EdmTypeKind.Complex, isAbstract, isOpen, baseType)
        {
            this.namespaceName = namespaceName ?? string.Empty;
            this.name = name ?? string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the EdmComplexType class.
        /// </summary>
        public EdmComplexType()
            : base(EdmTypeKind.Complex)
        {
            this.namespaceName = string.Empty;
            this.name = string.Empty;
        }

        /// <summary>
        /// Gets the schema element kind of this element.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }

        /// <summary>
        /// Gets or sets the namespace of this element.
        /// </summary>
        public string Namespace
        {
            get { return this.namespaceName; }
            set 
            { this.SetField(ref this.namespaceName, value ?? string.Empty); }
        }

        /// <summary>
        /// Gets or sets the name of this element.
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.SetField(ref this.name, value ?? string.Empty); }
        }

        /// <summary>
        /// Computes the type to return if a cycle is detected in the base type hierarchy.
        /// </summary>
        /// <returns>Bad type to stand in for the base type in case of a cycle</returns>
        protected override IEdmStructuredType ComputeCycleBaseType()
        {
            return new CyclicComplexType(((IEdmComplexType)this.baseStructuredType).FullName(), this.Location());
        }

        /// <summary>
        /// Ensures the new base type is the correct type and sets the base type of the instance.
        /// </summary>
        /// <param name="newBaseType">New base type for this type.</param>
        protected override void SetBaseType(IEdmStructuredType newBaseType)
        {
            this.SetField(ref this.baseStructuredType, (IEdmComplexType)newBaseType);
        }
    }
}

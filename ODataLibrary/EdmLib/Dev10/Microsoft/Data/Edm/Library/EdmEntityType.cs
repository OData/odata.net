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

using System.Collections.Generic;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents a definition of an EDM entity type.
    /// </summary>
    public class EdmEntityType : EdmStructuredType, IEdmEntityType
    {
        private string namespaceName;
        private string name;
        private List<IEdmStructuralProperty> declaredKey;

        /// <summary>
        /// Initializes a new instance of the EdmEntityType class.
        /// </summary>
        /// <param name="isAbstract">Denotes an entity that cannot be instantiated.</param>
        /// <param name="isOpen">Denotes if the type is open.</param>
        /// <param name="baseType">The base type of this entity type.</param>
        /// <param name="namespaceName">Namespace the entity belongs to.</param>
        /// <param name="name">Name of the entity.</param>
        /// <param name="key">They key of the entity.</param>
        public EdmEntityType(bool isAbstract, bool isOpen, IEdmEntityType baseType, string namespaceName, string name, IEnumerable<IEdmStructuralProperty> key)
            : base(EdmTypeKind.Entity, isAbstract, isOpen, baseType)
        {
            this.namespaceName = namespaceName ?? string.Empty;
            this.name = name ?? string.Empty;
            this.declaredKey = key != null ? new List<IEdmStructuralProperty>(key) : null;
            this.SetField(ref this.baseStructuredType, baseType);
        }

        /// <summary>
        /// Initializes a new instance of the EdmEntityType class.
        /// </summary>
        public EdmEntityType()
            : base(EdmTypeKind.Entity)
        {
            this.namespaceName = string.Empty;
            this.name = string.Empty;
            this.declaredKey = null;
        }

        /// <summary>
        /// Gets or sets the structural properties of the entity type that make up the entity key.
        /// </summary>
        public virtual IEnumerable<IEdmStructuralProperty> DeclaredKey
        {
            get { return this.declaredKey; }
            set { this.SetField(ref this.declaredKey, (List<IEdmStructuralProperty>)(value != null ? new List<IEdmStructuralProperty>(value) : null));}
        }

        /// <summary>
        /// Gets the kind of this schema element.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }

        /// <summary>
        /// Gets or sets the namespace this schema element belongs to.
        /// </summary>
        public string Namespace
        {
            get { return this.namespaceName; }
            set { this.SetField(ref this.namespaceName, value ?? string.Empty); }
        }

        /// <summary>
        /// Gets the namespace uri of this entity type.
        /// </summary>
        public string NamespaceUri
        {
            // ToDo JHamby: Change this to get the Vocabulary URI from the schema.
            get { return this.namespaceName; }
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
        /// Removes a property from this type.
        /// </summary>
        /// <param name="property">The property being removed.</param>
        public override void RemoveProperty(IEdmProperty property)
        {
            base.RemoveProperty(property);
            IEdmStructuralProperty structuralProperty = property as IEdmStructuralProperty;
            if (structuralProperty == null || this.declaredKey == null)
            {
                return;
            }

            if (this.declaredKey.Contains(structuralProperty))
            {
                this.declaredKey.Remove(structuralProperty);
            }
        }

        /// <summary>
        /// Gets the term kind of the entity type.
        /// </summary>
        public EdmTermKind TermKind
        {
            get { return EdmTermKind.Type; }
        }

        /// <summary>
        /// Computes the type to return if a cycle is detected in the base type hierarchy.
        /// </summary>
        /// <returns>Bad type to stand in for the base type in case of a cycle</returns>
        protected  override IEdmStructuredType ComputeCycleBaseType()
        {
            return new CyclicEntityType(((IEdmEntityType)this.baseStructuredType).FullName(), this.Location());
        }

        /// <summary>
        /// Ensures the new base type is the correct type and sets the base type of the instance.
        /// </summary>
        /// <param name="newBaseType">New base type for this type.</param>
        protected override void SetBaseType(IEdmStructuredType newBaseType)
        {
            this.SetField(ref this.baseStructuredType, (IEdmEntityType)newBaseType);
        }
    }
}

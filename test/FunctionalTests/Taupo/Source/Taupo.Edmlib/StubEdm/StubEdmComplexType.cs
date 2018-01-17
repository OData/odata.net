//---------------------------------------------------------------------
// <copyright file="StubEdmComplexType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.Test.Taupo.Edmlib.StubEdm
{
    /// <summary>
    /// Stub implementation of EdmComplexType
    /// </summary>
    public class StubEdmComplexType : StubEdmElement, IEdmComplexType
    {
        private List<IEdmProperty> declaredProperties = new List<IEdmProperty>();

        /// <summary>
        /// Initializes a new instance of the StubEdmComplexType class.
        /// </summary>
        /// <param name="namespaceName">The namespace name</param>
        /// <param name="name">The name of complex type</param>
        public StubEdmComplexType(string namespaceName, string name)
        {
            this.Namespace = namespaceName;
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets a value indicating whether it is abstract
        /// </summary>
        public bool IsAbstract { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it is open
        /// </summary>
        public bool IsOpen { get; set; }

        /// <summary>
        /// Gets or sets the base type
        /// </summary>
        public IEdmStructuredType BaseType { get; set; }

        /// <summary>
        /// Gets the declared properties
        /// </summary>
        public IEnumerable<IEdmProperty> DeclaredProperties
        {
            get { return this.declaredProperties.AsEnumerable(); }
        }

        /// <summary>
        /// Gets the type kind
        /// </summary>
        public EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Complex; }
        }

        /// <summary>
        /// Gets the schema element kind
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }

        /// <summary>
        /// Gets or sets the namespace name
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Finds a property
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <returns>The property.</returns>
        public IEdmProperty FindProperty(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            return this.StructuralProperties().FirstOrDefault(p => p.Name == name);
        }

        /// <summary>
        /// Adds a structural property
        /// </summary>
        /// <param name="structuralProperty">The structural property</param>
        public void Add(IEdmStructuralProperty structuralProperty)
        {
            this.declaredProperties.Add(structuralProperty);
            StubEdmStructuralProperty stubStructuralProperty = structuralProperty as StubEdmStructuralProperty;
            if (stubStructuralProperty != null)
            {
                stubStructuralProperty.DeclaringType = this;
            }
        }
    }
}

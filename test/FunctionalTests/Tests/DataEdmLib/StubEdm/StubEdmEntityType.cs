//---------------------------------------------------------------------
// <copyright file="StubEdmEntityType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;

namespace EdmLibTests.StubEdm
{
    /// <summary>
    /// Stub implementation of EdmEntityType
    /// </summary>
    public class StubEdmEntityType : StubEdmElement, IEdmEntityType
    {
        private List<IEdmProperty> declaredProperties = new List<IEdmProperty>();
        private List<IEdmStructuralProperty> declaredKeyProperties = null;

        /// <summary>
        /// Initializes a new instance of the StubEdmEntityType class.
        /// </summary>
        /// <param name="namespaceName">the namespace name</param>
        /// <param name="name">the name of the entity type</param>
        public StubEdmEntityType(string namespaceName, string name)
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
        /// Gets or sets a value indicating whether it is media entity
        /// </summary>
        public bool HasStream { get; set; }

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
        /// Gets the key
        /// </summary>
        public IEnumerable<IEdmStructuralProperty> DeclaredKey
        {
            get 
            {
                return this.declaredKeyProperties.AsEnumerable();
            }
        }

        /// <summary>
        /// Gets the type kind
        /// </summary>
        public EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Entity; }
        }

        /// <summary>
        /// Gets the schema element kind
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind 
        { 
            get { return EdmSchemaElementKind.TypeDefinition; } 
        }

        /// <summary>
        /// Gets or sets the namespace
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

            return this.StructuralProperties().Cast<IEdmProperty>()
                       .Concat(this.NavigationProperties().Cast<IEdmProperty>())
                       .FirstOrDefault(p => p.Name == name);
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

        /// <summary>
        /// Adds a navigation property
        /// </summary>
        /// <param name="navigation">the navigation property</param>
        /// <param name="hide">whether to make the navigation property invisible as a member of the type</param>
        public void Add(IEdmNavigationProperty navigation, bool hide = false)
        {
            if (!hide)
            {
                this.declaredProperties.Add(navigation);
            }

            StubEdmNavigationProperty stubNavigationProperty = navigation as StubEdmNavigationProperty;
            if (stubNavigationProperty != null)
            {
                stubNavigationProperty.DeclaringType = this;
            }
        }

        /// <summary>
        /// Set the key
        /// </summary>
        /// <param name="keyProperties">The key properties</param>
        public void SetKey(IEnumerable<IEdmStructuralProperty> keyProperties)
        {
            if (this.declaredKeyProperties == null)
            {
                this.declaredKeyProperties = new List<IEdmStructuralProperty>();
            }

            this.declaredKeyProperties.Clear();
            this.declaredKeyProperties.AddRange(keyProperties);
        }

        /// <summary>
        /// Set the key (conveniece method)
        /// </summary>
        /// <param name="keyProperty">The first key property</param>
        /// <param name="additionalKeys">The rest key properties</param>
        public void SetKey(IEdmStructuralProperty keyProperty, params IEdmStructuralProperty[] additionalKeys)
        {
            this.SetKey(new[] { keyProperty }.Concat(additionalKeys));
        }
    }
}

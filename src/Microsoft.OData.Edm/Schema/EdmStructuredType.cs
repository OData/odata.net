//---------------------------------------------------------------------
// <copyright file="EdmStructuredType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Common base class for definitions of EDM structured types.
    /// </summary>
    public abstract class EdmStructuredType : EdmType, IEdmStructuredType
    {
        private readonly IEdmStructuredType baseStructuredType;
        private readonly List<IEdmProperty> declaredProperties = new List<IEdmProperty>();
        private readonly bool isAbstract;
        private readonly bool isOpen;

        // PropertiesDictionary cache.
        private readonly Cache<EdmStructuredType, IDictionary<string, IEdmProperty>> propertiesDictionary = new Cache<EdmStructuredType, IDictionary<string, IEdmProperty>>();
        private static readonly Func<EdmStructuredType, IDictionary<string, IEdmProperty>> ComputePropertiesDictionaryFunc = (me) => me.ComputePropertiesDictionary();

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmStructuredType"/> class.
        /// </summary>
        /// <param name="isAbstract">Denotes a structured type that cannot be instantiated.</param>
        /// <param name="isOpen">Denotes if the type is open.</param>
        /// <param name="baseStructuredType">Base type of the type</param>
        protected EdmStructuredType(bool isAbstract, bool isOpen, IEdmStructuredType baseStructuredType)
        {
            this.isAbstract = isAbstract;
            this.isOpen = isOpen;
            this.baseStructuredType = baseStructuredType;
        }

        /// <summary>
        /// Gets a value indicating whether this type is abstract.
        /// </summary>
        public bool IsAbstract
        {
            get { return this.isAbstract; }
        }

        /// <summary>
        /// Gets a value indicating whether this type is open.
        /// </summary>
        public bool IsOpen
        {
            get { return this.isOpen; }
        }

        /// <summary>
        /// Gets the properties declared immediately within this type.
        /// </summary>
        public virtual IEnumerable<IEdmProperty> DeclaredProperties
        {
            get { return this.declaredProperties; }
        }

        /// <summary>
        /// Gets the base type of this type.
        /// </summary>
        public IEdmStructuredType BaseType
        {
            get { return this.baseStructuredType; }
        }

        /// <summary>
        /// Gets a dictionary of the properties in this type definition for faster lookup.
        /// </summary>
        protected IDictionary<string, IEdmProperty> PropertiesDictionary
        {
            get { return this.propertiesDictionary.GetValue(this, ComputePropertiesDictionaryFunc, null); }
        }

        /// <summary>
        /// Adds the <paramref name="property"/> to this type.
        /// <see cref="IEdmProperty.DeclaringType"/> of the <paramref name="property"/> must be this type.
        /// </summary>
        /// <param name="property">The property being added.</param>
        public void AddProperty(IEdmProperty property)
        {
            EdmUtil.CheckArgumentNull(property, "property");

            if (!Object.ReferenceEquals(this, property.DeclaringType))
            {
                throw new InvalidOperationException(Edm.Strings.EdmModel_Validator_Semantic_DeclaringTypeMustBeCorrect(property.Name));
            }

            this.declaredProperties.Add(property);
            this.propertiesDictionary.Clear(null);
        }

        /// <summary>
        /// Creates and adds a nullable structural property to this type.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        /// <returns>Created structural property.</returns>
        public EdmStructuralProperty AddStructuralProperty(string name, EdmPrimitiveTypeKind type)
        {
            EdmStructuralProperty property = new EdmStructuralProperty(this, name, EdmCoreModel.Instance.GetPrimitive(type, true));
            this.AddProperty(property);
            return property;
        }

        /// <summary>
        /// Creates and adds a nullable structural property to this type.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        /// <param name="isNullable">Flag specifying if the property is nullable.</param>
        /// <returns>Created structural property.</returns>
        public EdmStructuralProperty AddStructuralProperty(string name, EdmPrimitiveTypeKind type, bool isNullable)
        {
            EdmStructuralProperty property = new EdmStructuralProperty(this, name, EdmCoreModel.Instance.GetPrimitive(type, isNullable));
            this.AddProperty(property);
            return property;
        }

        /// <summary>
        /// Creates and adds a structural property to this type.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        /// <returns>Created structural property.</returns>
        public EdmStructuralProperty AddStructuralProperty(string name, IEdmTypeReference type)
        {
            EdmStructuralProperty property = new EdmStructuralProperty(this, name, type);
            this.AddProperty(property);
            return property;
        }

        /// <summary>
        /// Creates and adds a structural property to this type.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        /// <param name="defaultValue">The default value of this property.</param>
        /// <returns>Created structural property.</returns>
        public EdmStructuralProperty AddStructuralProperty(string name, IEdmTypeReference type, string defaultValue)
        {
            EdmStructuralProperty property = new EdmStructuralProperty(this, name, type, defaultValue);
            this.AddProperty(property);
            return property;
        }

        /// <summary>
        /// Creates and adds a unidirectional navigation property to this type.
        /// </summary>
        /// <param name="propertyInfo">Information to create the navigation property.</param>
        /// <returns>Created navigation property.</returns>
        public EdmNavigationProperty AddUnidirectionalNavigation(EdmNavigationPropertyInfo propertyInfo)
        {
            EdmUtil.CheckArgumentNull(propertyInfo, "propertyInfo");

            EdmNavigationProperty property = EdmNavigationProperty.CreateNavigationProperty(this, propertyInfo);

            this.AddProperty(property);
            return property;
        }

        /// <summary>
        /// Searches for a structural or navigation property with the given name in this type and all base types and returns null if no such property exists.
        /// </summary>
        /// <param name="name">The name of the property being found.</param>
        /// <returns>The requested property, or null if no such property exists.</returns>
        public IEdmProperty FindProperty(string name)
        {
            IEdmProperty property;
            return this.PropertiesDictionary.TryGetValue(name, out property) ? property : null;
        }

        /// <summary>
        /// Computes the the cached dictionary of properties for this type definition.
        /// </summary>
        /// <returns>Dictionary of properties keyed by their name.</returns>
        private IDictionary<string, IEdmProperty> ComputePropertiesDictionary()
        {
            Dictionary<string, IEdmProperty> properties = new Dictionary<string, IEdmProperty>();
            foreach (IEdmProperty property in this.Properties())
            {
                RegistrationHelper.RegisterProperty(property, property.Name, properties);
            }

            return properties;
        }
    }
}

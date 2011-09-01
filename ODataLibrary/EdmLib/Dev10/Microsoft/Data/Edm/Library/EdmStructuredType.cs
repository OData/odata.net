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

using System;
using System.Collections.Generic;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Common base class for definitions of EDM structured types.
    /// </summary>
    public abstract class EdmStructuredType : EdmType, IEdmStructuredType, IDependencyTrigger, IDependent
    {
        private bool isAbstract;
        private bool isOpen;
        private readonly List<IEdmProperty> declaredProperties = new List<IEdmProperty>();

        private readonly HashSetInternal<IDependencyTrigger> dependsOn = new HashSetInternal<IDependencyTrigger>();
        private readonly HashSetInternal<IDependent> dependents = new HashSetInternal<IDependent>();

        // PropertiesDictionary cache.
        private readonly Cache<EdmStructuredType, IDictionary<string, IEdmProperty>> propertiesDictionary = new Cache<EdmStructuredType, IDictionary<string, IEdmProperty>>();
        private readonly static Func<EdmStructuredType, IDictionary<string, IEdmProperty>> s_computePropertiesDictionary = (me) => me.ComputePropertiesDictionary();

        /// <summary>
        /// Intended base type of this type.
        /// </summary>
        protected internal IEdmStructuredType baseStructuredType;

        // BaseType cache.
        private readonly Cache<EdmStructuredType, IEdmStructuredType> baseTypeCache = new Cache<EdmStructuredType, IEdmStructuredType>();
        private readonly static Func<EdmStructuredType, IEdmStructuredType> s_computeBaseType = (me) => me.ComputeBaseType();
        private readonly static Func<EdmStructuredType, IEdmStructuredType> s_onCycleBaseType = (me) => me.ComputeCycleBaseType();

        /// <summary>
        /// Initializes a new instance of the EdmStructuredType class.
        /// </summary>
        /// <param name="kind">The kind of the type definition.</param>
        /// <param name="isAbstract">Denotes a structured type that cannot be instantiated.</param>
        /// <param name="isOpen">Denotes if the type is open.</param>
        /// <param name="baseStructuredType">Base type of the type</param>
        protected EdmStructuredType(EdmTypeKind kind, bool isAbstract, bool isOpen, IEdmStructuredType baseStructuredType)
            : base(kind)
        {
            this.isAbstract = isAbstract;
            this.isOpen = isOpen;
            this.baseStructuredType = baseStructuredType;
        }

        /// <summary>
        /// Initializes a new instance of the EdmStructuredType class.
        /// </summary>
        /// <param name="kind">The kind of the type definition.</param>
        protected EdmStructuredType(EdmTypeKind kind)
            : base(kind)
        {
            this.isAbstract = false;
            this.isOpen = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this type is abstract.
        /// </summary>
        public bool IsAbstract
        {
            get { return this.isAbstract; }
            set { this.SetField(ref this.isAbstract, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this type is open.
        /// </summary>
        public bool IsOpen
        {
            get { return this.isOpen; }
            set { this.SetField(ref this.isOpen, value); }
        }

        /// <summary>
        /// Gets the properties declared immediately within this type.
        /// </summary>
        public virtual IEnumerable<IEdmProperty> DeclaredProperties
        {
            get { return this.declaredProperties; }
        }

        /// <summary>
        /// Gets a dictionary of the properties in this type definition for faster lookup.
        /// </summary>
        protected IDictionary<string, IEdmProperty> PropertiesDictionary
        {
            get { return this.propertiesDictionary.GetValue(this, s_computePropertiesDictionary, null); }
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
                throw new InvalidOperationException(Edm.Strings.AddProperty_DeclaringTypeMismatch);
            }

            this.declaredProperties.Add(property);
            this.propertiesDictionary.Clear();
            this.FireDependency();
        }

        /// <summary>
        /// Removes a property from this type.
        /// </summary>
        /// <param name="property">The property being removed.</param>
        public virtual void RemoveProperty(IEdmProperty property)
        {
            this.declaredProperties.Remove(property);
            this.propertiesDictionary.Clear();
            this.FireDependency();
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

        HashSetInternal<IDependent> IDependencyTrigger.Dependents
        {
            get { return this.dependents; }
        }

        HashSetInternal<IDependencyTrigger> IDependent.DependsOn
        {
            get { return this.dependsOn; }
        }

        void IFlushCaches.FlushCaches()
        {
            this.baseTypeCache.Clear();
            this.propertiesDictionary.Clear();
            foreach (IEdmProperty property in this.declaredProperties)
            {
                IFlushCaches flushable = property as IFlushCaches;
                if (flushable != null)
                {
                    flushable.FlushCaches();
                }
            }
        }

        /// <summary>
        /// Computes the the cached dictionary of properties for this type definition.
        /// </summary>
        /// <returns>Dictionary of properties keyed by their name.</returns>
        protected IDictionary<string, IEdmProperty> ComputePropertiesDictionary()
        {
            Dictionary<string, IEdmProperty> properties = new Dictionary<string, IEdmProperty>();
            foreach (IEdmProperty property in this.Properties())
            {
                RegistrationHelper.RegisterProperty(property, property.Name, properties);
            }

            return properties;
        }

        /// <summary>
        /// Gets or sets the base type of this type.
        /// </summary>
        public IEdmStructuredType BaseType
        {
            get { return this.baseTypeCache.GetValue(this, s_computeBaseType, s_onCycleBaseType); }
            set { this.SetBaseType(value); }
        }

        /// <summary>
        /// Ensures the new base type is the correct type and sets the base type of the instance.
        /// </summary>
        /// <param name="newBaseType">New base type for this type.</param>
        protected abstract void SetBaseType(IEdmStructuredType newBaseType);

        private IEdmStructuredType ComputeBaseType()
        {
            if (this.baseStructuredType != null)
            {
                IEdmStructuredType junk = this.baseStructuredType.BaseType; // Evaluate the inductive step to detect cycles.
                return this.baseStructuredType;
            }

            return null;
        }

        /// <summary>
        /// Computes the type to return if a cycle is detected in the base type hierarchy.
        /// </summary>
        /// <returns>Bad type to stand in for the base type in case of a cycle</returns>
        protected virtual IEdmStructuredType ComputeCycleBaseType()
        {
            return null;
        }
    }
}

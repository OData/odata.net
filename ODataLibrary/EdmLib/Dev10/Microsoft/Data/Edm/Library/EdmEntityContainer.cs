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
using System.Linq;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents an EDM entity container.
    /// </summary>
    public class EdmEntityContainer : EdmElement, IEdmEntityContainer, IDependencyTrigger, IDependent
    {
        private string name;
        private readonly List<IEdmEntityContainerElement> elements = new List<IEdmEntityContainerElement>();
        private readonly Dictionary<string, IEdmEntitySet> entitySetDictionary = new Dictionary<string, IEdmEntitySet>();
        private readonly Dictionary<string, IEdmAssociationSet> associationSetDictionary = new Dictionary<string, IEdmAssociationSet>();
        private readonly Dictionary<string, object> functionImportDictionary = new Dictionary<string, object>();

        private readonly HashSetInternal<IDependencyTrigger> dependsOn = new HashSetInternal<IDependencyTrigger>();
        private readonly HashSetInternal<IDependent> dependents = new HashSetInternal<IDependent>();

        /// <summary>
        /// Initializes a new instance of the EdmEntityContainer class.
        /// </summary>
        /// <param name="name">Name of the entity container.</param>
        public EdmEntityContainer(string name)
        {
            this.name = name ?? string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the EdmEntityContainer class.
        /// </summary>
        public EdmEntityContainer()
        {
            this.name = string.Empty;
        }

        /// <summary>
        /// Adds an entity container element to this entity container.
        /// </summary>
        /// <param name="element">The element to add.</param>
        public void AddElement(IEdmEntityContainerElement element)
        {
            EdmUtil.CheckArgumentNull(element, "element");
            this.elements.Add(element);
            RegistrationHelper.RegisterEntityContainerElement(element, element.Name, this.functionImportDictionary, this.entitySetDictionary, this.associationSetDictionary);
        }

        /// <summary>
        /// Removes an entity container element from this entity container.
        /// </summary>
        /// <param name="element">The element to remove.</param>
        public void RemoveElement(IEdmEntityContainerElement element)
        {
            this.elements.Remove(element);
            RegistrationHelper.UnregisterEntityContainerElement(element, element.Name, this.functionImportDictionary, this.entitySetDictionary, this.associationSetDictionary);
        }

        /// <summary>
        /// Gets a collection of the elements of this entity container.
        /// </summary>
        public IEnumerable<IEdmEntityContainerElement> Elements
        {
            get
            {
                List<IEdmAssociationSet> associationSets = new List<IEdmAssociationSet>();

                foreach (IEdmEntityContainerElement element in this.elements)
                {
                    yield return element;
                    EdmEntitySet entitySet = element as EdmEntitySet;
                    if (entitySet != null)
                    {
                        associationSets.AddRange(entitySet.AssociationSets);
                    }
                }

                // Return associations sets after everything else.
                foreach (IEdmAssociationSet associationSet in associationSets)
                {
                    yield return associationSet;
                }
            }
        }

        /// <summary>
        /// Searches for an entity set with the given name in this entity container and returns null if no such set exists.
        /// </summary>
        /// <param name="setName">The name of the element being found.</param>
        /// <returns>The requested element, or null if the element does not exist.</returns>
        public IEdmEntitySet FindEntitySet(string setName)
        {
            IEdmEntitySet element;
            return this.entitySetDictionary.TryGetValue(setName, out element) ? element : null;
        }

        /// <summary>
        /// Searches for an association set with the given name in this entity container and returns null if no such set exists.
        /// </summary>
        /// <param name="setName">The name of the element being found.</param>
        /// <returns>The requested element, or null if the element does not exist.</returns>
        public IEdmAssociationSet FindAssociationSet(string setName)
        {
            IEdmAssociationSet element;
            return this.associationSetDictionary.TryGetValue(setName, out element) ? element : null;
        }

        /// <summary>
        /// Searches for function imports with the given name in this entity container and returns null if no such function import exists.
        /// </summary>
        /// <param name="functionName">The name of the function import being found.</param>
        /// <returns>A group of the requested function imports, or null if no such function import exists.</returns>
        public IEnumerable<IEdmFunctionImport> FindFunctionImports(string functionName)
        {
            object element;
            if (this.functionImportDictionary.TryGetValue(functionName, out element))
            {
                List<IEdmFunctionImport> listElement = this.Elements as List<IEdmFunctionImport>;
                if (listElement != null)
                {
                    return listElement;
                }

                return new IEdmFunctionImport[] { (IEdmFunctionImport)element };
            }

            return Enumerable.Empty<IEdmFunctionImport>();
        }

        /// <summary>
        /// Gets or sets the name of this entity container.
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.SetField(ref this.name, value ?? string.Empty); }
        }

        /// <summary>
        /// Gets the elements that need to refresh their caches when this element is changed.
        /// </summary>
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
        }
    }
}

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
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents an EDM model.
    /// </summary>
    public abstract class EdmModelBase : EdmElement, IEdmModel
    {
        private readonly Collection<IEdmEntityContainer> entityContainers = new Collection<IEdmEntityContainer>();
        private readonly Dictionary<string, IEdmEntityContainer> containersDictionary = new Dictionary<string, IEdmEntityContainer>();
        private readonly Dictionary<string, IEdmSchemaType> schemaTypeDictionary = new Dictionary<string, IEdmSchemaType>();
        private readonly Dictionary<string, IEdmAssociation> associationDictionary = new Dictionary<string, IEdmAssociation>();
        private readonly Dictionary<string, IEdmValueTerm> valueTermDictionary = new Dictionary<string, IEdmValueTerm>();
        private readonly Dictionary<string, object> functionDictionary = new Dictionary<string, object>();

        /// <summary>
        /// Gets the collection of entity containers in this model.
        /// </summary>
        public IEnumerable<IEdmEntityContainer> EntityContainers
        {
            get { return this.entityContainers; }
        }

        /// <summary>
        /// Gets the collection of schema elements that are contained in this model.
        /// </summary>
        public abstract IEnumerable<IEdmSchemaElement> SchemaElements
        {
            get;
        }

        /// <summary>
        /// Searches for an entity container with the given name in this model and returns null if no such entity container exists.
        /// </summary>
        /// <param name="name">The name of the entity container being found.</param>
        /// <returns>The requested entity container, or null if no such entity container exists.</returns>
        public IEdmEntityContainer FindEntityContainer(string name)
        {
            IEdmEntityContainer container;
            return this.containersDictionary.TryGetValue(name, out container) ? container : null;
        }

        /// <summary>
        /// Searches for a type with the given name in this model and returns null if no such type exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the type being found.</param>
        /// <returns>The requested type, or null if no such type exists.</returns>
        public IEdmSchemaType FindType(string qualifiedName)
        {
            IEdmSchemaType result;
            this.schemaTypeDictionary.TryGetValue(qualifiedName, out result);
            return result;
        }

        /// <summary>
        /// Searches for a value term with the given name in this model and returns null if no such value term exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the value term being found.</param>
        /// <returns>The requested value term, or null if no such value term exists.</returns>
        public IEdmValueTerm FindValueTerm(string qualifiedName)
        {
            IEdmValueTerm result;
            this.valueTermDictionary.TryGetValue(qualifiedName, out result);
            return result;
        }

        /// <summary>
        /// Searches for an association with the given name in this model and returns null if no such association exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the type being found.</param>
        /// <returns>The requested association, or null if no such type exists.</returns>
        public IEdmAssociation FindAssociation(string qualifiedName)
        {
            IEdmAssociation result;
            this.associationDictionary.TryGetValue(qualifiedName, out result);
            return result;
        }

        /// <summary>
        /// Searches for a function with the given name in this model and returns null if no such function exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the function being found.</param>
        /// <returns>A group of functions sharing the specified qualified name, or an empty enumerable if no such function exists.</returns>
        public IEnumerable<IEdmFunction> FindFunctions(string qualifiedName)
        {
            object element;
            if (this.functionDictionary.TryGetValue(qualifiedName, out element))
            {
                List<IEdmFunction> listElement = element as List<IEdmFunction>;
                if (listElement != null)
                {
                    return listElement;
                }

                return new IEdmFunction[] { (IEdmFunction)element };
            }

            return Enumerable.Empty<IEdmFunction>();
        }

        /// <summary>
        /// Adds a schema element to this model.
        /// </summary>
        /// <param name="element">The element to register.</param>
        protected void RegisterElement(IEdmSchemaElement element)
        {
            RegistrationHelper.RegisterSchemaElement(element, this.schemaTypeDictionary, this.valueTermDictionary, this.associationDictionary, this.functionDictionary);
        }

        /// <summary>
        /// Removes a schema element from this model.
        /// </summary>
        /// <param name="element">The element to deregister.</param>
        protected void UnregisterElement(IEdmSchemaElement element)
        {
            RegistrationHelper.UnregisterSchemaElement(element, this.schemaTypeDictionary, this.valueTermDictionary, this.associationDictionary, this.functionDictionary);
        }

        /// <summary>
        /// Adds an entity container to this model.
        /// </summary>
        /// <param name="container">The entity container to add.</param>
        public void AddEntityContainer(IEdmEntityContainer container)
        {
            EdmUtil.CheckArgumentNull(container, "container");
            this.entityContainers.Add(container);
            RegistrationHelper.AddElement(container, container.Name, this.containersDictionary, RegistrationHelper.CreateAmbiguousEntityContainerBinding);
        }

        /// <summary>
        /// Gets a dictionary of entity containers contained in this model.
        /// </summary>
        protected Dictionary<string, IEdmEntityContainer> ContainersDictionary
        {
            get { return this.containersDictionary; }
        }

        /// <summary>
        /// Gets the collection of entity containers contained in this model.
        /// </summary>
        protected Collection<IEdmEntityContainer> EntityContainersList
        {
            get { return this.entityContainers; }
        }

        /// <summary>
        /// Searches for vocabulary annotations specified by this model or a referenced model for a given element.
        /// </summary>
        /// <param name="element">The annotated element.</param>
        /// <returns>The vocabulary annotations for the element.</returns>
        public virtual IEnumerable<Annotations.IEdmVocabularyAnnotation> FindVocabularyAnnotations(IEdmAnnotatable element)
        {
            return Enumerable.Empty<Annotations.IEdmVocabularyAnnotation>();
        }
    }
}

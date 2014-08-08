//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library.Annotations;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents an EDM model.
    /// </summary>
    public abstract class EdmModelBase : EdmElement, IEdmModel
    {
        private readonly EdmDirectValueAnnotationsManager annotationsManager;
        private readonly Dictionary<string, IEdmEntityContainer> containersDictionary = new Dictionary<string, IEdmEntityContainer>();
        private readonly Dictionary<string, IEdmSchemaType> schemaTypeDictionary = new Dictionary<string, IEdmSchemaType>();
        private readonly Dictionary<string, IEdmValueTerm> valueTermDictionary = new Dictionary<string, IEdmValueTerm>();
        private readonly Dictionary<string, object> functionDictionary = new Dictionary<string, object>();
        private readonly List<IEdmModel> referencedModels;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmModelBase"/> class.
        /// </summary>
        /// <param name="referencedModels">Models to which this model refers.</param>
        /// <param name="annotationsManager">Annotations manager for the model to use.</param>
        protected EdmModelBase(IEnumerable<IEdmModel> referencedModels, EdmDirectValueAnnotationsManager annotationsManager)
        {
            EdmUtil.CheckArgumentNull(referencedModels, "referencedModels");
            EdmUtil.CheckArgumentNull(annotationsManager, "annotationsManager");

            this.referencedModels = new List<IEdmModel>(referencedModels);
            this.referencedModels.Add(EdmCoreModel.Instance);
            this.annotationsManager = annotationsManager;
        }

        /// <summary>
        /// Gets the collection of schema elements that are contained in this model.
        /// </summary>
        public abstract IEnumerable<IEdmSchemaElement> SchemaElements
        {
            get;
        }

        /// <summary>
        /// Gets the collection of vocabulary annotations that are contained in this model.
        /// </summary>
        public virtual IEnumerable<IEdmVocabularyAnnotation> VocabularyAnnotations
        {
            get { return Enumerable.Empty<IEdmVocabularyAnnotation>(); }
        }

        /// <summary>
        /// Gets the collection of models referred to by this model.
        /// </summary>
        public IEnumerable<IEdmModel> ReferencedModels
        {
            get { return this.referencedModels; }
        }

        /// <summary>
        /// Gets the model's annotations manager.
        /// </summary>
        public IEdmDirectValueAnnotationsManager DirectValueAnnotationsManager
        {
            get { return this.annotationsManager; }
        }

        /// <summary>
        /// Searches for an entity container with the given name in this model and returns null if no such entity container exists.
        /// </summary>
        /// <param name="name">The name of the entity container being found.</param>
        /// <returns>The requested entity container, or null if no such entity container exists.</returns>
        public IEdmEntityContainer FindDeclaredEntityContainer(string name)
        {
            IEdmEntityContainer container;
            return this.containersDictionary.TryGetValue(name, out container) ? container : null;
        }

        /// <summary>
        /// Searches for a type with the given name in this model and returns null if no such type exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the type being found.</param>
        /// <returns>The requested type, or null if no such type exists.</returns>
        public IEdmSchemaType FindDeclaredType(string qualifiedName)
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
        public IEdmValueTerm FindDeclaredValueTerm(string qualifiedName)
        {
            IEdmValueTerm result;
            this.valueTermDictionary.TryGetValue(qualifiedName, out result);
            return result;
        }

        /// <summary>
        /// Searches for a function with the given name in this model and returns null if no such function exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the function being found.</param>
        /// <returns>A group of functions sharing the specified qualified name, or an empty enumerable if no such function exists.</returns>
        public IEnumerable<IEdmFunction> FindDeclaredFunctions(string qualifiedName)
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
        /// Searches for vocabulary annotations specified by this model or a referenced model for a given element.
        /// </summary>
        /// <param name="element">The annotated element.</param>
        /// <returns>The vocabulary annotations for the element.</returns>
        public virtual IEnumerable<IEdmVocabularyAnnotation> FindDeclaredVocabularyAnnotations(IEdmVocabularyAnnotatable element)
        {
            return Enumerable.Empty<IEdmVocabularyAnnotation>();
        }

        /// <summary>
        /// Finds a list of types that derive directly from the supplied type.
        /// </summary>
        /// <param name="baseType">The base type that derived types are being searched for.</param>
        /// <returns>A list of types that derive directly from the base type.</returns>
        public abstract IEnumerable<IEdmStructuredType> FindDirectlyDerivedTypes(IEdmStructuredType baseType);

        /// <summary>
        /// Adds a schema element to this model.
        /// </summary>
        /// <param name="element">The element to register.</param>
        protected void RegisterElement(IEdmSchemaElement element)
        {
            EdmUtil.CheckArgumentNull(element, "element");
            RegistrationHelper.RegisterSchemaElement(element, this.schemaTypeDictionary, this.valueTermDictionary, this.functionDictionary, this.containersDictionary);
        }

        /// <summary>
        /// Adds a model reference to this model.
        /// </summary>
        /// <param name="model">The model to reference.</param>
        protected void AddReferencedModel(IEdmModel model)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            this.referencedModels.Add(model);
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="ModelWithRemovableElements.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;

namespace EdmLibTests.FunctionalUtilities
{
    internal class ModelWithRemovableElements<T> : IEdmModel
        where T : IEdmModel
    {
        private readonly T model;
        private Dictionary<IEdmSchemaElement, object> removedElements = new Dictionary<IEdmSchemaElement, object>();
        private Dictionary<IEdmVocabularyAnnotation, object> removedVocabularyAnnotations = new Dictionary<IEdmVocabularyAnnotation, object>();
        private Dictionary<IEdmModel, object> removeReferencedModels = new Dictionary<IEdmModel, object>();

        public ModelWithRemovableElements(T model)
        {
            this.model = model;
        }

        public T WrappedModel
        {
            get { return this.model; }
        }

        public IEnumerable<IEdmSchemaElement> SchemaElements
        {
            get
            {
                return this.model.SchemaElements.Except(this.removedElements.Keys);
            }
        }

        public IEnumerable<string> DeclaredNamespaces
        {
            get { return this.SchemaElements.Select(s => s.Namespace).Distinct(); }
        }

        public IEnumerable<IEdmVocabularyAnnotation> VocabularyAnnotations
        {
            get
            {
                return this.model.VocabularyAnnotations.Except(this.removedVocabularyAnnotations.Keys);
            }
        }

        public IEnumerable<IEdmModel> ReferencedModels
        {
            get { return this.model.ReferencedModels.Except(this.removeReferencedModels.Keys); }
        }

        public IEdmDirectValueAnnotationsManager DirectValueAnnotationsManager
        {
            get { return this.model.DirectValueAnnotationsManager; }
        }

        public IEdmEntityContainer EntityContainer
        {
            get
            {
                if (model.EntityContainer == null)
                {
                    return null;
                }

                if (this.removedElements.ContainsKey(model.EntityContainer))
                {
                    return null;
                }

                return this.model.EntityContainer;
            }
        }

        /// <summary>
        /// Searches for a type with the given name in this model only and returns null if no such type exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the type being found.</param>
        /// <returns>The requested type, or null if no such type exists.</returns>
        public IEdmSchemaType FindDeclaredType(string qualifiedName)
        {
            IEdmSchemaType type = this.model.FindDeclaredType(qualifiedName);
            return type != null && this.removedElements.ContainsKey(type) ? null : type;
        }

        public IEnumerable<IEdmOperation> FindDeclaredOperations(string qualifiedName)
        {
            IEnumerable<IEdmOperation> functions = this.model.FindDeclaredOperations(qualifiedName);
            return functions.Except(this.removedElements.Keys.Where(e => e.SchemaElementKind == EdmSchemaElementKind.Action || e.SchemaElementKind == EdmSchemaElementKind.Function).Cast<IEdmOperation>());
        }

        public IEnumerable<IEdmOperation> FindDeclaredBoundOperations(IEdmType bindingType)
        {
            IEnumerable<IEdmOperation> functions = this.model.FindDeclaredBoundOperations(bindingType);
            return functions.Except(this.removedElements.Keys.Where(e => e.SchemaElementKind == EdmSchemaElementKind.Action || e.SchemaElementKind == EdmSchemaElementKind.Function).Cast<IEdmOperation>());
        }

        public virtual IEnumerable<IEdmOperation> FindDeclaredBoundOperations(string qualifiedName, IEdmType bindingType)
        {
            return this.FindDeclaredOperations(qualifiedName).Where(o => o.IsBound && o.Parameters.Any() && o.HasEquivalentBindingType(bindingType));
        }

        public IEdmTerm FindDeclaredTerm(string qualifiedName)
        {
            IEdmTerm term = this.model.FindDeclaredTerm(qualifiedName);
            return term != null && this.removedElements.ContainsKey(term) ? null : term;
        }

        public IEnumerable<IEdmVocabularyAnnotation> FindDeclaredVocabularyAnnotations(IEdmVocabularyAnnotatable element)
        {
            IEnumerable<IEdmVocabularyAnnotation> annotations = this.model.FindDeclaredVocabularyAnnotations(element);
            return annotations.Except(this.removedVocabularyAnnotations.Keys);
        }

        public IEnumerable<IEdmStructuredType> FindDirectlyDerivedTypes(IEdmStructuredType baseType)
        {
            throw new NotImplementedException("Find derived types is not implemented");
        }

        internal void RemoveElement(IEdmSchemaElement element)
        {
            this.removedElements[element] = true;
        }

        internal void RemoveVocabularyAnnotation(IEdmVocabularyAnnotation annotation)
        {
            this.removedVocabularyAnnotations[annotation] = true;
        }

        internal void RemoveReference(IEdmModel reference)
        {
            this.removeReferencedModels[reference] = true;
            foreach (var tmp in reference.SchemaElements)
            {
                this.removedElements[tmp] = true;
            }
        }
    }
}

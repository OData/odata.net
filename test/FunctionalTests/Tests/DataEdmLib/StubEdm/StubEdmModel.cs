//---------------------------------------------------------------------
// <copyright file="StubEdmModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.StubEdm
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;

    /// <summary>
    /// Stub implementation of the EdmModel
    /// </summary>
    public class StubEdmModel : StubEdmElement, IEdmModel, IEnumerable
    {
        private List<IEdmEntityContainer> entityContainers = new List<IEdmEntityContainer>();
        private List<IEdmSchemaElement> schemaElements = new List<IEdmSchemaElement>();
        private StubAnnotationsManager annotationsManager = new StubAnnotationsManager();

        /// <summary>
        /// Gets the schema elements
        /// </summary>
        public IEnumerable<IEdmSchemaElement> SchemaElements
        {
            get { return this.schemaElements.AsEnumerable().Concat(this.entityContainers.Cast<IEdmSchemaElement>()); }
        }

        /// <summary>
        /// Gets the collection of namespaces that schema elements use contained in this model.
        /// </summary>
        public IEnumerable<string> DeclaredNamespaces
        {
            get { return this.SchemaElements.Select(s => s.Namespace).Distinct(); }
        }

        /// <summary>
        /// Gets the vocabulary annotations
        /// </summary>
        public IEnumerable<IEdmVocabularyAnnotation> VocabularyAnnotations
        {
            get { return Enumerable.Empty<IEdmVocabularyAnnotation>(); }
        }
        
        /// <summary>
        /// Gets the collection of models referred to by this model.
        /// </summary>
        public IEnumerable<IEdmModel> ReferencedModels
        {
            get { return new IEdmModel[] { EdmCoreModel.Instance }; }
        }

        /// <summary>
        /// Gets the model's annotations manager.
        /// </summary>
        public IEdmDirectValueAnnotationsManager DirectValueAnnotationsManager
        {
            get { return this.annotationsManager; }
        }

        /// <summary>
        /// Gets the only one entity container of the model.
        /// </summary>
        public IEdmEntityContainer EntityContainer
        {
            get { return this.entityContainers.FirstOrDefault(); }
        }

        /// <summary>
        /// Finds a schema type by name
        /// </summary>
        /// <param name="qualifiedName">the qualified name of the element</param>
        /// <returns>the schema type</returns>
        public IEdmSchemaType FindDeclaredType(string qualifiedName)
        {
            if (qualifiedName == null)
            {
                throw new ArgumentNullException("qualifiedName");
            }

            return this.schemaElements.OfType<IEdmSchemaType>().FirstOrDefault(e => (e.FullName() == qualifiedName));
        }

        /// <summary>
        /// Finds a term by name
        /// </summary>
        /// <param name="qualifiedName">the qualified name of the element</param>
        /// <returns>the term</returns>
        public IEdmTerm FindDeclaredTerm(string qualifiedName)
        {
            if (qualifiedName == null)
            {
                throw new ArgumentNullException("qualifiedName");
            }

            return this.schemaElements.OfType<IEdmTerm>().FirstOrDefault(e => e.FullName() == qualifiedName);
        }

        /// <summary>
        /// Finds a schema type by name
        /// </summary>
        /// <param name="qualifiedName">the qualified name of the element</param>
        /// <returns>the schema type</returns>
        public IEnumerable<IEdmOperation> FindDeclaredOperations(string qualifiedName)
        {
            if (qualifiedName == null)
            {
                throw new ArgumentNullException("qualifiedName");
            }

            return this.schemaElements.OfType<IEdmOperation>().Where(e => e.FullName() == qualifiedName);
        }

        /// <summary>
        /// Finds all operations with the given name which are bindable to an instance of the giving binding type or a more derived type.
        /// </summary>
        /// <param name="bindingType">The binding entity type.</param>
        /// <param name="operationName">The name of the operations to find. May be qualified with an entity container name.</param>
        /// <returns>The operations that match the search criteria.</returns>
        public IEnumerable<IEdmOperation> FindDeclaredBoundOperations(IEdmType bindingType)
        {
            return this.schemaElements.OfType<IEdmOperation>().Where(f => f.IsBound
                        && f.Parameters.Any()
                        && f.HasEquivalentBindingType(bindingType));
        }

        /// <summary>
        /// Searches for bound operations based on the qualified name and binding type, returns an empty enumerable if no operation exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the operation.</param>
        /// <param name="bindingType">Type of the binding.</param>
        /// <returns>
        /// A set of operations that share the qualified name and binding type or empty enumerable if no such operation exists.
        /// </returns>
        public IEnumerable<IEdmOperation> FindDeclaredBoundOperations(string qualifiedName, IEdmType bindingType)
        {
            return this.FindDeclaredOperations(qualifiedName).Where(o => o.IsBound && o.Parameters.Any() && o.HasEquivalentBindingType(bindingType));
        }

        /// <summary>
        /// Searches for vocabulary annotations specified by this model or a referenced model for a given element.
        /// </summary>
        /// <param name="element">The annotated element.</param>
        /// <returns>The vocabulary annotations for the element.</returns>
        public IEnumerable<IEdmVocabularyAnnotation> FindDeclaredVocabularyAnnotations(IEdmVocabularyAnnotatable element)
        {
            StubEdmElement stubElement = element as StubEdmElement;
            return stubElement != null ? stubElement.InlineVocabularyAnnotations : Enumerable.Empty<IEdmVocabularyAnnotation>();
        }

        /// <summary>
        /// Finds a list of types that derive from the supplied type.
        /// </summary>
        /// <param name="baseType">The base type that derived types are being searched for.</param>
        /// <returns>A list of types that derive from the type.</returns>
        public IEnumerable<IEdmStructuredType> FindDirectlyDerivedTypes(IEdmStructuredType baseType)
        {
            throw new NotImplementedException("Find derived types is not implemented in the stub model");
        }

        /// <summary>
        /// Adds a container
        /// </summary>
        /// <param name="container">the container</param>
        public void Add(IEdmEntityContainer container)
        {
            this.entityContainers.Add(container);
        }

        /// <summary>
        /// Adds a schema element
        /// </summary>
        /// <param name="schemaElement">the schema element</param>
        public void Add(IEdmSchemaElement schemaElement)
        {
            this.schemaElements.Add(schemaElement);
        }

        private class StubAnnotationsManager : IEdmDirectValueAnnotationsManager
        {
            private Dictionary<IEdmElement, List<IEdmDirectValueAnnotation>> annotations = new Dictionary<IEdmElement, List<IEdmDirectValueAnnotation>>();

            /// <summary>
            /// Sets an annotation value for an EDM element. If the value is null, no annotation is added and an existing annotation with the same name is removed.
            /// </summary>
            /// <param name="element">The annotated element.</param>
            /// <param name="namespaceName">Namespace that the annotation belongs to.</param>
            /// <param name="localName">Name of the annotation within the namespace.</param>
            /// <param name="value">New annotation to set.</param>
            public void SetAnnotationValue(IEdmElement element, string namespaceName, string localName, object value)
            {
                List<IEdmDirectValueAnnotation> elementAnnotations = this.FindAnnotations(element, true);

                IEdmDirectValueAnnotation annotation = FindAnnotation(elementAnnotations, namespaceName, localName);
                if (annotation != null)
                {
                    elementAnnotations.Remove(annotation);
                }

                if (value != null)
                {
                    elementAnnotations.Add(new EdmDirectValueAnnotation(namespaceName, localName, value));
                }
            }

            /// <summary>
            /// Sets a set of annotation values. If a supplied value is null, no annotation is added and an existing annotation with the same name is removed.
            /// </summary>
            /// <param name="annotationBindings">The annotations to set</param>
            public void SetAnnotationValues(IEnumerable<IEdmDirectValueAnnotationBinding> annotationBindings)
            {
                foreach (IEdmDirectValueAnnotationBinding annotation in annotationBindings)
                {
                    this.SetAnnotationValue(annotation.Element, annotation.NamespaceUri, annotation.Name, annotation.Value);
                }
            }

            /// <summary>
            /// Retrieves an annotation value for an EDM element. Returns null if no annotation with the given name exists.
            /// </summary>
            /// <param name="element">The annotated element.</param>
            /// <param name="namespaceName">Namespace that the annotation belongs to.</param>
            /// <param name="localName">Local name of the annotation.</param>
            /// <returns>Returns the annotation that corresponds to the provided name. Returns null if no annotation with the given name exists. </returns>
            public object GetAnnotationValue(IEdmElement element, string namespaceName, string localName)
            {
                List<IEdmDirectValueAnnotation> elementAnnotations = this.FindAnnotations(element, false);
                IEdmDirectValueAnnotation annotation = FindAnnotation(elementAnnotations, namespaceName, localName);
                return annotation != null ? annotation.Value : null;
            }

            /// <summary>
            /// Retrieves a set of annotation values. For each requested value, returns null if no annotation with the given name exists for the given element.
            /// </summary>
            /// <param name="annotationBindings">The set of requested annotations</param>
            /// <returns>Returns values that correspond to the provided annotations. A value is null if no annotation with the given name exists for the given element.</returns>
            public object[] GetAnnotationValues(IEnumerable<IEdmDirectValueAnnotationBinding> annotationBindings)
            {
                object[] values = new object[annotationBindings.Count()];

                int index = 0;
                foreach (IEdmDirectValueAnnotationBinding annotation in annotationBindings)
                {
                    values[index++] = this.GetAnnotationValue(annotation.Element, annotation.NamespaceUri, annotation.Name);
                }

                return values;
            }

            /// <summary>
            /// Gets annotations associated with an element.
            /// </summary>
            /// <param name="element">The annotated element.</param>
            /// <returns>The immediate annotations for the element.</returns>
            public IEnumerable<IEdmDirectValueAnnotation> GetDirectValueAnnotations(IEdmElement element)
            {
                return this.FindAnnotations(element, false) ?? Enumerable.Empty<IEdmDirectValueAnnotation>();
            }

            private static IEdmDirectValueAnnotation FindAnnotation(List<IEdmDirectValueAnnotation> annotations, string namespaceName, string localName)
            {
                return annotations != null ? annotations.FirstOrDefault(a => a.NamespaceUri == namespaceName && a.Name == localName) : null;
            }

            private List<IEdmDirectValueAnnotation> FindAnnotations(IEdmElement element, bool create)
            {
                List<IEdmDirectValueAnnotation> elementAnnotations;
                if (!this.annotations.TryGetValue(element, out elementAnnotations))
                {
                    if (create)
                    {
                        elementAnnotations = new List<IEdmDirectValueAnnotation>();
                        this.annotations[element] = elementAnnotations;
                    }
                }

                return elementAnnotations;
            }
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="EdmModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM model.
    /// </summary>
    public class EdmModel : EdmModelBase
    {
        private readonly List<IEdmSchemaElement> elements = new List<IEdmSchemaElement>();
        private readonly Dictionary<IEdmVocabularyAnnotatable, List<IEdmVocabularyAnnotation>> vocabularyAnnotationsDictionary = new Dictionary<IEdmVocabularyAnnotatable, List<IEdmVocabularyAnnotation>>();
        private readonly Dictionary<IEdmStructuredType, List<IEdmStructuredType>> derivedTypeMappings = new Dictionary<IEdmStructuredType, List<IEdmStructuredType>>();
        private readonly HashSet<string> declaredNamespaces = new HashSet<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmModel"/> class.
        /// </summary>
        public EdmModel()
            : base(Enumerable.Empty<IEdmModel>(), new EdmDirectValueAnnotationsManager())
        {
        }

        /// <summary>
        /// Gets the collection of schema elements that are contained in this model and referenced models.
        /// </summary>
        public override IEnumerable<IEdmSchemaElement> SchemaElements
        {
            // TODO: REF plus referneced elements
            get { return this.elements; }
        }

        /// <summary>
        /// Gets the collection of namespaces that schema elements use contained in this model.
        /// </summary>
        public override IEnumerable<string> DeclaredNamespaces
        {
            get { return this.declaredNamespaces; }
        }

        /// <summary>
        /// Gets the collection of vocabulary annotations that are contained in this model.
        /// </summary>
        public override IEnumerable<IEdmVocabularyAnnotation> VocabularyAnnotations
        {
            get { return this.vocabularyAnnotationsDictionary.SelectMany(kvp => kvp.Value); }
        }

        /// <summary>
        /// Adds a model reference to this model.
        /// </summary>
        /// <param name="model">The model to be referenced.</param>
        public new void AddReferencedModel(IEdmModel model)
        {
            base.AddReferencedModel(model);
        }

        /// <summary>
        /// Adds a schema element to this model.
        /// </summary>
        /// <param name="element">Element to be added.</param>
        public void AddElement(IEdmSchemaElement element)
        {
            if (!this.declaredNamespaces.Contains(element.Namespace))
            {
                this.declaredNamespaces.Add(element.Namespace);
            }

            EdmUtil.CheckArgumentNull(element, "element");
            this.elements.Add(element);
            IEdmStructuredType structuredType = element as IEdmStructuredType;
            if (structuredType != null && structuredType.BaseType != null)
            {
                List<IEdmStructuredType> derivedTypes;
                if (!this.derivedTypeMappings.TryGetValue(structuredType.BaseType, out derivedTypes))
                {
                    derivedTypes = new List<IEdmStructuredType>();
                    this.derivedTypeMappings[structuredType.BaseType] = derivedTypes;
                }

                derivedTypes.Add(structuredType);
            }

            this.RegisterElement(element);
        }

        /// <summary>
        /// Adds a collection of schema elements to this model.
        /// </summary>
        /// <param name="newElements">Elements to be added.</param>
        public void AddElements(IEnumerable<IEdmSchemaElement> newElements)
        {
            EdmUtil.CheckArgumentNull(newElements, "newElements");
            foreach (IEdmSchemaElement element in newElements)
            {
                this.AddElement(element);
            }
        }

        /// <summary>
        /// Adds a vocabulary annotation to this model.
        /// </summary>
        /// <param name="annotation">The annotation to be added.</param>
        public void AddVocabularyAnnotation(IEdmVocabularyAnnotation annotation)
        {
            EdmUtil.CheckArgumentNull(annotation, "annotation");
            if (annotation.Target == null)
            {
                throw new InvalidOperationException(Strings.Constructable_VocabularyAnnotationMustHaveTarget);
            }

            List<IEdmVocabularyAnnotation> elementAnnotations;
            if (!this.vocabularyAnnotationsDictionary.TryGetValue(annotation.Target, out elementAnnotations))
            {
                elementAnnotations = new List<IEdmVocabularyAnnotation>();
                this.vocabularyAnnotationsDictionary.Add(annotation.Target, elementAnnotations);
            }

            elementAnnotations.Add(annotation);
        }

        /// <summary>
        /// Searches for vocabulary annotations specified by this model.
        /// </summary>
        /// <param name="element">The annotated element.</param>
        /// <returns>The vocabulary annotations for the element.</returns>
        public override IEnumerable<IEdmVocabularyAnnotation> FindDeclaredVocabularyAnnotations(IEdmVocabularyAnnotatable element)
        {
            List<IEdmVocabularyAnnotation> elementAnnotations;
            return this.vocabularyAnnotationsDictionary.TryGetValue(element, out elementAnnotations) ? elementAnnotations : Enumerable.Empty<IEdmVocabularyAnnotation>();
        }

        /// <summary>
        /// Finds a list of types that derive directly from the supplied type.
        /// </summary>
        /// <param name="baseType">The base type that derived types are being searched for.</param>
        /// <returns>A list of types from this model that derive directly from the given type.</returns>
        public override IEnumerable<IEdmStructuredType> FindDirectlyDerivedTypes(IEdmStructuredType baseType)
        {
            List<IEdmStructuredType> types;
            if (this.derivedTypeMappings.TryGetValue(baseType, out types))
            {
                return types;
            }

            return Enumerable.Empty<IEdmStructuredType>();
        }

        /// <summary>
        /// Set a vocabulary annotation to this model.
        /// </summary>
        /// <param name="annotation">The annotation to be set.</param>
        public void SetVocabularyAnnotation(IEdmVocabularyAnnotation annotation)
        {
            EdmUtil.CheckArgumentNull(annotation, "annotation");
            if (annotation.Target == null)
            {
                throw new InvalidOperationException(Strings.Constructable_VocabularyAnnotationMustHaveTarget);
            }

            List<IEdmVocabularyAnnotation> elementAnnotations;
            if (!this.vocabularyAnnotationsDictionary.TryGetValue(annotation.Target, out elementAnnotations))
            {
                elementAnnotations = new List<IEdmVocabularyAnnotation>();
                this.vocabularyAnnotationsDictionary.Add(annotation.Target, elementAnnotations);
            }

            elementAnnotations.RemoveAll(p => p.Term.FullName() == annotation.Term.FullName());
            elementAnnotations.Add(annotation);
        }
    }
}

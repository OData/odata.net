//---------------------------------------------------------------------
// <copyright file="EdmModelBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Vocabularies.V1;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents an EDM model.
    /// </summary>
    public abstract class EdmModelBase : EdmElement, IEdmModel
    {
        private readonly List<IEdmModel> referencedEdmModels;
        private readonly IEdmDirectValueAnnotationsManager annotationsManager;
        private readonly Dictionary<string, IEdmEntityContainer> containersDictionary = new Dictionary<string, IEdmEntityContainer>();
        private readonly Dictionary<string, IEdmSchemaType> schemaTypeDictionary = new Dictionary<string, IEdmSchemaType>();
        private readonly Dictionary<string, IEdmValueTerm> valueTermDictionary = new Dictionary<string, IEdmValueTerm>();
        private readonly Dictionary<string, IList<IEdmOperation>> functionDictionary = new Dictionary<string, IList<IEdmOperation>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmModelBase"/> class.
        /// </summary>
        /// <param name="referencedModels">Models to which this model refers.</param>
        /// <param name="annotationsManager">Annotations manager for the model to use.</param>
        /// <remarks>Only either mainModel and referencedModels should have value.</remarks>
        protected EdmModelBase(IEnumerable<IEdmModel> referencedModels, IEdmDirectValueAnnotationsManager annotationsManager)
        {
            EdmUtil.CheckArgumentNull(referencedModels, "referencedModels");
            EdmUtil.CheckArgumentNull(annotationsManager, "annotationsManager");

            this.referencedEdmModels = new List<IEdmModel>(referencedModels);

            this.referencedEdmModels.Add(EdmCoreModel.Instance);

            if (CoreVocabularyModel.Instance != null)
            {
                this.referencedEdmModels.Add(CoreVocabularyModel.Instance);
            }

            if (CapabilitiesVocabularyModel.Instance != null)
            {
                this.referencedEdmModels.Add(CapabilitiesVocabularyModel.Instance);
            }

            this.annotationsManager = annotationsManager;
        }

        /// <summary>
        /// Gets the collection of schema elements that are contained in this model and referenced models.
        /// </summary>
        public abstract IEnumerable<IEdmSchemaElement> SchemaElements
        {
            get;
        }

        /// <summary>
        /// Gets the collection of namespaces that schema elements use contained in this model.
        /// </summary>
        public abstract IEnumerable<string> DeclaredNamespaces
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
            get { return this.referencedEdmModels; }
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
            get { return this.containersDictionary.Values.FirstOrDefault(); }
        }

        /// <summary>
        /// Searches for a type with the given name in this model only and returns null if no such type exists.
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
        /// Searches for a operation with the given name in this model and returns null if no such operation exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the operation being found.</param>
        /// <returns>A group of operations sharing the specified qualified name, or an empty enumerable if no such operation exists.</returns>
        public IEnumerable<IEdmOperation> FindDeclaredOperations(string qualifiedName)
        {
            IList<IEdmOperation> elements;
            if (this.functionDictionary.TryGetValue(qualifiedName, out elements))
            {
                return elements;
            }

            return Enumerable.Empty<IEdmOperation>();
        }

        /// <summary>
        /// Searches for bound operations based on the binding type, returns an empty enumerable if no operation exists.
        /// </summary>
        /// <param name="bindingType">Type of the binding.</param>
        /// <returns> A set of operations that share the binding type or empty enumerable if no such operation exists. </returns>
        public virtual IEnumerable<IEdmOperation> FindDeclaredBoundOperations(IEdmType bindingType)
        {
            foreach (IEnumerable<IEdmOperation> operations in this.functionDictionary.Values.Distinct())
            {
                foreach (IEdmOperation operation in operations.Where(o => o.IsBound && o.Parameters.Any() && o.HasEquivalentBindingType(bindingType)))
                {
                    yield return operation;
                }
            }
        }

        /// <summary>
        /// Searches for bound operations based on the qualified name and binding type, returns an empty enumerable if no operation exists.
        /// </summary>
        /// <param name="qualifiedName">The qualifeid name of the operation.</param>
        /// <param name="bindingType">Type of the binding.</param>
        /// <returns>
        /// A set of operations that share the name and binding type or empty enumerable if no such operation exists.
        /// </returns>
        public virtual IEnumerable<IEdmOperation> FindDeclaredBoundOperations(string qualifiedName, IEdmType bindingType)
        {
            return this.FindDeclaredOperations(qualifiedName).Where(o => o.IsBound && o.Parameters.Any() && o.HasEquivalentBindingType(bindingType));
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
            this.referencedEdmModels.Add(model);
        }
    }
}

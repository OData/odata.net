//---------------------------------------------------------------------
// <copyright file="EdmModelBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;

#if NET9_0
using System;
using System.Diagnostics.CodeAnalysis;
#endif

namespace Microsoft.OData.Edm
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
        private readonly Dictionary<string, IEdmTerm> termDictionary = new Dictionary<string, IEdmTerm>();
        private readonly Dictionary<string, IList<IEdmOperation>> functionDictionary = new Dictionary<string, IList<IEdmOperation>>();

        /// <summary>
        /// Cache of operations that are bindable to entity types, its a cache of all bindable functions, indexed by binding type.
        /// </summary>
        private readonly ConcurrentDictionary<string, IList<IEdmOperation>> bindableOperationsCache;


#if NET9_0
        private readonly Dictionary<string, IEdmEntityContainer>.AlternateLookup<ReadOnlyMemory<char>> containersDictionaryReadOnlyMemoryLookup;
        private readonly Dictionary<string, IEdmSchemaType>.AlternateLookup<ReadOnlyMemory<char>> schemaTypeDictionaryReadOnlyMemoryLookup;
        private readonly Dictionary<string, IEdmTerm>.AlternateLookup<ReadOnlyMemory<char>> termDictionaryReadOnlyMemoryLookup;
        private readonly Dictionary<string, IList<IEdmOperation>>.AlternateLookup<ReadOnlyMemory<char>> functionDictionaryReadOnlyMemoryLookup;
        
        // ReadOnlySpan lookups are currently in preview
        private readonly Dictionary<string, IEdmEntityContainer>.AlternateLookup<ReadOnlySpan<char>> containersDictionaryReadOnlySpanLookup;
        private readonly Dictionary<string, IEdmSchemaType>.AlternateLookup<ReadOnlySpan<char>> schemaTypeDictionaryReadOnlySpanLookup;
        private readonly Dictionary<string, IEdmTerm>.AlternateLookup<ReadOnlySpan<char>> termDictionaryReadOnlySpanLookup;
        private readonly Dictionary<string, IList<IEdmOperation>>.AlternateLookup<ReadOnlySpan<char>> functionDictionaryReadOnlySpanLookup;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmModelBase"/> class.
        /// </summary>
        /// <param name="referencedModels">Models to which this model refers.</param>
        /// <param name="annotationsManager">Annotations manager for the model to use.</param>
        /// <remarks>Only either mainModel and referencedModels should have value.</remarks>
        protected EdmModelBase(IEnumerable<IEdmModel> referencedModels, IEdmDirectValueAnnotationsManager annotationsManager)
            : this(referencedModels, annotationsManager, true /*includeDefaultVocabularies*/)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmModelBase"/> class.
        /// </summary>
        /// <param name="referencedModels">Models to which this model refers.</param>
        /// <param name="annotationsManager">Annotations manager for the model to use.</param>
        /// <param name="includeDefaultVocabularies">a boolean value indicating whether to embed the built-in vocabulary models.</param>
        /// <remarks>Only either mainModel and referencedModels should have value.</remarks>
        protected EdmModelBase(IEnumerable<IEdmModel> referencedModels, IEdmDirectValueAnnotationsManager annotationsManager, bool includeDefaultVocabularies)
        {
            EdmUtil.CheckArgumentNull(referencedModels, "referencedModels");
            EdmUtil.CheckArgumentNull(annotationsManager, "annotationsManager");

            this.bindableOperationsCache = new ConcurrentDictionary<string, IList<IEdmOperation>>();
            this.referencedEdmModels = new List<IEdmModel>(referencedModels);

            // EdmCoreModel is always embedded.
            this.referencedEdmModels.Insert(0, EdmCoreModel.Instance);

            if (includeDefaultVocabularies)
            {
                this.referencedEdmModels.AddRange(VocabularyModelProvider.VocabularyModels);
            }

            this.annotationsManager = annotationsManager;

#if NET9_0
            this.containersDictionaryReadOnlyMemoryLookup = this.containersDictionary.GetAlternateLookup<ReadOnlyMemory<char>>();
            this.schemaTypeDictionaryReadOnlyMemoryLookup = this.schemaTypeDictionary.GetAlternateLookup<ReadOnlyMemory<char>>();
            this.termDictionaryReadOnlyMemoryLookup = this.termDictionary.GetAlternateLookup<ReadOnlyMemory<char>>();
            this.functionDictionaryReadOnlyMemoryLookup = this.functionDictionary.GetAlternateLookup<ReadOnlyMemory<char>>();

            // Note ReadOnlySpan is a preview only feature
            this.containersDictionaryReadOnlySpanLookup = this.containersDictionary.GetAlternateLookup<ReadOnlySpan<char>>();
            this.schemaTypeDictionaryReadOnlySpanLookup = this.schemaTypeDictionary.GetAlternateLookup<ReadOnlySpan<char>>();
            this.termDictionaryReadOnlySpanLookup = this.termDictionary.GetAlternateLookup<ReadOnlySpan<char>>();
            this.functionDictionaryReadOnlySpanLookup = this.functionDictionary.GetAlternateLookup<ReadOnlySpan<char>>();
#endif
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

#if NET9_0
        /// <summary>
        /// Searches for a type with the given name in this model only and returns null if no such type exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the type being found.</param>
        /// <returns>The requested type, or null if no such type exists.</returns>
        public IEdmSchemaType FindDeclaredType(ReadOnlyMemory<char> qualifiedName)
        {
            IEdmSchemaType result;
            this.schemaTypeDictionaryReadOnlyMemoryLookup.TryGetValue(qualifiedName, out result);
            return result;
        }

        /// <summary>
        /// Searches for a type with the given name in this model only and returns null if no such type exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the type being found.</param>
        /// <returns>The requested type, or null if no such type exists.</returns>
        [Experimental("ODataNet9PreviewFeatures")]
        public IEdmSchemaType FindDeclaredType(ReadOnlySpan<char> qualifiedName)
        {
            IEdmSchemaType result;
            this.schemaTypeDictionaryReadOnlySpanLookup.TryGetValue(qualifiedName, out result);
            return result;
        }
#endif

        /// <summary>
        /// Searches for a term with the given name in this model and returns null if no such term exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the term being found.</param>
        /// <returns>The requested term, or null if no such term exists.</returns>
        public IEdmTerm FindDeclaredTerm(string qualifiedName)
        {
            IEdmTerm result;
            this.termDictionary.TryGetValue(qualifiedName, out result);
            return result;
        }

#if NET9_0
        /// <summary>
        /// Searches for a term with the given name in this model and returns null if no such term exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the term being found.</param>
        /// <returns>The requested term, or null if no such term exists.</returns>
        public IEdmTerm FindDeclaredTerm(ReadOnlyMemory<char> qualifiedName)
        {
            IEdmTerm result;
            this.termDictionaryReadOnlyMemoryLookup.TryGetValue(qualifiedName, out result);
            return result;
        }


        /// <summary>
        /// Searches for a term with the given name in this model and returns null if no such term exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the term being found.</param>
        /// <returns>The requested term, or null if no such term exists.</returns>
        [Experimental("ODataNet9PreviewFeatures")]
        public IEdmTerm FindDeclaredTerm(ReadOnlySpan<char> qualifiedName)
        {
            IEdmTerm result;
            this.termDictionaryReadOnlySpanLookup.TryGetValue(qualifiedName, out result);
            return result;
        }
#endif

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

#if NET9_0
        /// <summary>
        /// Searches for a operation with the given name in this model and returns null if no such operation exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the operation being found.</param>
        /// <returns>A group of operations sharing the specified qualified name, or an empty enumerable if no such operation exists.</returns>
        public IEnumerable<IEdmOperation> FindDeclaredOperations(ReadOnlyMemory<char> qualifiedName)
        {
            IList<IEdmOperation> elements;
            if (this.functionDictionaryReadOnlyMemoryLookup.TryGetValue(qualifiedName, out elements))
            {
                return elements;
            }

            return Enumerable.Empty<IEdmOperation>();
        }

        /// <summary>
        /// Searches for a operation with the given name in this model and returns null if no such operation exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the operation being found.</param>
        /// <returns>A group of operations sharing the specified qualified name, or an empty enumerable if no such operation exists.</returns>
        [Experimental("ODataNet9PreviewFeatures")]
        public IEnumerable<IEdmOperation> FindDeclaredOperations(ReadOnlySpan<char> qualifiedName)
        {
            IList<IEdmOperation> elements;
            if (this.functionDictionaryReadOnlySpanLookup.TryGetValue(qualifiedName, out elements))
            {
                return elements;
            }

            return Enumerable.Empty<IEdmOperation>();
        }
#endif

        /// <summary>
        /// Searches for bound operations based on the binding type, returns an empty enumerable if no operation exists.
        /// </summary>
        /// <param name="bindingType">Type of the binding.</param>
        /// <returns> A set of operations that share the binding type or empty enumerable if no such operation exists. </returns>
        public virtual IEnumerable<IEdmOperation> FindDeclaredBoundOperations(IEdmType bindingType)
        {
            IList<IEdmOperation> bindableOperations;

            string bindingTypeName = bindingType.FullTypeName();

            if (!this.bindableOperationsCache.TryGetValue(bindingTypeName, out bindableOperations))
            {
                HashSet<IList<IEdmOperation>> operationList = new HashSet<IList<IEdmOperation>>();
                bindableOperations = new List<IEdmOperation>();

                foreach (IList<IEdmOperation> operations in this.functionDictionary.Values)
                {
                    //To ensure the Listof operations is distinct, as the same list of operations could be there for another qualified name in dictionary
                    if (operationList.Add(operations))
                    {
                        for (int i = 0; i < operations.Count; i++)
                        {
                            if (operations[i].HasEquivalentBindingType(bindingType))
                            {
                                bindableOperations.Add(operations[i]);
                            }
                        }
                    }
                }

                this.bindableOperationsCache.TryAdd(bindingTypeName, bindableOperations);
            }
            
            return bindableOperations;
        }

        /// <summary>
        /// Searches for bound operations based on the qualified name and binding type, returns an empty enumerable if no operation exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the operation.</param>
        /// <param name="bindingType">Type of the binding.</param>
        /// <returns>
        /// A set of operations that share the name and binding type or empty enumerable if no such operation exists.
        /// </returns>
        public virtual IEnumerable<IEdmOperation> FindDeclaredBoundOperations(string qualifiedName, IEdmType bindingType)
        {
            IEnumerable<IEdmOperation> enumerable = this.FindDeclaredBoundOperations(bindingType);  

            if(enumerable == null)
            {
                return Enumerable.Empty<IEdmOperation>();
            }

            IList<IEdmOperation> matchedOperations = new List<IEdmOperation>();

            IList<IEdmOperation> operations = enumerable as IList<IEdmOperation>;

            if (operations != null)
            {
                for (int i = 0; i < operations.Count; i++)
                {
                    if (string.Equals(operations[i].FullName(), qualifiedName, System.StringComparison.Ordinal))
                    {
                        matchedOperations.Add(operations[i]);
                    }
                }
            }
            else
            {
                foreach(IEdmOperation operation in enumerable)
                {
                    if (string.Equals(operation.FullName(), qualifiedName, System.StringComparison.Ordinal))
                    {
                        matchedOperations.Add(operation);
                    }
                }
            }

            return matchedOperations;
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
            RegistrationHelper.RegisterSchemaElement(element, this.schemaTypeDictionary, this.termDictionary, this.functionDictionary, this.containersDictionary);
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

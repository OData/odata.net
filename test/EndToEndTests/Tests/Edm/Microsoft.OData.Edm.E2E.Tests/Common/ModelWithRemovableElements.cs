//---------------------------------------------------------------------
// <copyright file="ModelWithRemovableElements.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.E2E.Tests.Common;

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
        get { return model; }
    }

    public IEnumerable<IEdmSchemaElement> SchemaElements
    {
        get
        {
            return model.SchemaElements.Except(removedElements.Keys);
        }
    }

    public IEnumerable<string> DeclaredNamespaces
    {
        get { return SchemaElements.Select(s => s.Namespace).Distinct(); }
    }

    public IEnumerable<IEdmVocabularyAnnotation> VocabularyAnnotations
    {
        get
        {
            return model.VocabularyAnnotations.Except(removedVocabularyAnnotations.Keys);
        }
    }

    public IEnumerable<IEdmModel> ReferencedModels
    {
        get { return model.ReferencedModels.Except(removeReferencedModels.Keys); }
    }

    public IEdmDirectValueAnnotationsManager DirectValueAnnotationsManager
    {
        get { return model.DirectValueAnnotationsManager; }
    }

    public IEdmEntityContainer EntityContainer
    {
        get
        {
            if (model.EntityContainer == null)
            {
                return null;
            }

            if (removedElements.ContainsKey(model.EntityContainer))
            {
                return null;
            }

            return model.EntityContainer;
        }
    }

    /// <summary>
    /// Searches for a type with the given name in this model only and returns null if no such type exists.
    /// </summary>
    /// <param name="qualifiedName">The qualified name of the type being found.</param>
    /// <returns>The requested type, or null if no such type exists.</returns>
    public IEdmSchemaType FindDeclaredType(string qualifiedName)
    {
        IEdmSchemaType type = model.FindDeclaredType(qualifiedName);
        return type != null && removedElements.ContainsKey(type) ? null : type;
    }

    public IEnumerable<IEdmOperation> FindDeclaredOperations(string qualifiedName)
    {
        IEnumerable<IEdmOperation> functions = model.FindDeclaredOperations(qualifiedName);
        return functions.Except(removedElements.Keys.Where(e => e.SchemaElementKind == EdmSchemaElementKind.Action || e.SchemaElementKind == EdmSchemaElementKind.Function).Cast<IEdmOperation>());
    }

    public IEnumerable<IEdmOperation> FindDeclaredBoundOperations(IEdmType bindingType)
    {
        IEnumerable<IEdmOperation> functions = model.FindDeclaredBoundOperations(bindingType);
        return functions.Except(removedElements.Keys.Where(e => e.SchemaElementKind == EdmSchemaElementKind.Action || e.SchemaElementKind == EdmSchemaElementKind.Function).Cast<IEdmOperation>());
    }

    public virtual IEnumerable<IEdmOperation> FindDeclaredBoundOperations(string qualifiedName, IEdmType bindingType)
    {
        return FindDeclaredOperations(qualifiedName).Where(o => o.IsBound && o.Parameters.Any() && o.HasEquivalentBindingType(bindingType));
    }

    public IEdmTerm FindDeclaredTerm(string qualifiedName)
    {
        IEdmTerm term = model.FindDeclaredTerm(qualifiedName);
        return term != null && removedElements.ContainsKey(term) ? null : term;
    }

    public IEnumerable<IEdmVocabularyAnnotation> FindDeclaredVocabularyAnnotations(IEdmVocabularyAnnotatable element)
    {
        IEnumerable<IEdmVocabularyAnnotation> annotations = model.FindDeclaredVocabularyAnnotations(element);
        return annotations.Except(removedVocabularyAnnotations.Keys);
    }

    public IEnumerable<IEdmStructuredType> FindDirectlyDerivedTypes(IEdmStructuredType baseType)
    {
        throw new NotImplementedException("Find derived types is not implemented");
    }

    internal void RemoveElement(IEdmSchemaElement element)
    {
        removedElements[element] = true;
    }

    internal void RemoveVocabularyAnnotation(IEdmVocabularyAnnotation annotation)
    {
        removedVocabularyAnnotations[annotation] = true;
    }

    internal void RemoveReference(IEdmModel reference)
    {
        removeReferencedModels[reference] = true;
        foreach (var tmp in reference.SchemaElements)
        {
            removedElements[tmp] = true;
        }
    }
}

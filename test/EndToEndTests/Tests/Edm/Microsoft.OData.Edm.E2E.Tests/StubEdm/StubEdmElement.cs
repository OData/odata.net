﻿//---------------------------------------------------------------------
// <copyright file="StubEdmElement.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.E2E.Tests.StubEdm;

/// <summary>
/// Stub implementation of EdmElement
/// </summary>
public abstract class StubEdmElement : IEdmElement, IEnumerable
{
    private List<IEdmVocabularyAnnotation> vocabularyAnnotations = new List<IEdmVocabularyAnnotation>();

    /// <summary>
    /// Gets the vocabulary annotations
    /// </summary>
    public IEnumerable<IEdmVocabularyAnnotation> InlineVocabularyAnnotations
    {
        get { return this.vocabularyAnnotations; }
    }

    /// <summary>
    /// Gets an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>the enumerator</returns>
    public IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Add an vocabulary annotation
    /// </summary>
    /// <param name="annotation">the annotation</param>
    public void AddVocabularyAnnotation(IEdmVocabularyAnnotation annotation)
    {
        // question: how do you get the annotation? Namespace.Name, or qualifier.NamespaceUri.Name?
        this.vocabularyAnnotations.Add(annotation);
    }

    /// <summary>
    /// Removes annotation for Term
    /// </summary>
    /// <param name="term">The specified term</param>
    public void RemoveAnnotationsForTerm(IEdmTerm term)
    {
        var found = this.vocabularyAnnotations.Where(a => a.Term.Equals(term)).ToArray();
        foreach (var a in found)
        {
            this.vocabularyAnnotations.Remove(a);
        }
    }
}

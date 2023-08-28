//---------------------------------------------------------------------
// <copyright file="VocabularyAnnotationCache.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Cache for storing a model's vocabulary annotations for quicker retrieval
    /// </summary>
    internal class VocabularyAnnotationCache
    {
        private readonly ConcurrentDictionary<IEdmVocabularyAnnotatable, IEnumerable<IEdmVocabularyAnnotation>> annotationsCache =
            new ConcurrentDictionary<IEdmVocabularyAnnotatable, IEnumerable<IEdmVocabularyAnnotation>>(new VocabularyAnnotatableComparer());

        /// <summary>
        /// Adds vocabulary annotations for the specified <paramref name="element"/> to the cache.
        /// </summary>
        /// <param name="element">The element for which to cache annotations.</param>
        /// <param name="annotations">The vocabulary annotations to cache</param>
        public void AddVocabularyAnnotations(IEdmVocabularyAnnotatable element, IEnumerable<IEdmVocabularyAnnotation> annotations)
        {
            // since the cache is used with immutable models, we don't
            // bother updating the annotations if the key already exists
            annotationsCache.TryAdd(element, annotations);
        }
        
        /// <summary>
        /// Retrieves vocabulary annotations for the specified <paramref name="element"/> from the
        /// cache if they exist in the cache.
        /// </summary>
        /// <param name="element">The element for which to retrieve the annotations.</param>
        /// <param name="annotations">The vocabulary annotations retrieved from the cache.</param>
        /// <returns>True if the element's annotations were found in the cache, false otherwise.</returns>
        public bool TryGetVocabularyAnnotations(IEdmVocabularyAnnotatable element, out IEnumerable<IEdmVocabularyAnnotation> annotations)
        {
            return annotationsCache.TryGetValue(element, out annotations);
        }
    }

    internal class VocabularyAnnotatableComparer : IEqualityComparer<IEdmVocabularyAnnotatable>
    {
        public bool Equals(IEdmVocabularyAnnotatable x, IEdmVocabularyAnnotatable y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (x is IEdmPathExpression && y is IEdmPathExpression)
            {
                return EdmUtil.FullyQualifiedName(x) == EdmUtil.FullyQualifiedName(y);
            }

            // We should use full qualified name to compare, but the original codes uses the reference equals, let me keep it unchanged for back-compatibility.
            return object.ReferenceEquals(x, y);
        }

        public int GetHashCode(IEdmVocabularyAnnotatable obj)
        {
            return obj.GetHashCode();
        }
    }
}

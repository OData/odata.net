using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Cache for storing a model's vocabulary annotations for quicker retrieval
    /// </summary>
    internal class VocabularyAnnotationCache
    {
        private readonly ConcurrentDictionary<IEdmVocabularyAnnotatable, IEnumerable<IEdmVocabularyAnnotation>> annotationsCache =
            new ConcurrentDictionary<IEdmVocabularyAnnotatable, IEnumerable<IEdmVocabularyAnnotation>>();

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
}

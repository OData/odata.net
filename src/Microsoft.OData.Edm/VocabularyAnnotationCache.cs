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

        public void AddVocabularyAnnotations(IEdmVocabularyAnnotatable element, IEnumerable<IEdmVocabularyAnnotation> annotations)
        {
            annotationsCache[element] = annotations;
        }
        
        public bool TryGetVocabularyAnnotations(IEdmVocabularyAnnotatable element, out IEnumerable<IEdmVocabularyAnnotation> annotations)
        {
            return annotationsCache.TryGetValue(element, out annotations);
        }
    }
}

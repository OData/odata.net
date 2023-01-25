using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Cache used to store schema elements using case-normalized names
    /// to speed up case-insensitive model lookups.
    /// </summary>
    internal class CaseInsensitiveSchemaElementsCache
    {
        private static readonly List<IEdmSchemaElement> emptyList = new List<IEdmSchemaElement>();
        // This cache is meant to be populate up front and remain read-only
        // after that. Therefore, it doesn't need
        // to be a concurrent dictionary.
        private Dictionary<string, List<IEdmSchemaElement>> cache;

        public void PopulateCache(IEdmModel model)
        {
            Dictionary<string, List<IEdmSchemaElement>> cache = new Dictionary<string, List<IEdmSchemaElement>>();

            PopulateSchemaElements(model, cache);

            foreach (IEdmModel referencedModel in model.ReferencedModels)
            {
                PopulateSchemaElements(referencedModel, cache);
            }

            this.cache = cache;
        }

        public List<IEdmSchemaElement> FindElement(string qualifiedName)
        {
            if (cache.TryGetValue(qualifiedName.ToUpperInvariant(), out List<IEdmSchemaElement> results))
            {
                return results;
            }

            return emptyList;
        }

        public T FindSingle<T>(string qualifiedName, Func<object, string> duplicateErrorFunc) where T : IEdmSchemaElement
        {
            IReadOnlyList<IEdmSchemaElement> results = FindElement(qualifiedName);

            if (results.Count == 0)
            {
                return default(T);
            }

            if (results.Count > 1)
            {
                throw new ODataException(duplicateErrorFunc(qualifiedName));
            }

            return (T)results[0];
        }

        private static void PopulateSchemaElements(IEdmModel model, Dictionary<string, List<IEdmSchemaElement>> cache)
        {
            foreach (IEdmSchemaElement element in model.SchemaElements)
            {
                string normalizedKey = element.FullName().ToUpperInvariant();
                List<IEdmSchemaElement> results;
                if (!cache.TryGetValue(normalizedKey, out results))
                {
                    results = new List<IEdmSchemaElement>();
                    cache[normalizedKey] = results;
                }

                results.Add(element);
            }
        }
    }
}

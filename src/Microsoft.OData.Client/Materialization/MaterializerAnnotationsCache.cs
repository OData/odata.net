using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.OData.Client.Materialization
{
    /// <summary>
    /// A cache used to store temporary materialization metadata
    /// for deseriliazed <see cref="ODataItem"/>s during materialization of response payloads
    /// into client CLR objects. Keys are identified using reference equality.
    /// </summary>
    /// <remarks>
    /// Instances of the cache are not thread-safe. Each instance is expected to be used within the
    /// scope of a single request.
    /// 
    /// The cache is designed to store one entry type per key. For example, if the key is
    /// an <see cref="ODataResource"/>, its value should be a <see cref="MaterializerEntry"/>. It also
    /// expects the value for any given key will not change.
    /// So you should call `cache.SetAnnotation(odataResource, materializerEntry)` add the cache entry
    /// and `cache.GetAnnotations<MaterializerEntry>(odataResource)` to retrieve the entry.
    /// 
    /// If the requirements change such that we need to allow updating the value of a key to a different type,
    /// then the cache should be redesigned.
    /// </remarks>
    internal sealed class MaterializerAnnotationsCache
    {
        private readonly Dictionary<ODataAnnotatable, object> cache = new Dictionary<ODataAnnotatable, object>(ReferenceEqualityComparer<ODataAnnotatable>.Instance);

        /// <summary>
        /// Adds an entry to the cache with the <paramref name="annotatable"/>
        /// set as key,
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="annotatable">The key of the entry.</param>
        /// <param name="value">The value to associate with the key.</param>
        public void SetAnnotation<T>(ODataAnnotatable annotatable, T value) where T : class
        {
            Debug.Assert(annotatable != null, "annotatable != null");
            this.cache.Add(annotatable, value);
        }

        /// <summary>
        /// Retrieves the value associated with the specified <paramref name="annotatable"/>.
        /// Returns null if the cache does not contain an entry with the specifiedy key.
        /// </summary>
        /// <typeparam name="T">The expected type of the value.</typeparam>
        /// <param name="annotatable"></param>
        /// <returns>The value associated with the specified <paramref name="annotatable"/> if the entry exists, or null otherwise.</returns>
        public T GetAnnotation<T>(ODataAnnotatable annotatable) where T: class
        {
            if (this.cache.TryGetValue(annotatable, out object value))
            {
                T valueAsT = value as T;
                Debug.Assert(valueAsT != null, "valueAsT != null");
                return valueAsT;
            }

            return default(T);
        }
    }

    internal static class MaterializerContextExtensions
    {
        /// <summary>
        /// Associates the specified annotation <paramref name="value"/> with the specified
        /// <paramref name="annotatable"/> to store metadata used for materialization.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="context">The materializer context.</param>
        /// <param name="annotatable">The item to annotate.</param>
        /// <param name="value">The annotation value.</param>
        public static void SetAnnotation<T>(this IODataMaterializerContext context, ODataAnnotatable annotatable, T value) where T : class
        {
            context.AnnotationsCache.SetAnnotation(annotatable, value);
        }

        /// <summary>
        /// Retrieves the annotation value associated with te specified <paramref name="annotatable"/>.
        /// </summary>
        /// <typeparam name="T">The expected type of the annotation value.</typeparam>
        /// <param name="context">The materializer context.</param>
        /// <param name="annotatable">The item for which to retrieve the annotation.</param>
        /// <returns>The annotation value associated with the <paramref name="annotatable"/> if it exists, or null otherwise.</returns>
        public static T GetAnnotation<T>(this IODataMaterializerContext context, ODataAnnotatable annotatable) where T : class
        {
            return context.AnnotationsCache.GetAnnotation<T>(annotatable);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Client.Materialization
{
    internal class MaterializerAnnotationsCache
    {
        private readonly Dictionary<ODataAnnotatable, object> cache = new Dictionary<ODataAnnotatable, object>(ReferenceEqualityComparer<ODataAnnotatable>.Instance);

        public void SetAnnotation<T>(ODataAnnotatable annotatable, T value) where T : class
        {
            this.cache.Add(annotatable, value);
        }

        public T GetAnnotation<T>(ODataAnnotatable annotatable) where T: class
        {
            if (this.cache.TryGetValue(annotatable, out object value))
            {
                Debug.Assert(value is T);
                return value as T;
            }

            return default(T);
        }
    }

    internal static class MaterializerContextExtensions
    {
        public static void SetAnnotation<T>(this IODataMaterializerContext context, ODataAnnotatable annotatable, T value) where T : class
        {
            context.AnnotationsCache.SetAnnotation(annotatable, value);
        }

        public static T GetAnnotation<T>(this IODataMaterializerContext context, ODataAnnotatable annotatable) where T : class
        {
            return context.AnnotationsCache.GetAnnotation<T>(annotatable);
        }
    }
}

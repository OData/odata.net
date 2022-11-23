namespace Microsoft.Spatial
{
    using System.Linq;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Extension methods for <see cref="IOrderedQueryable{T}"/>
    /// </summary>
    /// <threadsafety static="true"/>
    public static class OrderedQueryableExtensions
    {
        /// <summary>
        /// Returns <paramref name="queryable"/> typed as <see cref="IOrderedQueryable{T}"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="queryable"/></typeparam>
        /// <param name="queryable">The queryable to type as <see cref="IOrderedQueryable{T}"/></param>
        /// <returns>The input queryable typed as <see cref="IOrderedQueryable{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IOrderedQueryable<T> AsOrderedQueryable<T>(this IOrderedQueryable<T> queryable)
        {
            return queryable;
        }
    }
}

namespace Microsoft.Spatial
{
    using System.Linq;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Extension methods for <see cref="IQueryable{T}"/>
    /// </summary>
    /// <threadsafety static="true"/>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Returns <paramref name="queryable"/> typed as <see cref="IQueryable{T}"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="queryable"/></typeparam>
        /// <param name="queryable">The queryable to type as <see cref="IQueryable{T}"/></param>
        /// <returns>The input queryable typed as <see cref="IQueryable{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IQueryable<T> AsQueryable<T>(this IQueryable<T> queryable)
        {
            return queryable;
        }
    }
}

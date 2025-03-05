namespace Microsoft.OData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Extension methods for <see cref="IQueryable{T}"/>.
    /// </summary>
    public static class QueryableExtensions
    {
        public static double Average(this IQueryable<short> source)
        {
            //// we return double because of this: https://docs.oasis-open.org/odata/odata-data-aggregation-ext/v4.0/cs03/odata-data-aggregation-ext-v4.0-cs03.html#StandardAggregationMethodaverage
            ArgumentNullException.ThrowIfNull(source);

            return source.Provider.Execute<double>(
                Expression.Call(
                    null,
                    new Func<IQueryable<short>, double>(QueryableExtensions.Average).Method,
                    source.Expression));
        }

        private static double Average(IEnumerable<short> source)
        {
            return source.Average();
        }

        public static double? Average(this IQueryable<short?> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            return source.Provider.Execute<double?>(
                Expression.Call(
                    null,
                    new Func<IQueryable<short?>, double?>(Average).Method,
                    source.Expression));
        }

        private static double? Average(IEnumerable<short?> source)
        {
            return source.Average();
        }
    }
}

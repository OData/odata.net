namespace Microsoft.OData.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public static class QueryableExtensions
    {
        public static double Average(this IQueryable<short> source)
        {
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

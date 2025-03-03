using System.Collections.Generic;
using System;

namespace Microsoft.OData.Core
{
    public static class EnumerableExtensions
    {
        public static double Average(this IEnumerable<short> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    throw new Exception("TODO");
                }

                long sum = enumerator.Current;
                long count = 1;
                while (enumerator.MoveNext())
                {
                    checked
                    {
                        sum += enumerator.Current;
                    }

                    ++count;
                }

                return (double)sum / count;
            }
        }
    }
}

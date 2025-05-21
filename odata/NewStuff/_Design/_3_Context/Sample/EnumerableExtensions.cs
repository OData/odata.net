namespace NewStuff._Design._3_Context.Sample
{
    using System.Collections.Generic;

    public delegate bool Try<TInput, TOutput>(TInput input, out TOutput output);

    public static class EnumerableExtensions
    {
        public static IEnumerable<TResult> TrySelect<TSource, TResult>(this IEnumerable<TSource> source, Try<TSource, TResult> @try)
        {
            foreach (var element in source)
            {
                if (@try(element, out var result))
                {
                    yield return result;
                }
            }
        }
    }
}





static class EnumerableExtensions
{
    public static IEnumerable<(T, FirstLast)> WithFirstLast<T>(this IEnumerable<T> items)
    {
        var enumerator = items.GetEnumerator();
        if (!enumerator.MoveNext()) { yield break; };
        var state = FirstLast.First;
        var current = enumerator.Current;
        while (enumerator.MoveNext())
        {
            yield return (current, state);
            state &= ~FirstLast.First; // not the first anymore since MoveNext succeeded twice
            current = enumerator.Current;
        }
        state |= FirstLast.Last; // add the Last flag
        yield return (current, state);
    }

    public static IEnumerable<(T, bool)> WithLast<T>(this IEnumerable<T> items)
    {
        var enumerator = items.GetEnumerator();
        if (!enumerator.MoveNext()) { yield break; };
        var isLast = false;
        var current = enumerator.Current;
        while (enumerator.MoveNext())
        {
            yield return (current, isLast);
            current = enumerator.Current;
        }
        isLast = true; // add the isLast flag
        yield return (current, isLast);
    }

    public static bool TryGetSingle<T>(this IEnumerable<T> items, [MaybeNullWhen(false)] out T single)
    {
        using var enumerator = items.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            single = default;
            return false;
        }
        single = enumerator.Current;
        return !enumerator.MoveNext();
    }
}



[Flags]
enum FirstLast { None = 0, First = 1, Last = 2 }

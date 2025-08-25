namespace Odata
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    public interface IReader<out TNext>
    {
        ValueTask Read();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moved"><see langword="false"/> if <see cref="IReader{TNext}.Read"/> needs to be called first</param>
        /// <returns></returns>
        TNext TryMoveNext(out bool moved);
    }

    public interface IReader<out TValue, out TNext> : IReader<TNext>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="moved"><see langword="false"/> if <see cref="IReader{TNext}.Read"/> needs to be called first</param>
        /// <returns></returns>
        TValue TryGetValue(out bool moved);
    }

    public static class ReaderExtensions
    {
        public static bool TryMoveNext2<TNext>(this IReader<TNext> reader, [MaybeNullWhen(false)] out TNext next)
        {
            next = reader.TryMoveNext(out var moved);
            return moved;
        }
        public static bool TryGetValue2<TValue, TNext>(this IReader<TValue, TNext> reader, [MaybeNullWhen(false)] out TValue value)
        {
            value = reader.TryGetValue(out var moved);
            return moved;
        }
    }
}

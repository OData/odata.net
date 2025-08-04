namespace NewStuff._Design._0_Convention.V3
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    public interface IReader<out TNext> where TNext : allows ref struct
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
        where TValue : allows ref struct
        where TNext : allows ref struct
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
        public static bool TryMoveNext2<TNext>(this IReader<TNext> reader, out TNext next)
        {
            next = reader.TryMoveNext(out var moved);
            return moved;
        }
    }
}

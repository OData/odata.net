namespace System
{
    /// <summary>
    /// Extension methods for <see cref="System.Array"/>
    /// </summary>
    /// <threadsafety static="true"/>
    public static class ArrayExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <param name="default"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static T ElementAtOrDefault<T>(this T[]? source, int index, T @default)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (index >= source.Length)
            {
                return @default;
            }

            return source[index];
        }
    }
}

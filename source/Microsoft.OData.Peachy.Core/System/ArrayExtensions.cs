namespace System
{
    /// <summary>
    /// Extension methods for <see cref="System.Array"/>
    /// </summary>
    public static class ArrayExtensions
    {
        public static T ElementAtOrDefault<T>(this T[]? source, int index, T @default)
        {
            if (index >= source.Length)
            {
                return @default;
            }

            return source[index];
        }
    }
}

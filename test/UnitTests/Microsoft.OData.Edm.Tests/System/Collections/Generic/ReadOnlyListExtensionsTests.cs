namespace System.Collections.Generic
{
    using Xunit;

    /// <summary>
    /// Extensions methods <see cref="IReadOnlyList{T}"/>
    /// </summary>
    public sealed class ReadOnlyListExtensionsTests
    {
        /// <summary>
        /// Copied from <see href="https://github.com/dotnet/runtime/blob/87c25589bda5a79baf8d056501663b8525f366a8/src/libraries/System.Runtime/tests/System/ArrayTests.cs#L2040"/>
        /// </summary>
        [Fact]
        public void FindLastIndex()
        {
            var intArray = new int[] { 40, 41, 42, 43, 44, 45, 46, 47, 48, 49 };
            Assert.Equal(9, intArray.FindLastIndex(i => i >= 43));
            Assert.Equal(-1, intArray.FindLastIndex(i => i == 99));

            intArray = new int[0];
            Assert.Equal(-1, intArray.FindLastIndex(i => i == 43));
        }
    }
}

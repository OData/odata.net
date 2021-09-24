using Microsoft.OData.Buffers;
using System.Buffers;

namespace SerializationBaselineTests
{
    public class CharArrayPool : ICharArrayPool
    {
        public char[] Rent(int minSize)
        {
            return ArrayPool<char>.Shared.Rent(minSize);
        }

        public void Return(char[] array)
        {
            ArrayPool<char>.Shared.Return(array);
        }

        static private ICharArrayPool _shared = new CharArrayPool();

        public static ICharArrayPool Shared { get { return _shared; } }
    }
}

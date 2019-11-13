using Microsoft.OData.Buffers;

namespace Microsoft.OData.Tests
{
    public class TestCharArrayPool : ICharArrayPool
    {
        public char[] Buffer { get { return Rent(MinSize); } }
        public int MinSize { set; get; }
        public TestCharArrayPool(int minSize)
        {
            this.MinSize = minSize;
        }
        public char[] Rent(int minSize)
        {
            return new char[minSize];
        }

        public void Return(char[] array)
        {

        }
    }

}
using Sprache;
using System.Numerics;

namespace CombinatorParsingV2
{
    public sealed class StringInput : IInput<char>
    {
        private readonly string input;

        private readonly int index;

        public StringInput(string input)
            : this(input, 0)
        {
        }

        private StringInput(string input, int index)
        {
            this.input = input;
            this.index = index;
        }

        public char Current
        {
            get
            {
                return this.input[this.index];
            }
        }

        public IInput<char> Next()
        {
            return new StringInput(this.input, this.index + 1);
        }
    }
}

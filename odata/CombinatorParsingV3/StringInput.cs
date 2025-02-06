using Sprache;
using System;
using System.Numerics;

namespace CombinatorParsingV3
{
    /// <summary>
    /// TODO there seem to be a number of options here:
    /// 1. class
    /// 2. sealed class
    /// 3. struct
    /// 4. ref struct (using a new ref struct version of nullable)
    /// 5. explicit interface implementation of `next()`
    /// 6. ref struct with a "trynext" instead of returning a nullable
    /// 
    /// 5 suprisingly has impact, the most out of 1-5. But 6 has the absolute most impact, barely adding any overhead to not traversing the input string at all.
    /// 
    /// TODO it might be good to create a repro of the 10 different options
    /// </summary>
    public ref struct StringInput : IInput<char, StringInput>
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
                return this.input[this.index]; //// TODO just record this once in the constructor?
            }
        }

        public StringInput Next(out bool success)
        {
            if (this.index == this.input.Length - 1)
            {
                success = false;
                return default;
            }

            success = true;
            return new StringInput(this.input, this.index + 1);
        }
    }
}

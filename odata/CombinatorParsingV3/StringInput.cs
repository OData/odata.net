using Sprache;
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
    /// TODO create a repro for all of these options (across both dimensions of type category + try vs non-try)
    /// </summary>
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
                return this.input[this.index]; //// TODO just record this once in the constructor?
            }
        }

        IInput<char>? IInput<char>.Next()
        {
            if (this.index == this.input.Length - 1)
            {
                return null;
            }

            return new StringInput(this.input, this.index + 1);
        }

        public bool Next(out StringInput output)
        {
            if (this.index == this.input.Length - 1)
            {
                output = default;
                return false;
            }

            output = new StringInput(this.input, this.index + 1);
            return true;
        }

        public StringInput? Next()
        {
            if (this.index == this.input.Length - 1)
            {
                return null;
            }

            return new StringInput(this.input, this.index + 1);
        }
    }

    public ref struct StringInput2
    {
        private readonly string input;

        private readonly int index;

        public StringInput2(string input)
            : this(input, 0)
        {
        }

        private StringInput2(string input, int index)
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

        public bool Next(out StringInput2 output)
        {
            if (this.index == this.input.Length - 1)
            {
                output = default;
                return false;
            }

            output = new StringInput2(this.input, this.index + 1);
            return true;
        }

        public RefNullable<StringInput2> Next()
        {
            if (this.index == this.input.Length - 1)
            {
                return new RefNullable<StringInput2>();
            }

            return new RefNullable<StringInput2>(new StringInput2(this.input, this.index + 1));
        }
    }

    public ref struct RefNullable<T> where T : allows ref struct
    {
        private readonly T value;
        private bool hasValue;

        public RefNullable()
        {
            this.hasValue = false;
        }

        public RefNullable(T value)
        {
            this.value = value;
            this.hasValue = true;
        }

        public bool HasValue => this.hasValue;

        public T Value
        {
            get
            {
                if (!this.hasValue)
                {
                    throw new System.Exception("TODO");
                }

                return this.value;
            }
        }
    }
}

using __GeneratedOdata.Parsers.Rules;
using CombinatorParsingV1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Text.RegularExpressions;

namespace CombinatorParsingV3
{
    public interface IOutput<out TToken, out TOutput>
    {
        bool Success { get; }

        TOutput Parsed { get; }

        IInput<TToken>? Remainder { get; }
    }

    public sealed class Output<TToken, TOutput> : IOutput<TToken, TOutput>
    {
        public Output(bool success, TOutput parsed, IInput<TToken>? remainder)
        {
            Success = success;
            Parsed = parsed;
            Remainder = remainder;
        }

        public bool Success { get; }

        public TOutput Parsed { get; }

        public IInput<TToken>? Remainder { get; }
    }

    public interface IInput<out TToken>
    {
        TToken Current { get; }

        IInput<TToken> Next(); //// TODO oops, this should be nullable
    }

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
            var newIndex = this.index + 1;
            if (newIndex >= this.input.Length)
            {
                return null;
            }

            return new StringInput(this.input, newIndex);
        }
    }

    public interface IInput2<TToken, TInput> where TToken : allows ref struct where TInput : IInput2<TToken, TInput>, allows ref struct
    {
        bool IsDone { get; }

        TToken Current { get; }

        TInput Next();
    }

    public readonly ref struct RefInput : IInput2<char, RefInput>
    {
        private readonly string input;

        private readonly int index;

        public RefInput(string input)
            : this(input, 0)
        {
        }

        private RefInput(string input, int index)
        {
            this.input = input;

            this.index = index;
        }

        public bool IsDone
        {
            get
            {
                return this.index >= this.input.Length;
            }
        }

        public char Current
        {
            get
            {
                return this.input[this.index];
            }
        }

        public RefInput Next()
        {
            if (this.index + 1 >= this.input.Length)
            {
                return default;
            }

            return new RefInput(this.input, this.index + 1);
        }
    }

}

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

    public interface IOutput2<TToken, TParsed, TInput> 
        where TToken : allows ref struct 
        where TInput : IInput2<TToken, TInput>, allows ref struct
    {
        bool Success { get; }

        TParsed Parsed { get; }

        TInput Remainder { get; }
    }

    public readonly ref struct RefOutput<TParsed> : IOutput2<char, TParsed, RefInput>
    {
        public RefOutput(bool success, TParsed parsed, RefInput remainder)
        {
            Success = success;
            Parsed = parsed;
            Remainder = remainder;
        }

        public bool Success { get; }

        public TParsed Parsed { get; }

        public RefInput Remainder { get; }
    }

    public interface IParser2<TToken, TInput, TParsed, TOutput>
        where TToken : allows ref struct
        where TInput : IInput2<TToken, TInput>, allows ref struct
        where TOutput : IOutput2<TToken, TParsed, TInput>, allows ref struct
    {
        TOutput Parse(TInput input);
    }

    public static class Parse2
    {
        public static IParser2<char, RefInput, char, RefOutput<char>> Char(char @char)
        {
            return new CharParser(@char);
        }

        private sealed class CharParser : IParser2<char, RefInput, char, RefOutput<char>>
        {
            private readonly char @char;

            public CharParser(char @char)
            {
                this.@char = @char;
            }

            public RefOutput<char> Parse(RefInput input)
            {
                if (input.Current == this.@char)
                {
                    return new RefOutput<char>(true, this.@char, input.Next());
                }

                return new RefOutput<char>(false, default, input);
            }
        }

        public static IParser2<char, RefInput, IEnumerable<TParsed>, RefOutput<IEnumerable<TParsed>>> Many<TParsed>(this IParser2<char, RefInput, TParsed, RefOutput<TParsed>> parser)
        {
            return new ManyParser<TParsed>(parser);
        }

        private sealed class ManyParser<TParsed> : IParser2<char, RefInput, IEnumerable<TParsed>, RefOutput<IEnumerable<TParsed>>>
        {
            private readonly IParser2<char, RefInput, TParsed, RefOutput<TParsed>> parser;

            public ManyParser(IParser2<char, RefInput, TParsed, RefOutput<TParsed>> parser)
            {
                this.parser = parser;
            }

            public RefOutput<IEnumerable<TParsed>> Parse(RefInput input)
            {
                //// TODO congratulations, you've again hit the issue: the caller needs to know if the parse was successful before you tell them the parsed values, which means that you need the full enumerable before you can return
                return default;
            }
        }
    }

    public readonly ref struct RefEnumerable<T> : IEnumerable<T> where T : allows ref struct
    {
        public IEnumerator<T> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}

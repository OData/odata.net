namespace CombinatorParsing
{
    using __GeneratedOdata.Parsers.Rules;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    //// TODO add covariance and contravariance where able

    public interface IParser<TInput, TToken, TOutput, TParsed, TParser> where TInput : IInput<TToken, TInput>, allows ref struct where TOutput : IOutput<TParsed, TToken, TInput>, allows ref struct where TToken : allows ref struct where TParsed : allows ref struct where TParser : IParser<TInput, TToken, TOutput, TParsed, TParser>, allows ref struct
    {
        TOutput Parse(TInput input);
    }

    public interface IInput<TToken, TInput> where TInput : IInput<TToken, TInput>, allows ref struct where TToken : allows ref struct
    {
        TToken Current { get; }

        TInput Next();
    }

    public readonly ref struct Input<TToken> : IInput<TToken, Input<TToken>> //// TODO make ttoken allow ref struct
    {
        private readonly IReadOnlyList<TToken> source;

        private readonly int index;

        public Input()
        {
            throw new System.InvalidOperationException("TODO actually invent your own custom exception type for this");
        }

        public Input(IReadOnlyList<TToken> source)
            : this(source, 0)
        {
        }

        private Input(IReadOnlyList<TToken> source, int index)
        {
            this.source = source;

            this.index = index;
        }

        public TToken Current
        {
            get
            {
                return this.source[this.index];
            }
        }

        public Input<TToken> Next()
        {
            return new Input<TToken>(this.source, this.index + 1);
        }
    }

    public interface IOutput<TParsed, TToken, TInput> where TInput : IInput<TToken, TInput>, allows ref struct where TParsed : allows ref struct where TToken : allows ref struct
    {
        bool Success { get; } //// TODO use `either`

        TParsed Parsed { get; }

        TInput Remainder { get; }
    }

    /// <summary>
    /// TODO maybe just `struct`? not really sure the implications...
    /// </summary>
    public readonly ref struct Output<TParsed, TToken, TInput> : IOutput<TParsed, TToken, TInput> where TInput : IInput<TToken, TInput>, allows ref struct where TParsed : allows ref struct where TToken : allows ref struct
    {
        private readonly TParsed parsed;

        public Output()
        {
            //// TODO you need one of these constructors for each of your ref structs
            throw new System.InvalidOperationException("TODO actually invent your own custom exception type for this");
        }

        public Output(TParsed parsed, TInput remainder)
        {
            this.Success = true;
            this.parsed = parsed;
            this.Remainder = remainder;
        }

        public Output(TInput remainder)
        {
            this.Success = false;
            this.Remainder = remainder;
        }

        public bool Success { get; }

        public TParsed Parsed
        {
            get
            {
                if (this.Success)
                {
                    return this.parsed;
                }

                throw new System.Exception("TODO");
            }
        }

        public TInput Remainder { get; }
    }

    public static class ParserExtensions
    {
        private readonly ref struct Parser<TInput, TToken, TOutput, TParsed> : IParser<TInput, TToken, TOutput, TParsed, Parser<TInput, TToken, TOutput, TParsed>> where TInput : IInput<TToken, TInput>, allows ref struct where TOutput : IOutput<TParsed, TToken, TInput>, allows ref struct where TToken : allows ref struct where TParsed : allows ref struct
        {
            public TOutput Parse(TInput input)
            {
                throw new System.NotImplementedException();
            }

            public ParserExtensions.OrParser<TInput, TToken, TOutput, TParsed, Parser<TInput, TToken, TOutput, TParsed>, TNextParser> Or<TNextParser>(TNextParser next) where TNextParser : IParser<TInput, TToken, TOutput, TParsed, TNextParser>, allows ref struct
            {
                //// TODO
                return new ParserExtensions.OrParser<TInput, TToken, TOutput, TParsed, Parser<TInput, TToken, TOutput, TParsed>, TNextParser>();
            }
        }

        public static void DoWork()
        {
            var parser1 = new Parser<Input<char>, char, Output<object, char, Input<char>>, object>();
            var parser2 = new Parser<Input<char>, char, Output<object, char, Input<char>>, object>();

            var parser3 = parser1.Or(parser2);
            var parser4 = parser3.Or(parser1);

            /*Or
                <
                    Parser<Input<char>, char, Output<object, char, Input<char>>, object>,
                    Parser<Input<char>, char, Output<object, char, Input<char>>, object>,
                    Input<char>,
                    Output<object, char, Input<char>>,
                    char,
                    object
                >(parser1, parser2);*/
        }

        /*public static OrParser<TInput, TToken, TOutput, TParsed, TFirstParser, TSecondParser> Or<TFirstParser, TSecondParser, TInput, TOutput, TToken, TParsed>(this TFirstParser first, TSecondParser second) where TInput : IInput<TToken, TInput>, allows ref struct where TOutput : IOutput<TParsed, TToken, TInput>, allows ref struct where TToken : allows ref struct where TParsed : allows ref struct where TFirstParser : IParser<TInput, TToken, TOutput, TParsed>, allows ref struct where TSecondParser : IParser<TInput, TToken, TOutput, TParsed>, allows ref struct
        {
            return new OrParser<TInput, TToken, TOutput, TParsed, TFirstParser, TSecondParser>();
        }*/

        public readonly ref struct AtLeast<TInput, TToken, TOutput, TParsed, TParser> : IParser<TInput, TToken, TOutput, TParsed, AtLeast<TInput, TToken, TOutput, TParsed, TParser>> where TInput : IInput<TToken, TInput>, allows ref struct where TOutput : IOutput<TParsed, TToken, TInput>, allows ref struct where TToken : allows ref struct where TParsed : allows ref struct where TParser : IParser<TInput, TToken, TOutput, TParsed, TParser>, allows ref struct
        {
            private readonly TParser parser;
            private readonly int minimum;

            public AtLeast(TParser parser, int minimum)
            {
                this.parser = parser;
                this.minimum = minimum;
            }

            public TOutput Parse(TInput input)
            {
                //// TODO make it return a collection
                TOutput output;
                for (int i = 0; i < this.minimum; ++i)
                {
                    output = this.parser.Parse(input);
                    if (!output.Success)
                    {
                        return output;
                    }

                    input = output.Remainder;
                }
            }
        }

        public readonly ref struct ExactlyParser<TInput, TToken, TParsed, TParser, TOutput2> : IParser<TInput, TToken, Output<IEnumerable<TParsed>, TToken, TInput>, IEnumerable<TParsed>, ExactlyParser<TInput, TToken, TParsed, TParser, TOutput2>> where TInput : IInput<TToken, TInput>, allows ref struct where TToken : allows ref struct where TParsed : allows ref struct where TParser : IParser<TInput, TToken, TOutput2, TParsed, TParser>, allows ref struct where TOutput2 : IOutput<TParsed, TToken, TInput>, allows ref struct
        {
            private readonly TParser parser;
            private readonly int count;

            public ExactlyParser(TParser parser, int count)
            {
                if (count < 0)
                {
                    throw new System.Exception("TODO");
                }

                this.parser = parser;
                this.count = count;
            }

            public Output<IEnumerable<TParsed>, TToken, TInput> Parse(TInput input)
            {
                var parsed = Empty<TParsed>();
                for (int i = 0; i < this.count; ++i)
                {
                    var output = this.parser.Parse(input);
                    if (!output.Success)
                    {
                        return new Output<IEnumerable<TParsed>, TToken, TInput>(input); //// TODO you need a way to expose errors
                    }

                    Append(parsed, output.Parsed);
                    input = output.Remainder;
                }

                return new Output<IEnumerable<TParsed>, TToken, TInput>(parsed, input);
            }
        }

        private static IEnumerable<T> Empty<T>() where T : allows ref struct
        {
            yield break;
        }

        private static IEnumerable<T> Append<T>(IEnumerable<T> source, T value) where T : allows ref struct
        {
            foreach (var element in source)
            {
                yield return element;
            }

            yield return value;
        }

        public readonly ref struct OrParser<TInput, TToken, TOutput, TParsed, TFirstParser, TSecondParser> : IParser<TInput, TToken, TOutput, TParsed, OrParser<TInput, TToken, TOutput, TParsed, TFirstParser, TSecondParser>> where TInput : IInput<TToken, TInput>, allows ref struct where TOutput : IOutput<TParsed, TToken, TInput>, allows ref struct where TToken : allows ref struct where TParsed : allows ref struct where TFirstParser : IParser<TInput, TToken, TOutput, TParsed, TFirstParser>, allows ref struct where TSecondParser : IParser<TInput, TToken, TOutput, TParsed, TSecondParser>, allows ref struct
        {
            private readonly TFirstParser firstParser;
            private readonly TSecondParser secondParser;

            public OrParser(TFirstParser firstParser, TSecondParser secondParser)
            {
                this.firstParser = firstParser;
                this.secondParser = secondParser;
            }

            public TOutput Parse(TInput input)
            {
                var output = this.firstParser.Parse(input);
                if (output.Success)
                {
                    return output;
                }

                var secondOutput = this.secondParser.Parse(input);
                if (secondOutput.Success)
                {
                    return secondOutput;
                }

                //// TODO how to pick which failure to return?
                return default!;
            }

            public OrParser<TInput, TToken, TOutput, TParsed, OrParser<TInput, TToken, TOutput, TParsed, TFirstParser, TSecondParser>, TNextParser> Or<TNextParser>(TNextParser next) where TNextParser : IParser<TInput, TToken, TOutput, TParsed, TNextParser>, allows ref struct
            {
                return new OrParser<TInput, TToken, TOutput, TParsed, OrParser<TInput, TToken, TOutput, TParsed, TFirstParser, TSecondParser>, TNextParser>(this, next);
            }

            public ExactlyParser<TInput, TToken, TParsed, OrParser<TInput, TToken, TOutput, TParsed, TFirstParser, TSecondParser>, TOutput> Exactly<TParser>(int count)
            {
                return new ExactlyParser<TInput, TToken, TParsed, OrParser<TInput, TToken, TOutput, TParsed, TFirstParser, TSecondParser>, TOutput>(this, count);
            }
        }
    }
}

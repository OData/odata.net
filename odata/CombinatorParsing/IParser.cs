namespace CombinatorParsing
{
    using __GeneratedOdata.Parsers.Rules;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;

    //// TODO add covariance and contravariance where able

    public ref struct BigCounter
    {
        private ulong smallEnd;
        
        private uint bigEnd;

        public BigCounter()
        {
            this.smallEnd = 0;
            this.bigEnd = 0;
        }

        public void Increment()
        {
            // increment the small end of the counter
            var result = Interlocked.Increment(ref this.smallEnd);
            if (result == 0)
            {
                // if `result` is `0`, then we have overflowed; let's increment the big end
                Interlocked.Increment(ref this.bigEnd);
            }
        }
    }

    public static class Interlocked2
    {
        private struct Thing
        {
            public Thing(ulong smallEnd, ulong bigEnd)
            {
                this.SmallEnd = smallEnd;
                this.BigEnd = bigEnd;
            }

            public ulong SmallEnd;

            public ulong BigEnd;
        }

        public static UInt128 Increment(ref UInt128 value)
        {
            var originalValue = value;
            var newValue = originalValue + 1;
            while (Interlocked.CompareExchange(ref value, newValue, originalValue) != originalValue)
            {
                originalValue = value;
                newValue = originalValue + 1;
            }

            return newValue;




            var thing = Unsafe.As<UInt128, Thing>(ref value);
            var smallEnd = thing.SmallEnd;
            var bigEnd = thing.BigEnd;

            Thing result;

            // increment the small end of the counter
            var smallResult = Interlocked.Increment(ref thing.SmallEnd);
            if (smallResult == 0)
            {
                // if `smallResult` is `0`, then we have overflowed; let's increment the big end
                var bigResult = Interlocked.Increment(ref thing.BigEnd);
                result = new Thing(smallResult, bigResult);
            }
            else
            {
                //// TODO the issue is, we don't know if some other thread overflowed us before we called increment; if they did, we should return bigend + 1
                if (thing.BigEnd == bigEnd)
                {
                    //// TODO and this assumes that only increments are allowed
                    result = new Thing(smallResult, bigEnd);
                }
                else
                {
                    result = new Thing(smallResult, bigEnd);
                }
            }

            return Unsafe.As<Thing, UInt128>(ref result);
        }
    }

    //// TODO use this instead of selectmany? https://github.com/xtofs/Floskel/blob/main/README.md


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
        }

        //// TODO parser need to be ref struct? they really only get instantiated once; it's really the closures and delegates that are probably perforamnce issues
        //// TODO profile delegates
        //// TODO create a parser tree akin to an expression tree that can be optimized?
        public readonly ref struct AtLeast<TInput, TToken, TParsed, TOutput2, TParser> : IParser<TInput, TToken, Output<IEnumerable<TParsed>, TToken, TInput>, IEnumerable<TParsed>, AtLeast<TInput, TToken, TParsed, TOutput2, TParser>> where TInput : IInput<TToken, TInput>, allows ref struct where TToken : allows ref struct where TParsed : allows ref struct where TParser : IParser<TInput, TToken, TOutput2, TParsed, TParser>, allows ref struct where TOutput2 : IOutput<TParsed, TToken, TInput>, allows ref struct
        {
            private readonly TParser parser;
            private readonly int minimum;

            public AtLeast(TParser parser, int minimum)
            {
                this.parser = parser;
                this.minimum = minimum;
            }

            public Output<IEnumerable<TParsed>, TToken, TInput> Parse(TInput input)
            {
                /*var exactly = new ExactlyParser<TInput, TToken, TParsed, TParser, TOutput2>(this.parser, this.minimum);
                var exactlyOutput = exactly.Parse(input);
                if (!exactlyOutput.Success)
                {
                    return new Output<IEnumerable<TParsed>, TToken, TInput>(exactlyOutput.Remainder);
                }

                var many = new ManyParser<TInput, TToken, TParsed, TParser, TOutput2>(this.parser);
                var manyOutput = many.Parse(exactlyOutput.Remainder);
                if (manyOutput.Success)
                {
                    return new Output<IEnumerable<TParsed>, TToken, TInput>(
                        Concat(exactlyOutput.Parsed, manyOutput.Parsed),
                        manyOutput.Remainder);
                }
                else
                {
                    return exactlyOutput;
                }*/

                return default;
            }
        }

        public readonly ref struct ManyParser<TInput, TToken, TParsed, TParser, TOutput2> : IParser<TInput, TToken, Output<IEnumerable<TParsed>, TToken, TInput>, IEnumerable<TParsed>, ExactlyParser<TInput, TToken, TParsed, TParser, TOutput2>> where TInput : IInput<TToken, TInput>, allows ref struct where TToken : allows ref struct where TParsed : allows ref struct where TParser : IParser<TInput, TToken, TOutput2, TParsed, TParser>, allows ref struct where TOutput2 : IOutput<TParsed, TToken, TInput>, allows ref struct
        {
            private readonly TParser parser;

            public ManyParser(TParser parser)
            {
                this.parser = parser;
            }

            public Output<IEnumerable<TParsed>, TToken, TInput> Parse(TInput input)
            {
                /*var parsed = Empty<TParsed>();
                while (true)
                {
                    var output = this.parser.Parse(input);
                    if (!output.Success)
                    {
                        return new Output<IEnumerable<TParsed>, TToken, TInput>(
                            parsed, 
                            input);
                    }

                    parsed = Append(parsed, output.Parsed);
                    input = output.Remainder;
                }*/
                return default;
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
                /*var parsed = Empty<TParsed>();
                var appended = Append(Enumerable.Empty<string>(), "ASdf");
                foreach (var element in appended)
                {
                }*/

                /*for (int i = 0; i < this.count; ++i)
                {
                    var output = this.parser.Parse(input);
                    if (!output.Success)
                    {
                        return new Output<IEnumerable<TParsed>, TToken, TInput>(input); //// TODO you need a way to expose errors
                    }

                    parsed = Append(parsed, output.Parsed);
                    input = output.Remainder;
                }

                return new Output<IEnumerable<TParsed>, TToken, TInput>(parsed, input);*/
                return default;
            }
        }

        public ref struct RefEnumerable<T> : IEnumerable<T> where T : allows ref struct
        {
            public RefEnumerable()
            {
            }

            public IEnumerator<T> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        public static class RefEnumerable
        {
            public static RefEnumerable<T> Empty<T>() where T : allows ref struct
            {
                return new RefEnumerable<T>();
            }
        }

        /*private static RefEnumerable<T> Empty<T>() where T : allows ref struct
        {
            return RefEnumerable.Empty<T>();
        }

        private static AppendEnumerable<T> Append<T>(IEnumerable<T> source, T value) where T : allows ref struct
        {
            return new AppendEnumerable<T>(source, value);
        }

        private ref struct AppendEnumerable<T> : IEnumerable<T> where T : allows ref struct
        {
            private readonly IEnumerable<T> source;
            private readonly T value;

            public AppendEnumerable(IEnumerable<T> source, T value)
            {
                this.source = source;
                this.value = value;
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public AppendEnumerator GetEnumerator()
            {
                return new AppendEnumerator(this);
            }

            public ref struct AppendEnumerator : IEnumerator<T>
            {
                private readonly AppendEnumerable<T> enumerable;

                public AppendEnumerator(AppendEnumerable<T> enumerable)
                {
                    this.enumerable = enumerable;
                }

                public T Current => throw new NotImplementedException();

                object IEnumerator.Current => throw new NotImplementedException();

                public void Dispose()
                {
                    throw new NotImplementedException();
                }

                public bool MoveNext()
                {
                    throw new NotImplementedException();
                }

                public void Reset()
                {
                    throw new NotImplementedException();
                }
            }
        }

        private static IEnumerable<T> Concat<T>(IEnumerable<T> first, IEnumerable<T> second) where T : allows ref struct
        {
            foreach (var element in first)
            {
                yield return element;
            }

            foreach (var element in second)
            {
                yield return element;
            }
        }*/

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

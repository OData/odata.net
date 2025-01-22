namespace CombinatorParsing
{
    using __GeneratedOdata.Parsers.Rules;
    using Root;
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
                var parsed = RefEnumerable.Empty<TParsed>();
                var empty = RefEnumerable.Empty<string>();
                var appended = RefEnumerable.Append(empty, "ASdf");
                foreach (var element in appended)
                {
                }

                ////empty = RefEnumerable.Append(empty, "asdf");

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

        public static class Ref2
        {
            public static Ref2<TElement, IRefEnumerable<TElement, IRefEnumerator<TElement>>, IRefEnumerator<TElement>> Empty<TElement>()
            {
                //// TODO actually implement this
                return new Ref2<TElement, IRefEnumerable<TElement, IRefEnumerator<TElement>>, IRefEnumerator<TElement>>();
            }

            public ref struct AppendEnumerable<TElement, TEnumerable, TEnumerator> : IRefEnumerable<TElement, AppendEnumerable<TElement, TEnumerable, TEnumerator>.Enumerator> 
                where TElement : allows ref struct
                where TEnumerable : IRefEnumerable<TElement, TEnumerator>, allows ref struct
                where TEnumerator : IRefEnumerator<TElement>, allows ref struct
            {
                public AppendEnumerable(TEnumerable enumerable, TElement value)
                {
                }

                public Enumerator GetEnumerator()
                {
                    throw new NotImplementedException();
                }

                public ref struct Enumerator : IRefEnumerator<TElement>
                {
                    public TElement Current => throw new NotImplementedException();

                    public bool MoveNext()
                    {
                        throw new NotImplementedException();
                    }
                }
            }

            public ref struct AppendEnumerable2<TElement, TEnumerable, TEnumerator> : IRefEnumerable<TElement, AppendEnumerable2<TElement, TEnumerable, TEnumerator>.Enumerator>
                where TElement : allows ref struct
                where TEnumerable : IRefEnumerable<TElement, TEnumerator>, allows ref struct
                where TEnumerator : IRefEnumerator<TElement>, allows ref struct
            {
                private readonly Ref2<TElement, TEnumerable, TEnumerator> source;
                private readonly TElement value;

                public AppendEnumerable2(Ref2<TElement, TEnumerable, TEnumerator> source, TElement value)
                {
                    this.source = source;
                    this.value = value;
                }

                public Enumerator GetEnumerator()
                {
                    throw new NotImplementedException();
                }

                public ref struct Enumerator : IRefEnumerator<TElement>
                {
                    public TElement Current => throw new NotImplementedException();

                    public bool MoveNext()
                    {
                        throw new NotImplementedException();
                    }
                }
            }

            public static Ref2<TElement, AppendEnumerable2<TElement, TEnumerable, TEnumerator>, AppendEnumerable2<TElement, TEnumerable, TEnumerator>.Enumerator> Append2<TElement, TEnumerable, TEnumerator>(
                Ref2<TElement, TEnumerable, TEnumerator> source,
                TElement value)
                where TElement : allows ref struct
                where TEnumerable : IRefEnumerable<TElement, TEnumerator>, allows ref struct
                where TEnumerator : IRefEnumerator<TElement>, allows ref struct
            {
                return new Ref2<TElement, AppendEnumerable2<TElement, TEnumerable, TEnumerator>, AppendEnumerable2<TElement, TEnumerable, TEnumerator>.Enumerator>(
                    new AppendEnumerable2<TElement, TEnumerable, TEnumerator>(source, value));
            }
        }

        public ref struct Ref2<TElement, TEnumerable, TEnumerator> : IRefEnumerable<TElement, Ref2<TElement, TEnumerable, TEnumerator>.Enumerator>
            where TElement : allows ref struct 
            where TEnumerable : IRefEnumerable<TElement, TEnumerator>, allows ref struct
            where TEnumerator : IRefEnumerator<TElement>, allows ref struct
        {
            private readonly TEnumerable enumerable;

            public Ref2(TEnumerable enumerable)
            {
                this.enumerable = enumerable;
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(
                    this.enumerable.GetEnumerator());
            }

            public ref struct Enumerator : IRefEnumerator<TElement>
            {
                private readonly TEnumerator enumerator;

                public Enumerator(TEnumerator enumerator)
                {
                    this.enumerator = enumerator;
                }

                public TElement Current
                {
                    get
                    {
                        return this.enumerator.Current;
                    }
                }

                public bool MoveNext()
                {
                    return this.enumerator.MoveNext();
                }
            }
        }

        public interface IRefEnumerable<TElement, TEnumerator> 
            where TElement : allows ref struct 
            where TEnumerator : IRefEnumerator<TElement>, allows ref struct
        {
            TEnumerator GetEnumerator();
        }

        public interface IRefEnumerator<T> where T : allows ref struct
        {
            T Current { get; }

            bool MoveNext();
        }

        public ref struct RefEnumerable<TElement, TEnumerableContext, TEnumeratorContext> : IEnumerable<TElement> where TElement : allows ref struct where TEnumerableContext : allows ref struct where TEnumeratorContext : allows ref struct
        {
            private readonly Func<TEnumerableContext, RefEnumerator> getEnumerator;

            private readonly TEnumerableContext enumerableContext;

            public RefEnumerable(Func<TEnumerableContext, RefEnumerator> getEnumerator, TEnumerableContext enumerableContext)
            {
                this.getEnumerator = getEnumerator;
                this.enumerableContext = enumerableContext;
            }

            IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public RefEnumerator GetEnumerator()
            {
                return this.getEnumerator(this.enumerableContext);
            }

            public ref struct RefEnumerator : IEnumerator<TElement>
            {
                private readonly Func<TEnumeratorContext, TElement> current;
                private readonly Func<TEnumeratorContext, bool> moveNext;
                private readonly TEnumeratorContext context;

                public RefEnumerator(Func<TEnumeratorContext, TElement> current, Func<TEnumeratorContext, bool> moveNext, TEnumeratorContext context)
                {
                    //// TODO do all methods
                    //// TODO this seems like it probably defeats the purpose

                    this.current = current;
                    this.moveNext = moveNext;
                    this.context = context;
                }

                public TElement Current
                {
                    get
                    {

                        return this.current(this.context);
                    }
                }

                object IEnumerator.Current => throw new NotImplementedException();

                public void Dispose()
                {
                }

                public bool MoveNext()
                {
                    return this.moveNext(this.context);
                }

                public void Reset()
                {
                    throw new NotImplementedException();
                }
            }
        }

        public static class RefEnumerable
        {
            public static RefEnumerable<T, Root.Void, Root.Void> Empty<T>() where T : allows ref struct
            {
                return new RefEnumerable<T, Root.Void, Root.Void>();
            }

            public static RefEnumerable<TElement, AppendContext<TElement, TEnumerableContext, TEnumeratorContext>, AppendEnumerator<TElement, TEnumerableContext, TEnumeratorContext>> Append<TElement, TEnumerableContext, TEnumeratorContext>(RefEnumerable<TElement, TEnumerableContext, TEnumeratorContext> source, TElement value) where TElement : allows ref struct where TEnumerableContext : allows ref struct where TEnumeratorContext : allows ref struct
            {
                return new RefEnumerable
                    <
                        TElement,
                        AppendContext<TElement, TEnumerableContext, TEnumeratorContext>, 
                        AppendEnumerator<TElement, TEnumerableContext, TEnumeratorContext>
                    >(
                    appendContext => 
                        new RefEnumerable<TElement, AppendContext<TElement, TEnumerableContext, TEnumeratorContext>, AppendEnumerator<TElement, TEnumerableContext, TEnumeratorContext>>.RefEnumerator(
                            appendEnumerator => appendEnumerator.Current,
                            appendEnumerator => appendEnumerator.MoveNext(),
                            new AppendEnumerator<TElement, TEnumerableContext, TEnumeratorContext>(
                                appendContext.Source.GetEnumerator(),
                                appendContext.Value)),
                    new AppendContext<TElement, TEnumerableContext, TEnumeratorContext>(
                        source,
                        value));
            }

            public ref struct AppendContext<TElement, TEnumerableContext, TEnumeratorContext> where TElement : allows ref struct where TEnumerableContext : allows ref struct where TEnumeratorContext : allows ref struct
            {
                public AppendContext(RefEnumerable<TElement, TEnumerableContext, TEnumeratorContext> source, TElement value)
                {
                    Source = source;
                    Value = value;
                }

                public RefEnumerable<TElement, TEnumerableContext, TEnumeratorContext> Source { get; }
                public TElement Value { get; }
            }
        }

        public ref struct AppendEnumerator<TElement, TEnumerableContext, TEnumeratorContext> : IEnumerator<TElement> where TElement : allows ref struct where TEnumerableContext : allows ref struct where TEnumeratorContext : allows ref struct
        {
            private readonly RefEnumerable<TElement, TEnumerableContext, TEnumeratorContext>.RefEnumerator enumerator;

            private readonly TElement value;

            private bool finished;

            public AppendEnumerator(RefEnumerable<TElement, TEnumerableContext, TEnumeratorContext>.RefEnumerator enumerator, TElement value)
            {
                this.enumerator = enumerator;
                this.value = value;

                this.finished = false;
            }

            public TElement Current
            {
                get
                {
                    if (this.finished)
                    {
                        return this.value;
                    }

                    return this.enumerator.Current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public void Dispose()
            {
                //// TODO proper dispose
                this.enumerator.Dispose();
            }

            public bool MoveNext()
            {
                if (this.finished)
                {
                    return false;
                }

                var moved = this.enumerator.MoveNext();
                if (!moved)
                {
                    this.finished = true;
                    return true;
                }

                return moved;
            }

            public void Reset()
            {
                throw new NotImplementedException();
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

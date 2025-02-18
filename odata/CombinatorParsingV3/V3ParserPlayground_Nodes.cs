namespace CombinatorParsingV3
{
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using System.Collections;
    using System.Threading;
    using static CombinatorParsingV3.V3ParserPlayground;
    using CombinatorParsingV1;

    public static partial class V3ParserPlayground
    {
        public sealed class Slash<TMode> : IDeferredAstNode<char, Slash<ParseMode.Realized>> where TMode : ParseMode
        {
            private readonly Func<IDeferredOutput2<char>> future;

            private Output<char, Slash<ParseMode.Realized>>? cachedOutput;

            public Slash(Func<IDeferredOutput2<char>> future)
            {
                this.future = future;

                this.cachedOutput = null;
            }

            private Slash()
            {
                //// TODO actually assign fields to make the `realize` method not throw on subsequent calls
            }

            public static Slash<ParseMode.Realized> Realized { get; } = new Slash<ParseMode.Realized>(); //// TODO it's not a great experience to have to call `Slash<TMode>.Realized`; the `tmode` part never matters

            public IOutput<char, Slash<ParseMode.Realized>> Realize()
            {
                if (this.cachedOutput != null)
                {
                    return this.cachedOutput;
                }

                var output = this.future();
                if (!output.Success)
                {
                    this.cachedOutput = new Output<char, Slash<ParseMode.Realized>>(false, default, output.Remainder);
                    return this.cachedOutput;
                }

                var input = output.Remainder;

                if (input.Current == '/')
                {
                    this.cachedOutput = new Output<char, Slash<ParseMode.Realized>>(true, Slash<TMode>.Realized, input.Next());
                    return this.cachedOutput;
                }
                else
                {
                    this.cachedOutput = new Output<char, Slash<ParseMode.Realized>>(false, default, input);
                    return this.cachedOutput;
                }
            }
        }

        public abstract class AlphaNumeric<TMode> : IDeferredAstNode<char, AlphaNumeric<ParseMode.Realized>>
        {
            private AlphaNumeric()
            {
            }

            public IOutput<char, AlphaNumeric<ParseMode.Realized>> Realize()
            {
                return this.RealizeImpl();
            }

            protected abstract IOutput<char, AlphaNumeric<ParseMode.Realized>> RealizeImpl();

            public sealed class A : AlphaNumeric<TMode>, IDeferredAstNode<char, AlphaNumeric<ParseMode.Realized>.A>
            {
                private readonly Func<IDeferredOutput2<char>> future;

                private Output<char, AlphaNumeric<ParseMode.Realized>.A>? cachedOutput;

                public A(Func<IDeferredOutput2<char>> future)
                {
                    this.future = future;

                    this.cachedOutput = null;
                }

                private A()
                {
                    //// TODO actually assign fields to make the `realize` method not throw on subsequent calls
                }

                public static AlphaNumeric<ParseMode.Realized>.A Realized { get; } = new AlphaNumeric<ParseMode.Realized>.A();

                public new IOutput<char, AlphaNumeric<ParseMode.Realized>.A> Realize()
                {
                    if (this.cachedOutput != null)
                    {
                        return this.cachedOutput;
                    }

                    var output = this.future();
                    if (!output.Success)
                    {
                        this.cachedOutput = new Output<char, AlphaNumeric<ParseMode.Realized>.A>(false, default, output.Remainder);
                        return this.cachedOutput;
                    }

                    var input = output.Remainder;
                    if (input == null)
                    {
                        this.cachedOutput = new Output<char, AlphaNumeric<ParseMode.Realized>.A>(false, default, input);
                        return this.cachedOutput;
                    }

                    if (input.Current == 'A')
                    {
                        this.cachedOutput = new Output<char, AlphaNumeric<ParseMode.Realized>.A>(true, AlphaNumeric<ParseMode.Realized>.A.Realized, input.Next());
                        return this.cachedOutput;
                    }
                    else
                    {
                        this.cachedOutput = new Output<char, AlphaNumeric<ParseMode.Realized>.A>(false, default, input);
                        return this.cachedOutput;
                    }
                }

                protected override IOutput<char, AlphaNumeric<ParseMode.Realized>> RealizeImpl()
                {
                    return this.Realize();
                }
            }
        }

        /*public sealed class AlphaNumeric : IDeferredAstNode<char, AlphaNumeric>
        {
            private readonly IParser<char, AlphaNumeric> parser;
            private readonly IInput<char> input;

            //// private char @char;

            //// private bool deferred;

            public AlphaNumeric(IParser<char, AlphaNumeric> parser, IInput<char> input)
            {
                this.parser = parser;
                this.input = input;

                //// this.deferred = true;
            }

            public AlphaNumeric(char @char)
            {
                this.@char = @char;

                this.deferred = false;
            }

            public char Char
            {
                get
                {

                }
            }

            public IOutput<char, AlphaNumeric> Realize()
            {
                if (this.deferred)
                {
                    var _char_1 = this.parser.Parse(input);
                    if (_char_1.Success)
                    {
                        this.@char = _char_1.Parsed.@char;
                        this.deferred = false;
                        return new Output<char, AlphaNumeric>(true, this, _char_1.Remainder);
                    }
                    else
                    {
                        return new Output<char, AlphaNumeric>(false, default, this.input);
                    }
                }
                else
                {
                    //// TODO where do you get the remainder from for the case where this instance was constructed with the char already? //// TODO maybe that constructor overload shouldn't exist?
                    return new Output<char, AlphaNumeric>(true, this, null);
                }
            }
        }*/

        /*public sealed class Many<T> : IDeferredAstNode<char, IEnumerable<T>> where T : IDeferredAstNode<char, T>
        {
            ////private IInput<char> input;
            private readonly Func<Func<IDeferredOutput2<char>>, T> nodeFactory;

            private readonly Func<IDeferredOutput2<char>> future;

            //public Many(IInput<char> input, Func<IInput<char>, T> nodeFactory)
            //{
            //    this.input = input;
            //    this.nodeFactory = nodeFactory;

            //    this.future = null;
            //}

            public Many(Func<IDeferredOutput2<char>> future, Func<Func<IDeferredOutput2<char>>, T> nodeFactory)
            {
                this.future = future; //// TODO this should be of type `future`
                this.nodeFactory = nodeFactory;
            }

            public IOutput<char, IEnumerable<T>> Realize() //// TODO the other ones all have the second type as themselves...maybe you need an ienumerable property instead?
            {
                var output2 = this.future();
                if (!output2.Success)
                {
                    return new Output<char, IEnumerable<T>>(false, default, output2.Remainder);
                }

                var input = output2.Remainder;

                var sequence = new List<T>();
                var node = this.nodeFactory(DeferredOutput2.FromValue(input));
                var output = node.Realize();
                while (output.Success)
                {
                    sequence.Add(output.Parsed);
                    node = this.nodeFactory(DeferredOutput2.FromValue(output.Remainder));
                }

                return new Output<char, IEnumerable<T>>(true, sequence, input);
            }
        }*/

        public sealed class AtLeastOne<TDeferredAstNode, TRealizedAstNode, TMode> : IDeferredAstNode<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode> where TMode : ParseMode
        {
            private readonly Func<Func<IDeferredOutput2<char>>, TDeferredAstNode> nodeFactory;

            private readonly Func<IDeferredOutput2<char>> future;

            private Output<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>? cachedOutput;

            public AtLeastOne(Func<IDeferredOutput2<char>> future, Func<Func<IDeferredOutput2<char>>, TDeferredAstNode> nodeFactory)
            {
                this.future = future; //// TODO this should be of type `future`
                this.nodeFactory = nodeFactory;

                this.cachedOutput = null;
            }

            /*public SequenceNode<T> Element
            {
                get
                {
                    return new SequenceNode<T>(this.future, this.nodeFactory);
                }
            }

            public IOutput<char, Many<T>> Realize()
            {
                var output = this.Element.Realize();
                if (output.Success)
                {
                    return new Output<char, Many<T>>(true, this, output.Remainder);
                }
                else
                {
                    return new Output<char, Many<T>>(false, default, output.Remainder);
                }
            }*/

            private AtLeastOne(TRealizedAstNode _1, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> _2, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> _3)
            {
            }
            
            public TDeferredAstNode _1
            {
                get
                {
                    return this.nodeFactory(this.future);
                }
            }

            public OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode> _2
            {
                get
                {
                    //// TODO i think you could actually just set this in the constructor...
                    return new OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode>(DeferredOutput2.ToPromise(this._1.Realize), this.nodeFactory);
                }
            }

            public OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode> _3
            {
                get
                {
                    return new OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode>(DeferredOutput2.ToPromise(this._2.Realize), this.nodeFactory);
                }
            }

            public IOutput<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> Realize()
            {
                if (this.cachedOutput != null)
                {
                    return this.cachedOutput;
                }

                var output = this._3.Realize();
                if (output.Success)
                {
                    this.cachedOutput = new Output<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        true,
                        new AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                            this._1.Realize().Parsed,
                            new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(this._2.Realize().Parsed),
                            new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(this._3.Realize().Parsed)),
                        output.Remainder);
                    return this.cachedOutput;
                }
                else
                {
                    this.cachedOutput = new Output<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(false, default, output.Remainder);
                    return this.cachedOutput;
                }
            }
        }

        public sealed class Many<TDeferredAstNode, TRealizedAstNode, TMode> : IDeferredAstNode<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode> where TMode : ParseMode
        {
            private readonly Func<Func<IDeferredOutput2<char>>, TDeferredAstNode> nodeFactory;

            private readonly Func<IDeferredOutput2<char>> future;

            private Output<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>? cachedOutput;

            public Many(Func<IDeferredOutput2<char>> future, Func<Func<IDeferredOutput2<char>>, TDeferredAstNode> nodeFactory)
            {
                this.future = future; //// TODO this should be of type `future`
                this.nodeFactory = nodeFactory;

                this.cachedOutput = null;
            }

            private Many(ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> node)
            {
            }

            /*private Many(OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> _1, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> _2, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> _3)
            {
            }*/

            /*public SequenceNode<T> Element
            {
                get
                {
                    return new SequenceNode<T>(this.future, this.nodeFactory);
                }
            }

            public IOutput<char, Many<T>> Realize()
            {
                var output = this.Element.Realize();
                if (output.Success)
                {
                    return new Output<char, Many<T>>(true, this, output.Remainder);
                }
                else
                {
                    return new Output<char, Many<T>>(false, default, output.Remainder);
                }
            }*/

            /*public OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode> _1
            {
                get
                {
                    return new OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode>(this.future, this.nodeFactory);
                }
            }

            public OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode> _2
            {
                get
                {
                    return new OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode>(DeferredOutput2.ToPromise(this._1.Realize), this.nodeFactory);
                }
            }

            public OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode> _3
            {
                get
                {
                    return new OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode>(DeferredOutput2.ToPromise(this._2.Realize), this.nodeFactory);
                }
            }*/

            public ManyNode<TDeferredAstNode, TRealizedAstNode, TMode> Node
            {
                get
                {
                    return new ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>(this.future, this.nodeFactory);
                }
            }

            public IOutput<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> Realize()
            {
                if (this.cachedOutput != null)
                {
                    return this.cachedOutput;
                }

                var output = this.Node.Realize();
                if (output.Success)
                {
                    this.cachedOutput = new Output<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        true, 
                        new Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                            output.Parsed),
                        output.Remainder);
                    return this.cachedOutput;
                }
                else
                {
                    // if the optional failed to parse, it means that its dependencies failed to parse
                    this.cachedOutput = new Output<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(false, default, output.Remainder);
                    return this.cachedOutput;
                }
            }
        }

        public sealed class ManyNode<TDeferredAstNode, TRealizedAstNode, TMode> : IDeferredAstNode<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode> where TMode : ParseMode
        {
            private readonly Func<IDeferredOutput2<char>> future;
            private readonly Func<Func<IDeferredOutput2<char>>, TDeferredAstNode> nodeFactory;

            private Output<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> cachedOutput;

            public ManyNode(Func<IDeferredOutput2<char>> future, Func<Func<IDeferredOutput2<char>>, TDeferredAstNode> nodeFactory)
            {
                this.future = future;
                this.nodeFactory = nodeFactory;

                this.cachedOutput = null;
            }

            private static ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> GetTerminalRealizedNode()
            {
                return new ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(new RealNullable<TRealizedAstNode>()), GetTerminalRealizedNode);
            }

            private ManyNode(OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode> element, Func<ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>> next)
            {
            }

            public OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode> Element
            {
                get
                {
                    return new OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode>(this.future, this.nodeFactory);
                }
            }

            /// <summary>
            /// TODO realize should only be called on this if <see cref="Element"/> has a value
            /// </summary>
            public ManyNode<TDeferredAstNode, TRealizedAstNode, TMode> Next
            {
                get
                {
                    return new ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>(DeferredOutput2.ToPromise(this.Element.Realize), this.nodeFactory);
                }
            }

            public IOutput<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> Realize()
            {
                if (this.cachedOutput != null)
                {
                    return this.cachedOutput;
                }

                var realizedElement = this.Element.Realize();
                if (!realizedElement.Success)
                {
                    // this means that the nullable parsing *didn't succeed*, which only happens if its dependencies couldn't be parsed; this means that we also can't succeed in parsing
                    this.cachedOutput = new Output<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(false, default, realizedElement.Remainder);
                    return this.cachedOutput;
                }

                if (realizedElement.Parsed.HasValue)
                {
                    var realizedNext = this.Next.Realize();
                    // *this* instance is the "dependency" for `next`, so we can only have success cases
                    this.cachedOutput = new Output<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        true,
                        new ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                            new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(realizedElement.Parsed),
                            () => realizedNext.Parsed),
                        realizedNext.Remainder);
                    return this.cachedOutput;
                }
                else
                {
                    this.cachedOutput = new Output<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        true,
                        new ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                            new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(realizedElement.Parsed),
                            GetTerminalRealizedNode),
                        realizedElement.Remainder);
                    return this.cachedOutput;
                }
            }
        }

        //public sealed class SequenceNode<T> : IDeferredAstNode<char, SequenceNode<T>> where T : IDeferredAstNode<char, T>
        //{
        //    private readonly Func<IDeferredOutput2<char>> future;
        //    private readonly Func<Func<IDeferredOutput2<char>>, T> nodeFactory;

        //    public SequenceNode(Func<IDeferredOutput2<char>> future, Func<Func<IDeferredOutput2<char>>, T> nodeFactory)
        //    {
        //        this.future = future;
        //        this.nodeFactory = nodeFactory;
        //    }

        //    public T Value
        //    {
        //        get
        //        {
        //            return this.nodeFactory(this.future);
        //            ////return new OptionalNode<T>(this.future, this.nodeFactory);
        //        }
        //    }

        //    /*public OptionalNode<SequenceNode<T>> Next
        //    {
        //        get
        //        {
        //            return new OptionalNode<SequenceNode<T>>(DeferredOutput2.ToPromise(this.Value.Realize), input => new SequenceNode<T>(input, this.nodeFactory));
        //        }
        //    }

        //    public IOutput<char, SequenceNode<T>> Realize()
        //    {
        //        var output = this.Next.Realize();
        //        if (output.Success)
        //        {
        //            return new Output<char, SequenceNode<T>>(true, this, output.Remainder);
        //        }
        //        else
        //        {
        //            // TODO this branch can't really get hit; is that ok?
        //            return new Output<char, SequenceNode<T>>(false, default, output.Remainder);
        //        }
        //    }*/

        //    public OptionalNode<SequenceNode<T>> Next
        //    {
        //        get
        //        {
        //            return new OptionalNode<SequenceNode<T>>(DeferredOutput2.ToPromise(this.Value.Realize), input => new SequenceNode<T>(input, this.nodeFactory));
        //            //// return new SequenceNode<T>(DeferredOutput2.ToPromise(this.Value.Realize), this.nodeFactory);
        //        }
        //    }

        //    public IOutput<char, SequenceNode<T>> Realize()
        //    {
        //        var output = this.Next.Realize();
        //        if (output.Success)
        //        {
        //            return new Output<char, SequenceNode<T>>(true, this, output.Remainder);
        //        }
        //        else
        //        {
        //            // TODO this branch can't really get hit; is that ok?
        //            return new Output<char, SequenceNode<T>>(false, default, output.Remainder);
        //        }
        //    }
        //}

        public sealed class OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode> : IDeferredAstNode<char, RealNullable<TRealizedAstNode>> where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode> where TMode : ParseMode
        {
            private readonly Func<IDeferredOutput2<char>> future;
            private readonly Func<Func<IDeferredOutput2<char>>, TDeferredAstNode> nodeFactory;

            private Output<char, RealNullable<TRealizedAstNode>>? cachedOutput;

            public OptionalNode(Func<IDeferredOutput2<char>> future, Func<Func<IDeferredOutput2<char>>, TDeferredAstNode> nodeFactory)
            {
                this.future = future;
                this.nodeFactory = nodeFactory;

                this.cachedOutput = null;
            }

            internal OptionalNode(RealNullable<TRealizedAstNode> value)
            {
                //// TODO only let this be called if `TMode` is `Realized`
            }

            public TDeferredAstNode Value
            {
                get
                {
                    return this.nodeFactory(this.future);
                }
            }

            private IOutput<char, TDeferredAstNode?> Get()
            {
                IDeferredOutput2<char> output = null;
                var node = this.nodeFactory(() => output = this.future());

                if (node == null)
                {
                    return new Output<char, TDeferredAstNode?>(true, node, output.Remainder);
                }
                else
                {
                    var output2 = node.Realize();
                    return new Output<char, TDeferredAstNode?>(true, node, output2.Remainder);
                }
            }

            public IOutput<char, RealNullable<TRealizedAstNode>> Realize()
            {
                if (this.cachedOutput != null)
                {
                    return this.cachedOutput;
                }

                var deferredOutput = this.future();
                if (!deferredOutput.Success)
                {
                    this.cachedOutput = new Output<char, RealNullable<TRealizedAstNode>>(false, default, deferredOutput.Remainder);
                    return this.cachedOutput;
                }

                var output = this.Value.Realize();
                if (output.Success)
                {
                    this.cachedOutput = new Output<char, RealNullable<TRealizedAstNode>>(true, new RealNullable<TRealizedAstNode>(output.Parsed), output.Remainder);
                    return this.cachedOutput;
                }
                else
                {
                    this.cachedOutput = new Output<char, RealNullable<TRealizedAstNode>>(true, new RealNullable<TRealizedAstNode>(), output.Remainder); //// deferredOutput.Remainder);
                    return this.cachedOutput;
                }
            }
        }

        public readonly struct RealNullable<T>
        {
            private readonly T value;

            public RealNullable(T value)
            {
                this.value = value;

                this.HasValue = true;
            }

            public bool HasValue { get; }

            public T Value
            {
                get
                {
                    if (!this.HasValue)
                    {
                        throw new InvalidOperationException("TODO");
                    }

                    return this.value;
                }
            }
        }

        public sealed class Segment<TMode> : IDeferredAstNode<char, Segment<ParseMode.Realized>> where TMode : ParseMode
        {
            ////private readonly IParser<char, Segment> parser;
            private readonly Func<IDeferredOutput2<char>> future;

            ////private Slash slash;
            ////private IEnumerable<AlphaNumeric> characters;

            ////private bool deferred;

            private Output<char, Segment<ParseMode.Realized>>? cachedOutput;

            public Segment(Func<IDeferredOutput2<char>> future)
            //// : this(SegmentParser.Instance, input)
            {
                this.future = future;

                this.cachedOutput = null;
            }

            /*public Segment(IParser<char, Segment> parser, IInput<char> input)
            {
                this.parser = parser;
                this.input = input;

                this.deferred = true;
            }

            public Segment(Slash slash, IEnumerable<AlphaNumeric> characters)
            {
                this.slash = slash;
                this.characters = characters;

                this.deferred = false;
            }*/

            private Segment(Slash<ParseMode.Realized> slash, AtLeastOne<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, ParseMode.Realized> characters)
            {
            }

            public Slash<TMode> Slash
            {
                get
                {
                    return new Slash<TMode>(this.future);
                }
            }

            public AtLeastOne<AlphaNumeric<TMode>, AlphaNumeric<ParseMode.Realized>, TMode> Characters
            {
                get
                {
                    return new AtLeastOne<AlphaNumeric<TMode>, AlphaNumeric<ParseMode.Realized>, TMode>(
                        DeferredOutput2.ToPromise(this.Slash.Realize),
                        input => new AlphaNumeric<TMode>.A(input)); //// TODO what would a discriminated union actually look like here?

                    /*if (this.deferred)
                    {
                        throw new System.Exception("TODO not parsed yet");
                        // TODO ideferredoutput should probably just be ioutput so that you can get this remainder
                        // return new Characters(CharactersParser.Instance, this.Slash.Realize().Remainder)
                    }

                    return this.characters;*/
                }
            }

            public IOutput<char, Segment<ParseMode.Realized>> Realize()
            {
                if (this.cachedOutput != null)
                {
                    return this.cachedOutput;
                }

                var output = this.Characters.Realize();
                if (output.Success)
                {
                    this.cachedOutput = new Output<char, Segment<ParseMode.Realized>>(
                        true, 
                        new Segment<ParseMode.Realized>(
                            this.Slash.Realize().Parsed,
                            this.Characters.Realize().Parsed as AtLeastOne<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, ParseMode.Realized>), //// TODO this is the hackiest part of the whole parsemode thing so far; see if you can address it
                        output.Remainder);
                    return this.cachedOutput;
                }
                else
                {
                    this.cachedOutput = new Output<char, Segment<ParseMode.Realized>>(false, default, output.Remainder);
                    return this.cachedOutput;
                }

                //// TODO you should cache the deferredoutput instance
                /*if (this.deferred)
                {
                    var output = this.parser.Parse(this.input);
                    if (output.Success)
                    {
                        this.slash = output.Parsed.slash;
                        this.characters = output.Parsed.characters;
                        this.deferred = false;
                        return new DeferredOutput<Segment>(true, this);
                    }
                    else
                    {
                        return new DeferredOutput<Segment>(false, default);
                    }
                }
                else
                {
                    // we are not a deferred instance, but maybe our members are
                    var slashOutput = this.slash.Realize();
                    if (!slashOutput.Success)
                    {
                        return new DeferredOutput<Segment>(false, default);
                    }

                    var charactersOutput = this.characters.Select(character => character.Realize());
                    if (!charactersOutput.All(_ => _.Success))
                    {
                        return new DeferredOutput<Segment>(false, default);
                    }

                    this.slash = slashOutput.Parsed;
                    this.characters = charactersOutput.Select(_ => _.Parsed);

                    //// TODO this method basically just mimics the parser implementations; should you actually just put all of the parsing here?

                    return new DeferredOutput<Segment>(true, this);
                }*/
            }
        }

        public sealed class EqualsSign<TMode> : IDeferredAstNode<char, EqualsSign<ParseMode.Realized>> where TMode : ParseMode
        {
            ////private readonly IInput<char> input;
            private readonly Func<IDeferredOutput2<char>> future;

            private Output<char, EqualsSign<ParseMode.Realized>>? cachedOutput;

            /*public EqualsSign(IInput<char> input)
            {
                this.input = input;
            }*/

            public EqualsSign(Func<IDeferredOutput2<char>> future)
            {
                //// TODO create the `future` type and then follow this single constructor pattern everywhere
                this.future = future;

                this.cachedOutput = null;
            }

            private EqualsSign()
            {
            }

            public static EqualsSign<ParseMode.Realized> Realized { get; } = new EqualsSign<ParseMode.Realized>();

            public IOutput<char, EqualsSign<ParseMode.Realized>> Realize()
            {
                if (this.cachedOutput != null)
                {
                    return this.cachedOutput;
                }

                var output = this.future();
                if (!output.Success)
                {
                    this.cachedOutput = new Output<char, EqualsSign<ParseMode.Realized>>(false, default, output.Remainder);
                    return this.cachedOutput;
                }

                var input = output.Remainder;
                if (input == null)
                {
                    this.cachedOutput = new Output<char, EqualsSign<ParseMode.Realized>>(false, default, input);
                    return this.cachedOutput;
                }

                if (input.Current == '=')
                {
                    this.cachedOutput = new Output<char, EqualsSign<ParseMode.Realized>>(true, EqualsSign<ParseMode.Realized>.Realized, input.Next());
                    return this.cachedOutput;
                }
                else
                {
                    this.cachedOutput = new Output<char, EqualsSign<ParseMode.Realized>>(false, default, input);
                    return this.cachedOutput;
                }
            }
        }

        public sealed class OptionName<TMode> : IDeferredAstNode<char, OptionName<ParseMode.Realized>> where TMode : ParseMode
        {
            private readonly Func<IDeferredOutput2<char>> future;

            private Output<char, OptionName<ParseMode.Realized>>? cachedOutput;

            /*public OptionName(IEnumerable<AlphaNumeric> characters)
{
Characters = characters;
}*/

            public OptionName(Func<IDeferredOutput2<char>> future)
            {
                this.future = future;

                this.cachedOutput = null;
            }

            private OptionName(AtLeastOne<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, ParseMode.Realized> characters)
            {
            }

            public AtLeastOne<AlphaNumeric<TMode>, AlphaNumeric<ParseMode.Realized>, TMode> Characters
            {
                get
                {
                    return new AtLeastOne<AlphaNumeric<TMode>, AlphaNumeric<ParseMode.Realized>, TMode>(future, input => new AlphaNumeric<TMode>.A(input));
                }
            }

            public IOutput<char, OptionName<ParseMode.Realized>> Realize()
            {
                if (this.cachedOutput != null)
                {
                    return this.cachedOutput;
                }

                var output = this.Characters.Realize();
                if (output.Success)
                {
                    this.cachedOutput = new Output<char, OptionName<ParseMode.Realized>>(
                        true, 
                        new OptionName<ParseMode.Realized>(this.Characters.Realize().Parsed as AtLeastOne<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, ParseMode.Realized>),
                        output.Remainder);
                    return this.cachedOutput;
                }
                else
                {
                    this.cachedOutput = new Output<char, OptionName<ParseMode.Realized>>(false, default, output.Remainder);
                    return this.cachedOutput;
                }
            }
        }

        public sealed class OptionValue<TMode> : IDeferredAstNode<char, OptionValue<ParseMode.Realized>> where TMode : ParseMode
        {
            private readonly Func<IDeferredOutput2<char>> future;

            private Output<char, OptionValue<ParseMode.Realized>>? cachedOutput;

            /*private readonly IInput<char> input;

public OptionValue(IInput<char> input)
{
this.input = input;
}*/

            /*public OptionValue(IEnumerable<AlphaNumeric> characters)
            {
                Characters = characters;
            }*/

            public OptionValue(Func<IDeferredOutput2<char>> future)
            {
                this.future = future;

                this.cachedOutput = null;
            }

            private OptionValue(Many<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, ParseMode.Realized> characters)
            {
            }

            public Many<AlphaNumeric<TMode>, AlphaNumeric<ParseMode.Realized>, TMode> Characters
            {
                get
                {
                    return new Many<AlphaNumeric<TMode>, AlphaNumeric<ParseMode.Realized>, TMode>(this.future, input => new AlphaNumeric<TMode>.A(input));
                }
            }

            public IOutput<char, OptionValue<ParseMode.Realized>> Realize()
            {
                if (this.cachedOutput != null)
                {
                    return this.cachedOutput;
                }

                var output = this.Characters.Realize();
                if (output.Success)
                {
                    this.cachedOutput = new Output<char, OptionValue<ParseMode.Realized>>(
                        true,
                        new OptionValue<ParseMode.Realized>(this.Characters.Realize().Parsed as Many<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, ParseMode.Realized>),
                        output.Remainder);
                    return this.cachedOutput;
                }
                else
                {
                    this.cachedOutput = new Output<char, OptionValue<ParseMode.Realized>>(false, default, output.Remainder);
                    return this.cachedOutput;
                }

            }
        }

        public sealed class QueryOption<TMode> : IDeferredAstNode<char, QueryOption<ParseMode.Realized>> where TMode : ParseMode
        {
            private readonly Func<IDeferredOutput2<char>> future;

            private Output<char, QueryOption<ParseMode.Realized>>? cachedOutput;

            public QueryOption(Func<IDeferredOutput2<char>> future)
            {
                this.future = future;

                this.cachedOutput = null;
            }

            /*public QueryOption(OptionName name, EqualsSign equalsSign, OptionValue optionValue)
            {
                Name = name;
                EqualsSign = equalsSign;
                OptionValue = optionValue;
            }*/

            private QueryOption(OptionName<ParseMode.Realized> name, EqualsSign<ParseMode.Realized> equalsSign, OptionValue<ParseMode.Realized> optionValue)
            {
            }

            public OptionName<TMode> Name
            {
                get
                {
                    return new OptionName<TMode>(this.future);
                }
            }

            public EqualsSign<TMode> EqualsSign
            {
                get
                {
                    return new EqualsSign<TMode>(DeferredOutput2.ToPromise(this.Name.Realize));
                }
            }

            public OptionValue<TMode> OptionValue
            {
                get
                {
                    return new OptionValue<TMode>(DeferredOutput2.ToPromise(this.EqualsSign.Realize));
                }
            }

            public IOutput<char, QueryOption<ParseMode.Realized>> Realize()
            {
                if (this.cachedOutput != null)
                {
                    return this.cachedOutput;
                }

                var output = this.OptionValue.Realize();
                if (output.Success)
                {
                    this.cachedOutput = new Output<char, QueryOption<ParseMode.Realized>>(
                        
                        true, 
                        new QueryOption<ParseMode.Realized>(this.Name.Realize().Parsed, this.EqualsSign.Realize().Parsed, this.OptionValue.Realize().Parsed),
                        output.Remainder);

                    return this.cachedOutput;
                }
                else
                {
                    this.cachedOutput = new Output<char, QueryOption<ParseMode.Realized>>(false, default, output.Remainder);
                    return this.cachedOutput;
                }
            }
        }

        public sealed class QuestionMark<TMode> : IDeferredAstNode<char, QuestionMark<ParseMode.Realized>> where TMode : ParseMode
        {
            private readonly Func<IDeferredOutput2<char>> future;

            private Output<char, QuestionMark<ParseMode.Realized>>? cachedOutput;

            /*private QuestionMark()
{
}

public static QuestionMark Instance { get; } = new QuestionMark();*/

            public QuestionMark(Func<IDeferredOutput2<char>> future)
            {
                this.future = future;

                this.cachedOutput = null;
            }

            private QuestionMark()
            {
            }

            public static QuestionMark<ParseMode.Realized> Realized { get; } = new QuestionMark<ParseMode.Realized>();

            public IOutput<char, QuestionMark<ParseMode.Realized>> Realize()
            {
                if (this.cachedOutput != null)
                {
                    return this.cachedOutput;
                }

                var output = this.future();
                if (!output.Success)
                {
                    this.cachedOutput = new Output<char, QuestionMark<ParseMode.Realized>>(false, default, output.Remainder);
                    return this.cachedOutput;
                }

                var input = output.Remainder;

                if (input.Current == '?')
                {
                    this.cachedOutput = new Output<char, QuestionMark<ParseMode.Realized>>(true, QuestionMark<TMode>.Realized, input.Next());
                    return this.cachedOutput;
                }
                else
                {
                    this.cachedOutput = new Output<char, QuestionMark<ParseMode.Realized>>(false, default, input);
                    return this.cachedOutput;
                }
            }
        }

        public sealed class OdataUri<TMode> : IDeferredAstNode<char, OdataUri<ParseMode.Realized>> where TMode : ParseMode
        {
            private readonly Func<IDeferredOutput2<char>> future;

            private Output<char, OdataUri<ParseMode.Realized>>? cachedOutput;

            public OdataUri(Func<IDeferredOutput2<char>> future)
            {
                //// TODO add the type parameter check or hvae a static factory method that only returns the deferred type

                this.future = future;

                this.cachedOutput = null;
            }

            private OdataUri(
                AtLeastOne<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>, ParseMode.Realized> segments,
                QuestionMark<ParseMode.Realized> questionMark,
                Many<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>, ParseMode.Realized> queryOptions)
            {
            }

            public AtLeastOne<Segment<TMode>, Segment<ParseMode.Realized>, TMode> Segments //// TODO implement "at least one"
            {
                get
                {
                    return new AtLeastOne<Segment<TMode>, Segment<ParseMode.Realized>, TMode>(this.future, input => new Segment<TMode>(input));
                }
            }

            public QuestionMark<TMode> QuestionMark
            {
                get
                {
                    return new QuestionMark<TMode>(DeferredOutput2.ToPromise(this.Segments.Realize));
                }
            }

            public Many<QueryOption<TMode>, QueryOption<ParseMode.Realized>, TMode> QueryOptions
            {
                get
                {
                    return new Many<QueryOption<TMode>, QueryOption<ParseMode.Realized>, TMode>(DeferredOutput2.ToPromise(this.QuestionMark.Realize), input => new QueryOption<TMode>(input));
                }
            }

            public IOutput<char, OdataUri<ParseMode.Realized>> Realize()
            {
                if (this.cachedOutput != null)
                {
                    return this.cachedOutput;
                }

                var output = this.QueryOptions.Realize();
                if (output.Success)
                {
                    this.cachedOutput = new Output<char, OdataUri<ParseMode.Realized>>(
                        true,
                        new OdataUri<ParseMode.Realized>(
                            this.Segments.Realize().Parsed as AtLeastOne<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>, ParseMode.Realized>,
                            this.QuestionMark.Realize().Parsed,
                            this.QueryOptions.Realize().Parsed as Many<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>, ParseMode.Realized>),
                        output.Remainder);
                    return this.cachedOutput;
                }
                else
                {
                    this.cachedOutput = new Output<char, OdataUri<ParseMode.Realized>>(false, default, output.Remainder);
                    return this.cachedOutput;
                }
            }
        }

        /*public class OdataUri : DeferredOdataUri
        {
            public OdataUri(IEnumerable<Segment> segments, QuestionMark questionMark, IEnumerable<QueryOption> queryOptions)
                :base(new Lazy<IEnumerable<Segment>>(segments), new Lazy<QuestionMark>(questionMark), new Lazy<IEnumerable<QueryOption>>(queryOptions))
            {
            }
        }

        public class DeferredOdataUri : IDeferredAstNode<char, OdataUri>
        {
            private readonly Lazy<CombinatorParsingV3.IOutput<char, IEnumerable<Segment>>> segments;

            protected DeferredOdataUri(Lazy<IOutput<char, IEnumerable<Segment>>> segments, Lazy<QuestionMark> questionMark, Lazy<IEnumerable<QueryOption>> queryOptions)
            {
                this.segments = segments;
            }

            public DeferredOdataUri(IInput<char> input)
                : this(input, SegmentParser.Instance)
            {
            }

            public DeferredOdataUri(IInput<char> input, IParser<char, IEnumerable<Segment>> segmentParser, )
                : this(new Lazy<IEnumerable<Segment>>(() => segmentParser.Parse(input)))
            {
            }

            public IEnumerable<Segment> Segments
            {
                get
                {
                    if (segments.Value.Success)
                    {
                        return segments.Value.Parsed;
                    }
                    else
                    {
                        throw new Exception("TODO");
                    }
                }
            }

            public QuestionMark QuestionMark { get; }
            public IEnumerable<QueryOption> QueryOptions { get; }

            public IOutput<char, OdataUri> Realize()
            {
                var finalSegments = segments.Value.Parsed;
                if (!segments.Value.Success)
                {
                    return new DeferredOutput<OdataUri>(false, default);
                }

                // 

                return new DeferredOutput<OdataUri>(true, new OdataUri(finalSegments, ));
            }
        }*/

        /*public class UriParserV2 : IDeferredParser<char, OdataUri, DeferredOdataUri>
        {
            public DeferredOdataUri Parse(IInput<char> input)
            {
                return new DeferredOdataUri(
                    );
            }
        }*/
    }
}

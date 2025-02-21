namespace CombinatorParsingV3
{
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using System.Collections;
    using System.Threading;
    using static CombinatorParsingV3.V3ParserPlayground;
    using CombinatorParsingV1;
    using System.Runtime.CompilerServices;

    public sealed class Future<T>
    {
        private readonly Func<T> promise;

        private RealNullable<T> value;

        public Future(Func<T> promise)
        {
            this.promise = promise;

            this.value = default;
        }

        public T Value
        {
            get
            {
                if (!this.value.TryGetValue(out var value))
                {
                    value = this.promise();

                    this.value = value;
                }

                return value;
            }
        }

        public static implicit operator T(Future<T> future)
        {
            //// TODO is this implicit conversion a good idea?
            return future.Value;
        }

        public static implicit operator Future<T>(Func<T> promise)
        {
            return new Future<T>(promise);
        }
    }

    public static class Func
    {
        public static Func<TOutput> Lift<TInput, TOutput>(Func<TInput> inner, Func<TInput, TOutput> outer)
        {
            //// TODO i don't know if this is actually a lift
            return () => outer(inner());
        }

        public static Func<T> Close<T>(T value)
        {
            return () => value;
        }

        public readonly struct Closure<T>
        {
            private readonly T value;

            public Closure(T value)
            {
                //// TODO do you want something like this for better memory management in the `close` method?
                this.value = value;
            }

            public static implicit operator Func<T>(Closure<T> closure)
            {
                return () => closure.value;
            }
        }
    }

    public readonly struct RealNullable<T> //// TODO should you have a class variant so that no boxing occurs in those cases?
    {
        private readonly T value;

        private readonly bool hasValue;

        public RealNullable(T value)
        {
            this.value = value;
            this.hasValue = true;
        }

        public bool TryGetValue([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out T value)
        {
            value = this.value;
            return this.hasValue;
        }

        public static implicit operator RealNullable<T>(T value)
        {
            return new RealNullable<T>(value);
        }
    }

    public static partial class V3ParserPlayground
    {
        public static class ModelingOptionss
        {
            /// <summary>
            /// ISSUES:
            /// 
            /// runtime check that it's not initialied with `TMode` = `ParseMode.Realized`
            /// realized versions have the `realize` method defined on them, so callers can still call `realize` even if it's a no-op
            /// there's nothing that stops the use of `slash<parsemode.realized>` in cases where no instance is needed
            /// can't use singletons for realized nodes that are really singletons
            /// </summary>
            public static class Option1
            {
                public sealed class Slash<TMode> : IDeferredAstNode<char, Slash<ParseMode.Realized>> where TMode : ParseMode
                {
                    private readonly Future<IDeferredOutput<char>> previouslyParsedOutput;

                    private Future<IOutput<char, Slash<ParseMode.Realized>>> cachedOutput;

                    public Slash(Future<IDeferredOutput<char>> previouslyParsedOutput)
                    {
                        if (typeof(TMode) != typeof(ParseMode.Deferred))
                        {
                            throw new ArgumentException("TODO");
                        }

                        this.previouslyParsedOutput = previouslyParsedOutput;

                        this.cachedOutput = new Future<IOutput<char, Slash<ParseMode.Realized>>>(() => this.RealizeImpl());
                    }

                    private Slash(Future<IOutput<char, Slash<ParseMode.Realized>>> output)
                    {
                        this.cachedOutput = output;
                    }

                    public IOutput<char, Slash<ParseMode.Realized>> Realize()
                    {
                        return cachedOutput.Value;
                    }

                    private IOutput<char, Slash<ParseMode.Realized>> RealizeImpl()
                    {
                        var output = this.previouslyParsedOutput.Value;
                        if (!output.Success)
                        {
                            return new Output<char, Slash<ParseMode.Realized>>(false, default, output.Remainder);
                        }

                        var input = output.Remainder;

                        if (input.Current == '/')
                        {
                            return new Output<char, Slash<ParseMode.Realized>>(
                                true, 
                                new Slash<ParseMode.Realized>(this.cachedOutput), 
                                input.Next());
                        }
                        else
                        {
                            return new Output<char, Slash<ParseMode.Realized>>(false, default, input);
                        }
                    }
                }
            }

            /// <summary>
            /// ISSUES: 
            /// 
            /// realized versions have the `realize` method defined on them, so callers can still call `realize` even if it's a no-op
            /// there's nothing that stops the use of `slash<parsemode.realized>` in cases where no instance is needed
            /// `deferred` vs `realized` is handled by the static class, which requires an `internal` constructor; this means that we may accidentally create within our own code a realized node when it's actually deferred or a deferred node when it's actually realized
            /// requires two classes for every AST node, one that represents the node, and the other that guarantees that the wrong `parsemode` is not used
            /// can't use singletons for realized nodes that are really singletons
            /// </summary>
            public static class Option2
            {
                public static class Slash
                {
                    public static Slash<ParseMode.Deferred> Create(Future<IDeferredOutput<char>> previouslyParsedOutput)
                    {
                        return new Slash<ParseMode.Deferred>(previouslyParsedOutput);
                    }
                }

                public sealed class Slash<TMode> : IDeferredAstNode<char, Slash<ParseMode.Realized>> where TMode : ParseMode
                {
                    private readonly Future<IDeferredOutput<char>> previouslyParsedOutput;

                    private Future<IOutput<char, Slash<ParseMode.Realized>>> cachedOutput;

                    internal Slash(Future<IDeferredOutput<char>> previouslyParsedOutput)
                    {
                        System.Diagnostics.Debug.Assert(typeof(TMode) == typeof(ParseMode.Deferred));

                        this.previouslyParsedOutput = previouslyParsedOutput;

                        this.cachedOutput = new Future<IOutput<char, Slash<ParseMode.Realized>>>(() => this.RealizeImpl());
                    }

                    private Slash(Future<IOutput<char, Slash<ParseMode.Realized>>> output)
                    {
                        this.cachedOutput = output;
                    }

                    public IOutput<char, Slash<ParseMode.Realized>> Realize()
                    {
                        return cachedOutput.Value;
                    }

                    private IOutput<char, Slash<ParseMode.Realized>> RealizeImpl()
                    {
                        var output = this.previouslyParsedOutput.Value;
                        if (!output.Success)
                        {
                            return new Output<char, Slash<ParseMode.Realized>>(false, default, output.Remainder);
                        }

                        var input = output.Remainder;

                        if (input.Current == '/')
                        {
                            return new Output<char, Slash<ParseMode.Realized>>(
                                true,
                                new Slash<ParseMode.Realized>(this.cachedOutput),
                                input.Next());
                        }
                        else
                        {
                            return new Output<char, Slash<ParseMode.Realized>>(false, default, input);
                        }
                    }
                }
            }

            /// <summary>
            /// ISSUES:
            /// 
            /// requies two classes per AST node, and the two classes both need to have consistent structure throughout the entire tree
            /// </summary>
            public static class Option3
            {
                public sealed class DeferredSlash : IDeferredAstNode<char, RealizedSlash>
                {
                    private readonly Future<IDeferredOutput<char>> previouslyParsedOutput;

                    private Future<IOutput<char, RealizedSlash>> cachedOutput;

                    public DeferredSlash(Future<IDeferredOutput<char>> previouslyParsedOutput)
                    {
                        this.previouslyParsedOutput = previouslyParsedOutput;

                        this.cachedOutput = new Future<IOutput<char, RealizedSlash>>(() => this.RealizeImpl());
                    }

                    public IOutput<char, RealizedSlash> Realize()
                    {
                        return cachedOutput.Value;
                    }

                    private IOutput<char, RealizedSlash> RealizeImpl()
                    {
                        var output = this.previouslyParsedOutput.Value;
                        if (!output.Success)
                        {
                            return new Output<char, RealizedSlash>(false, default, output.Remainder);
                        }

                        var input = output.Remainder;

                        if (input.Current == '/')
                        {
                            return new Output<char, RealizedSlash>(
                                true,
                                RealizedSlash.Instance,
                                input.Next());
                        }
                        else
                        {
                            return new Output<char, RealizedSlash>(false, default, input);
                        }
                    }
                }

                public sealed class RealizedSlash
                {
                    private RealizedSlash()
                    {
                    }

                    public static RealizedSlash Instance { get; } = new RealizedSlash();
                }
            }

            /// <summary>
            /// ISSUES: 
            /// 
            /// realized versions have the `realize` method defined on them, so callers can still call `realize` even if it's a no-op
            /// there's nothing that stops the use of `slash<parsemode.realized>` in cases where no instance is needed
            /// can't use singletons for realized nodes that are really singletons
            /// it's weird that you can say `Slash<ParseMode.Realized>.Create(...)` and you'll get a deferred node
            /// </summary>
            public static class Option4
            {
                public sealed class Slash<TMode> : IDeferredAstNode<char, Slash<ParseMode.Realized>> where TMode : ParseMode
                {
                    public static Slash<ParseMode.Deferred> Create(Future<IDeferredOutput<char>> previouslyParsedOutput)
                    {
                        return new Slash<ParseMode.Deferred>(previouslyParsedOutput);
                    }

                    private readonly Future<IDeferredOutput<char>> previouslyParsedOutput;

                    private Future<IOutput<char, Slash<ParseMode.Realized>>> cachedOutput;

                    private Slash(Future<IDeferredOutput<char>> previouslyParsedOutput)
                    {
                        this.previouslyParsedOutput = previouslyParsedOutput;

                        this.cachedOutput = new Future<IOutput<char, Slash<ParseMode.Realized>>>(() => this.RealizeImpl());
                    }

                    private Slash(Future<IOutput<char, Slash<ParseMode.Realized>>> output)
                    {
                        this.cachedOutput = output;
                    }

                    public IOutput<char, Slash<ParseMode.Realized>> Realize()
                    {
                        return cachedOutput.Value;
                    }

                    private IOutput<char, Slash<ParseMode.Realized>> RealizeImpl()
                    {
                        var output = this.previouslyParsedOutput.Value;
                        if (!output.Success)
                        {
                            return new Output<char, Slash<ParseMode.Realized>>(false, default, output.Remainder);
                        }

                        var input = output.Remainder;

                        if (input.Current == '/')
                        {
                            return new Output<char, Slash<ParseMode.Realized>>(
                                true,
                                new Slash<ParseMode.Realized>(this.cachedOutput),
                                input.Next());
                        }
                        else
                        {
                            return new Output<char, Slash<ParseMode.Realized>>(false, default, input);
                        }
                    }
                }
            }

            /// <summary>
            /// ISSUES: 
            /// 
            /// realized versions have the `realize` method defined on them, so callers can still call `realize` even if it's a no-op
            /// there's nothing that stops the use of `slash<parsemode.realized>` in cases where no instance is needed
            /// can't use singletons for realized nodes that are really singletons
            /// two classes per AST node
            /// </summary>
            public static class Option5
            {
                public static class Slash
                {
                    public static Slash<ParseMode.Deferred> Create(Future<IDeferredOutput<char>> previouslyParsedOutput)
                    {
                        return Slash<ParseMode.Deferred>.Create(previouslyParsedOutput);
                    }
                }

                public sealed class Slash<TMode> : IDeferredAstNode<char, Slash<ParseMode.Realized>> where TMode : ParseMode
                {
                    internal static Slash<ParseMode.Deferred> Create(Future<IDeferredOutput<char>> previouslyParsedOutput)
                    {
                        return new Slash<ParseMode.Deferred>(previouslyParsedOutput);
                    }

                    private readonly Future<IDeferredOutput<char>> previouslyParsedOutput;

                    private Future<IOutput<char, Slash<ParseMode.Realized>>> cachedOutput;

                    private Slash(Future<IDeferredOutput<char>> previouslyParsedOutput)
                    {
                        this.previouslyParsedOutput = previouslyParsedOutput;

                        this.cachedOutput = new Future<IOutput<char, Slash<ParseMode.Realized>>>(() => this.RealizeImpl());
                    }

                    private Slash(Future<IOutput<char, Slash<ParseMode.Realized>>> output)
                    {
                        this.cachedOutput = output;
                    }

                    public IOutput<char, Slash<ParseMode.Realized>> Realize()
                    {
                        return cachedOutput.Value;
                    }

                    private IOutput<char, Slash<ParseMode.Realized>> RealizeImpl()
                    {
                        var output = this.previouslyParsedOutput.Value;
                        if (!output.Success)
                        {
                            return new Output<char, Slash<ParseMode.Realized>>(false, default, output.Remainder);
                        }

                        var input = output.Remainder;

                        if (input.Current == '/')
                        {
                            return new Output<char, Slash<ParseMode.Realized>>(
                                true,
                                new Slash<ParseMode.Realized>(this.cachedOutput),
                                input.Next());
                        }
                        else
                        {
                            return new Output<char, Slash<ParseMode.Realized>>(false, default, input);
                        }
                    }
                }
            }
        }

        public sealed class Slash<TMode> : IDeferredAstNode<char, Slash<ParseMode.Realized>> where TMode : ParseMode
        {
            private readonly Future<IDeferredOutput<char>> previouslyParsedOutput;

            private readonly Future<IOutput<char, Slash<ParseMode.Realized>>> cachedOutput;

            public Slash(Future<IDeferredOutput<char>> previouslyParsedOutput)
            {
                if (typeof(TMode) != typeof(ParseMode.Deferred))
                {
                    throw new ArgumentException("TODO");
                }

                this.previouslyParsedOutput = previouslyParsedOutput;

                this.cachedOutput = new Future<IOutput<char, Slash<ParseMode.Realized>>>(() => this.RealizeImpl());
            }

            private Slash(Future<IOutput<char, Slash<ParseMode.Realized>>> output)
            {
                this.cachedOutput = output;
            }

            public IOutput<char, Slash<ParseMode.Realized>> Realize()
            {
                return cachedOutput.Value;
            }

            private IOutput<char, Slash<ParseMode.Realized>> RealizeImpl()
            {
                var output = this.previouslyParsedOutput.Value;
                if (!output.Success)
                {
                    return new Output<char, Slash<ParseMode.Realized>>(false, default, output.Remainder);
                }

                var input = output.Remainder;

                if (input.Current == '/')
                {
                    return new Output<char, Slash<ParseMode.Realized>>(
                        true,
                        new Slash<ParseMode.Realized>(this.cachedOutput),
                        input.Next());
                }
                else
                {
                    return new Output<char, Slash<ParseMode.Realized>>(false, default, input);
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
                return this.DerivedRealize();
            }

            protected abstract IOutput<char, AlphaNumeric<ParseMode.Realized>> DerivedRealize();

            public sealed class A : AlphaNumeric<TMode>, IDeferredAstNode<char, AlphaNumeric<ParseMode.Realized>.A>
            {
                private readonly Future<IDeferredOutput<char>> future;

                private readonly Future<IOutput<char, AlphaNumeric<ParseMode.Realized>.A>> cachedOutput;

                public A(Future<IDeferredOutput<char>> future)
                {
                    if (typeof(TMode) != typeof(ParseMode.Deferred))
                    {
                        throw new ArgumentException("tODO");
                    }

                    this.future = future;

                    this.cachedOutput = new Future<IOutput<char, AlphaNumeric<ParseMode.Realized>.A>>(() => this.RealizeImpl());
                }

                private A(Future<IOutput<char, AlphaNumeric<ParseMode.Realized>.A>> cachedOutput)
                {
                    this.cachedOutput = cachedOutput;
                }

                public new IOutput<char, AlphaNumeric<ParseMode.Realized>.A> Realize()
                {
                    return this.cachedOutput.Value;
                }

                private IOutput<char, AlphaNumeric<ParseMode.Realized>.A> RealizeImpl()
                {
                    var output = this.future.Value;
                    if (!output.Success)
                    {
                        return new Output<char, AlphaNumeric<ParseMode.Realized>.A>(false, default, output.Remainder);
                    }

                    var input = output.Remainder;
                    if (input == null)
                    {
                        return new Output<char, AlphaNumeric<ParseMode.Realized>.A>(false, default, input);
                    }

                    if (input.Current == 'A')
                    {
                        return new Output<char, AlphaNumeric<ParseMode.Realized>.A>(true, new AlphaNumeric<ParseMode.Realized>.A(this.cachedOutput), input.Next());
                    }
                    else
                    {
                        return new Output<char, AlphaNumeric<ParseMode.Realized>.A>(false, default, input);
                    }
                }

                protected override IOutput<char, AlphaNumeric<ParseMode.Realized>> DerivedRealize()
                {
                    return this.Realize();
                }
            }
        }

        public sealed class AtLeastOne<TDeferredAstNode, TRealizedAstNode, TMode> : IDeferredAstNode<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode> where TMode : ParseMode
        {
            private readonly Future<IDeferredOutput<char>> future;

            private readonly Func<Future<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory;

            private readonly Future<TDeferredAstNode> __1;

            private readonly Future<IOutput<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput;

            public AtLeastOne(
                Future<IDeferredOutput<char>> future, 
                Func<Future<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory)
            {
                this.future = future;
                this.nodeFactory = nodeFactory;

                this.__1 = this.future.Lift(this.nodeFactory);

                this.cachedOutput = new Future<IOutput<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>>(() => this.RealizeImpl());
            }

            private AtLeastOne(
                TRealizedAstNode _1, 
                ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> node,
                Future<IOutput<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput)
            {
            }
            
            public TDeferredAstNode _1
            {
                get
                {
                    return this.nodeFactory(this.future);
                }
            }

            /*public OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode> _2
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
            }*/

            public ManyNode<TDeferredAstNode, TRealizedAstNode, TMode> Node
            {
                get
                {
                    return new ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>(Func.Lift(this._1.Realize, DeferredOutput.Create)/*() => DeferredOutput.Create(this._1.Realize())*/, this.nodeFactory);
                }
            }

            private IOutput<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> RealizeImpl()
            {
            }

            public IOutput<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> Realize()
            {
                if (this.cachedOutput != null)
                {
                    return this.cachedOutput;
                }

                var output = this.Node.Realize();
                if (output.Success)
                {
                    this.cachedOutput = new Output<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        true,
                        new AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                            this._1.Realize().Parsed,
                            output.Parsed),
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
            private readonly Func<Future<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory;

            private readonly Future<IDeferredOutput<char>> future;

            private Output<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>? cachedOutput;

            public Many(Future<IDeferredOutput<char>> future, Func<Future<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory)
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
            private readonly Future<IDeferredOutput<char>> future;
            private readonly Func<Future<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory;

            private Output<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>? cachedOutput;

            public ManyNode(Future<IDeferredOutput<char>> future, Func<Future<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory)
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
                    return new ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>(Func.Lift(this.Element.Realize, DeferredOutput.Create), this.nodeFactory);
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

                if (realizedElement.Parsed.TryGetValue(out var parsed))
                {
                    var realizedNext = this.Next.Realize();
                    // *this* instance is the "dependency" for `next`, so we can only have success cases
                    this.cachedOutput = new Output<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        true,
                        new ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                            new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(parsed),
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
            private readonly Future<IDeferredOutput<char>> future;
            private readonly Func<Future<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory;

            private readonly RealNullable<RealNullable<TRealizedAstNode>> value;

            private Output<char, RealNullable<TRealizedAstNode>>? cachedOutput;

            public OptionalNode(Future<IDeferredOutput<char>> future, Func<Future<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory)
            {
                this.future = future;
                this.nodeFactory = nodeFactory;

                this.cachedOutput = null;
            }

            internal OptionalNode(RealNullable<TRealizedAstNode> value)
            {
                this.value = value;
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
                IDeferredOutput<char> output = null;
                var node = this.nodeFactory(new Future<IDeferredOutput<char>>(() => output = this.future.Value));

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

                if (this.value.TryGetValue(out var value))
                {
                    this.cachedOutput = new Output<char, RealNullable<TRealizedAstNode>>(true, value, null);
                    return this.cachedOutput;
                }

                var deferredOutput = this.future.Value;
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

        public sealed class Segment<TMode> : IDeferredAstNode<char, Segment<ParseMode.Realized>> where TMode : ParseMode
        {
            ////private readonly IParser<char, Segment> parser;
            private readonly Future<IDeferredOutput<char>> future;

            ////private Slash slash;
            ////private IEnumerable<AlphaNumeric> characters;

            ////private bool deferred;

            private Output<char, Segment<ParseMode.Realized>>? cachedOutput;

            public Segment(Future<IDeferredOutput<char>> future)
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
                        Func.Lift(this.Slash.Realize, DeferredOutput.Create),
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
            private readonly Future<IDeferredOutput<char>> future;

            private Output<char, EqualsSign<ParseMode.Realized>>? cachedOutput;

            /*public EqualsSign(IInput<char> input)
            {
                this.input = input;
            }*/

            public EqualsSign(Future<IDeferredOutput<char>> future)
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

                var output = this.future.Value;
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
            private readonly Future<IDeferredOutput<char>> future;

            private Output<char, OptionName<ParseMode.Realized>>? cachedOutput;

            /*public OptionName(IEnumerable<AlphaNumeric> characters)
{
Characters = characters;
}*/

            public OptionName(Future<IDeferredOutput<char>> future)
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
            private readonly Future<IDeferredOutput<char>> future;

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

            public OptionValue(Future<IDeferredOutput<char>> future)
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
            private readonly Future<IDeferredOutput<char>> future;

            private Output<char, QueryOption<ParseMode.Realized>>? cachedOutput;

            public QueryOption(Future<IDeferredOutput<char>> future)
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
                    return new EqualsSign<TMode>(Func.Lift(this.Name.Realize, DeferredOutput.Create));
                }
            }

            public OptionValue<TMode> OptionValue
            {
                get
                {
                    return new OptionValue<TMode>(DeferredOutput.ToPromise(this.EqualsSign.Realize));
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
            private readonly Future<IDeferredOutput<char>> future;

            private Output<char, QuestionMark<ParseMode.Realized>>? cachedOutput;

            /*private QuestionMark()
{
}

public static QuestionMark Instance { get; } = new QuestionMark();*/

            public QuestionMark(Future<IDeferredOutput<char>> future)
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

                var output = this.future.Value;
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
            private readonly Future<IDeferredOutput<char>> future;

            private Output<char, OdataUri<ParseMode.Realized>>? cachedOutput;

            public OdataUri(Future<IDeferredOutput<char>> future)
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
                    return new QuestionMark<TMode>(DeferredOutput.ToPromise(this.Segments.Realize));
                }
            }

            public Many<QueryOption<TMode>, QueryOption<ParseMode.Realized>, TMode> QueryOptions
            {
                get
                {
                    return new Many<QueryOption<TMode>, QueryOption<ParseMode.Realized>, TMode>(DeferredOutput.ToPromise(this.QuestionMark.Realize), input => new QueryOption<TMode>(input));
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

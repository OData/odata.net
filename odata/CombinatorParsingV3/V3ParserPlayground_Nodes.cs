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
    using System.Reflection.Metadata.Ecma335;
    using System.Net.Cache;

    public interface IFuture<out T> //// TODO i'm not sure i like having an interface...
    {
        T Value { get; }
    }

    public sealed class Future<T> : IFuture<T>
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

        public sealed class AlphaNumericHolder : IDeferredAstNode<char, AlphaNumeric<ParseMode.Realized>>
        {
            private readonly Future<IDeferredOutput<char>> future;

            private readonly IFuture<IOutput<char, AlphaNumeric<ParseMode.Realized>>> cachedOutput;

            public AlphaNumericHolder(Future<IDeferredOutput<char>> future)
            {
                this.future = future;

                this.cachedOutput = new Future<IOutput<char, AlphaNumeric<ParseMode.Realized>>>(this.RealizeImpl);
            }

            public AlphaNumericHolder(IFuture<IOutput<char, AlphaNumeric<ParseMode.Realized>>> cachedOutput)
            {
                this.cachedOutput = cachedOutput;
            }

            public IOutput<char, AlphaNumeric<ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IOutput<char, AlphaNumeric<ParseMode.Realized>> RealizeImpl()
            {
                var a = new AlphaNumeric<ParseMode.Deferred>.A(this.future).Realize();
                if (a.Success)
                {
                    return a;
                }

                var c = new AlphaNumeric<ParseMode.Deferred>.C(this.future).Realize();
                if (c.Success)
                {
                    return c;
                }

                return new Output<char, AlphaNumeric<ParseMode.Realized>>(false, default, this.future.Value.Remainder);
            }
        }

        public abstract class AlphaNumeric<TMode> : IDeferredAstNode<char, AlphaNumeric<ParseMode.Realized>>, IFromRealizedable<AlphaNumericHolder>
            where TMode : ParseMode
        {
            private AlphaNumeric()
            {
            }

            public IOutput<char, AlphaNumeric<ParseMode.Realized>> Realize()
            {
                return this.DerivedRealize();
            }

            protected abstract IOutput<char, AlphaNumeric<ParseMode.Realized>> DerivedRealize();

            public abstract AlphaNumericHolder Convert();

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

                public override AlphaNumericHolder Convert()
                {
                    if (typeof(TMode) == typeof(ParseMode.Deferred))
                    {
                        return new AlphaNumericHolder(this.future);
                    }
                    else
                    {
                        return new AlphaNumericHolder(this.cachedOutput);
                    }
                }
            }

            public sealed class C : AlphaNumeric<TMode>, IDeferredAstNode<char, AlphaNumeric<ParseMode.Realized>.C>
            {
                private readonly Future<IDeferredOutput<char>> future;

                private readonly Future<IOutput<char, AlphaNumeric<ParseMode.Realized>.C>> cachedOutput;

                public C(Future<IDeferredOutput<char>> future)
                {
                    if (typeof(TMode) != typeof(ParseMode.Deferred))
                    {
                        throw new ArgumentException("tODO");
                    }

                    this.future = future;

                    this.cachedOutput = new Future<IOutput<char, AlphaNumeric<ParseMode.Realized>.C>>(() => this.RealizeImpl());
                }

                private C(Future<IOutput<char, AlphaNumeric<ParseMode.Realized>.C>> cachedOutput)
                {
                    this.cachedOutput = cachedOutput;
                }

                public new IOutput<char, AlphaNumeric<ParseMode.Realized>.C> Realize()
                {
                    return this.cachedOutput.Value;
                }

                private IOutput<char, AlphaNumeric<ParseMode.Realized>.C> RealizeImpl()
                {
                    var output = this.future.Value;
                    if (!output.Success)
                    {
                        return new Output<char, AlphaNumeric<ParseMode.Realized>.C>(false, default, output.Remainder);
                    }

                    var input = output.Remainder;
                    if (input == null)
                    {
                        return new Output<char, AlphaNumeric<ParseMode.Realized>.C>(false, default, input);
                    }

                    if (input.Current == 'C')
                    {
                        return new Output<char, AlphaNumeric<ParseMode.Realized>.C>(true, new AlphaNumeric<ParseMode.Realized>.C(this.cachedOutput), input.Next());
                    }
                    else
                    {
                        return new Output<char, AlphaNumeric<ParseMode.Realized>.C>(false, default, input);
                    }
                }

                protected override IOutput<char, AlphaNumeric<ParseMode.Realized>> DerivedRealize()
                {
                    return this.Realize();
                }

                public override AlphaNumericHolder Convert()
                {
                    if (typeof(TMode) == typeof(ParseMode.Deferred))
                    {
                        return new AlphaNumericHolder(this.future);
                    }
                    else
                    {
                        return new AlphaNumericHolder(this.cachedOutput);
                    }
                }
            }
        }

        public static TDeferredAstNode FromRealized<TDeferredAstNode, TRealizedAstNode>(TRealizedAstNode realizedAstNode)
            where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode>
        {
            //// TODO make this a static interface method on ideferrednode probably?
            ////return default!;

            if (realizedAstNode is IFromRealizedable<TDeferredAstNode> fromRealizedable)
            {
                return fromRealizedable.Convert();
            }

            return default!;
        }

        public interface IFromRealizedable<TDeferredAstNode>
        {
            TDeferredAstNode Convert();
        }

        public sealed class AtLeastOne<TDeferredAstNode, TRealizedAstNode, TMode> : 
            IDeferredAstNode<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>
            where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode> 
            where TMode : ParseMode
        {
            private readonly Future<IDeferredOutput<char>> future;
            private readonly Func<Future<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory;

            private readonly Future<TDeferredAstNode> __1;
            private readonly Future<ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>> node;

            private readonly 
                Future<IOutput<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> 
                cachedOutput;

            public AtLeastOne(
                Future<IDeferredOutput<char>> future,
                Func<Future<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory)
            {
                this.future = future;
                this.nodeFactory = nodeFactory;

                this.__1 = new Future<TDeferredAstNode>(
                    () => this.nodeFactory(this.future));
                this.node = new Future<ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>>(
                    () => new ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>(
                        Func.Lift(this._1.Realize, DeferredOutput.Create), 
                        this.nodeFactory));

                this.cachedOutput = new Future
                    <
                        IOutput<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>
                    >(
                        () => this.RealizeImpl());
            }

            private AtLeastOne(
                TRealizedAstNode _1,
                ManyNode<TDeferredAstNode, TRealizedAstNode, TMode> node,
                Future<IOutput<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput)
            {
                this.__1 = new Future<TDeferredAstNode>(
                    () => FromRealized<TDeferredAstNode, TRealizedAstNode>(_1));
                this.node = new Future<ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>>(() => node);

                this.cachedOutput = cachedOutput;
            }

            public TDeferredAstNode _1 //// TODO does the type of this property actually make sense once realized?
            {
                get
                {
                    return this.__1;
                }
            }

            public ManyNode<TDeferredAstNode, TRealizedAstNode, TMode> Node
            {
                get
                {
                    return this.node;
                }
            }

            public IOutput<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IOutput<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> RealizeImpl()
            {
                var output = this.Node.Realize();
                if (output.Success)
                {
                    return new Output<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        true,
                        new AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                            this._1.Realize().Parsed,
                            output.Parsed,
                            this.cachedOutput),
                        output.Remainder);
                }
                else
                {
                    return new Output<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        false, 
                        default, 
                        output.Remainder);
                }
            }
        }

        public sealed class Many<TDeferredAstNode, TRealizedAstNode, TMode> 
            : IDeferredAstNode<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>
            where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode>
            where TMode : ParseMode
        {
            private readonly Future<IDeferredOutput<char>> future;
            private readonly Func<Future<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory;

            private readonly Future<ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>> node;

            private readonly Future<IOutput<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput;

            public Many(Future<IDeferredOutput<char>> future, Func<Future<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory)
            {
                this.future = future;
                this.nodeFactory = nodeFactory;

                this.node = new Future<ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>>(
                    () => new ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>(this.future, this.nodeFactory));

                this.cachedOutput = new Future<IOutput<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>>(
                    () => this.RealizeImpl());
            }

            private Many(
                ManyNode<TDeferredAstNode, TRealizedAstNode, TMode> node, 
                Future<IOutput<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput)
            {
                this.node = new Future<ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>>(() => node);

                this.cachedOutput = cachedOutput;
            }

            public ManyNode<TDeferredAstNode, TRealizedAstNode, TMode> Node
            {
                get
                {
                    return this.node;
                }
            }

            public IOutput<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IOutput<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> RealizeImpl()
            {
                var output = this.Node.Realize();
                if (output.Success)
                {
                    return new Output<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        true,
                        new Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                            output.Parsed,
                            this.cachedOutput),
                        output.Remainder);
                }
                else
                {
                    // if the optional failed to parse, it means that its dependencies failed to parse
                    return new Output<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        false, 
                        default, 
                        output.Remainder);
                }
            }
        }

        public sealed class ManyNode<TDeferredAstNode, TRealizedAstNode, TMode> : IDeferredAstNode<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode> where TMode : ParseMode
        {
            private readonly Future<IDeferredOutput<char>> future;
            private readonly Func<Future<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory;

            private readonly Future<OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode>> element;
            private readonly Future<ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>> next;

            private readonly Future<IOutput<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput;

            public ManyNode(
                Future<IDeferredOutput<char>> future, 
                Func<Future<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory)
            {
                this.future = future;
                this.nodeFactory = nodeFactory;

                this.element = new Future<OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode>>(
                    () => new OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode>(this.future, this.nodeFactory));
                this.next = new Future<ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>>(
                    () => 
                        new ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>(
                            Func.Lift(this.Element.Realize, DeferredOutput.Create), 
                    this.nodeFactory));

                this.cachedOutput = new Future
                    <
                        IOutput<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>
                    >(
                        () => this.RealizeImpl());
            }

            private static ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> GetTerminalRealizedNode(
                Future<IOutput<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput,
                OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> element)
            {
                return new ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                    element, 
                    () => GetTerminalRealizedNode(cachedOutput, element),
                    cachedOutput);
            }

            private ManyNode(
                OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode> element, 
                Func<ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>> next, 
                Future<IOutput<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput)
            {
                this.element = new Future<OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode>>(() => element);
                this.next = new Future<ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>>(() => next());

                this.cachedOutput = cachedOutput;
            }

            public OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode> Element
            {
                get
                {
                    return this.element;
                }
            }

            /// <summary>
            /// TODO realize should only be called on this if <see cref="Element"/> has a value
            /// </summary>
            public ManyNode<TDeferredAstNode, TRealizedAstNode, TMode> Next
            {
                get
                {
                    return this.next;
                }
            }

            public IOutput<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IOutput<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> RealizeImpl()
            {
                var realizedElement = this.Element.Realize();
                if (!realizedElement.Success)
                {
                    // this means that the nullable parsing *didn't succeed*, which only happens if its dependencies couldn't be parsed; this means that we also can't succeed in parsing
                    return new Output<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        false,
                        default, 
                        realizedElement.Remainder);
                }

                if (realizedElement.Parsed.Value.TryGetValue(out var parsed))
                {
                    var realizedNext = this.Next.Realize();
                    // *this* instance is the "dependency" for `next`, so we can only have success cases
                    return new Output<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        true,
                        new ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                            realizedElement.Parsed,
                            () => realizedNext.Parsed,
                            this.cachedOutput),
                        realizedNext.Remainder);
                }
                else
                {
                    return new Output<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        true,
                        new ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                            realizedElement.Parsed,
                            () => GetTerminalRealizedNode(this.cachedOutput, realizedElement.Parsed),
                            this.cachedOutput),
                        realizedElement.Remainder);
                }
            }
        }

        public sealed class OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode> :
            IDeferredAstNode<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>
            where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode>
            where TMode : ParseMode
        {
            private readonly Future<IDeferredOutput<char>> future;
            private readonly Func<Future<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory;

            private readonly RealNullable<RealNullable<TRealizedAstNode>> value;

            private readonly Future<IOutput<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput;

            public OptionalNode(
                Future<IDeferredOutput<char>> future,
                Func<Future<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory)
            {
                this.future = future;
                this.nodeFactory = nodeFactory;

                this.value = new RealNullable<RealNullable<TRealizedAstNode>>();

                this.cachedOutput = new Future<IOutput<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>>(this.RealizeImpl);
            }

            private OptionalNode(RealNullable<TRealizedAstNode> value, Future<IOutput<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput)
            {
                this.value = new RealNullable<RealNullable<TRealizedAstNode>>(value);
                this.cachedOutput = cachedOutput;
            }

            public RealNullable<TRealizedAstNode> Value
            {
                get
                {
                    if (this.value.TryGetValue(out var realizedValue))
                    {
                        return realizedValue;
                    }

                    throw new InvalidOperationException("can't get the value on a non-realized optionalnode");
                }
            }

            public IOutput<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IOutput<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> RealizeImpl()
            {
                var deferredOutput = this.future.Value;
                if (!deferredOutput.Success)
                {
                    return new Output<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(false, default, deferredOutput.Remainder);
                }

                var value = this.nodeFactory(this.future);
                var output = value.Realize();
                if (output.Success)
                {
                    return new Output<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        true,
                        new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                            new RealNullable<TRealizedAstNode>(output.Parsed),
                            this.cachedOutput),
                        output.Remainder);
                }
                else
                {
                    return new Output<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        true,
                        new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                            new RealNullable<TRealizedAstNode>(),
                            this.cachedOutput),
                        output.Remainder);
                }
            }
        }

        /*public sealed class DeferredOptionalNode<TDeferredAstNode, TRealizedAstNode> :
            IDeferredAstNode<char, RealizedOptionalNode<TRealizedAstNode>>
            where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode>
        {
            private readonly Future<IDeferredOutput<char>> future;
            private readonly Func<Future<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory;

            internal readonly Future<IOutput<char, RealizedOptionalNode<TRealizedAstNode>>> cachedOutput; //// TODO this shouldn't be internal

            public DeferredOptionalNode(
                Future<IDeferredOutput<char>> future,
                Func<Future<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory)
            {
                this.future = future;
                this.nodeFactory = nodeFactory;

                this.cachedOutput = new Future<IOutput<char, RealizedOptionalNode<TRealizedAstNode>>>(this.RealizeImpl);
            }

            public IOutput<char, RealizedOptionalNode<TRealizedAstNode>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IOutput<char, RealizedOptionalNode<TRealizedAstNode>> RealizeImpl()
            {
                var deferredOutput = this.future.Value;
                if (!deferredOutput.Success)
                {
                    return new Output<char, RealizedOptionalNode<TRealizedAstNode>>(false, default, deferredOutput.Remainder);
                }

                var value = this.nodeFactory(this.future);
                var output = value.Realize();
                if (output.Success)
                {
                    return new Output<char, RealizedOptionalNode<TRealizedAstNode>>(
                        true,
                        new RealizedOptionalNode<TRealizedAstNode>(output.Parsed),
                        output.Remainder);
                }
                else
                {
                    return new Output<char, RealizedOptionalNode<TRealizedAstNode>>(
                        true,
                        new RealizedOptionalNode<TRealizedAstNode>(new RealNullable<TRealizedAstNode>()),
                        output.Remainder);
                }
            }
        }

        public sealed class RealizedOptionalNode<TAstNode>
        {
            public RealizedOptionalNode(RealNullable<TAstNode> value)
            {
                this.Value = value;
            }

            public RealNullable<TAstNode> Value { get; }
        }*/

        /*public sealed class OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode> 
            : IDeferredAstNode<char, OptionalNode<TRealizedAstNode, TRealizedAstNode, ParseMode.Realized>> 
            where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode>
            where TRealizedAstNode : IDeferredAstNode<char, TRealizedAstNode>
            where TMode : ParseMode
        {
            private readonly Future<IDeferredOutput<char>> future;
            private readonly Func<Future<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory;

            private readonly Future<TDeferredAstNode> value;

            internal readonly Future<IOutput<char, RealNullable<TRealizedAstNode>>> cachedOutput; //// TODO this shouldn't be internal

            public OptionalNode(
                Future<IDeferredOutput<char>> future, 
                Func<Future<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory)
            {
                this.future = future;
                this.nodeFactory = nodeFactory;

                this.value = new Future<TDeferredAstNode>(() => this.nodeFactory(this.future));

                this.cachedOutput = new Future<IOutput<char, RealNullable<TRealizedAstNode>>>(this.RealizeImpl);
            }

            internal OptionalNode(
                TDeferredAstNode value, 
                Future<IOutput<char, RealNullable<TRealizedAstNode>>> cachedOutput)
            {
                this.value = new Future<TDeferredAstNode>(() => value);

                this.cachedOutput = cachedOutput;
            }

            public TDeferredAstNode Value
            {
                get
                {
                    //// TODO this always returns deferred; maybe you should have many use the realized node for `tdeferredastnode` when this is realized?
                    return this.value;
                }
            }

            public IOutput<char, RealNullable<TRealizedAstNode>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IOutput<char, RealNullable<TRealizedAstNode>> RealizeImpl()
            {
                var deferredOutput = this.future.Value;
                if (!deferredOutput.Success)
                {
                    return new Output<char, RealNullable<TRealizedAstNode>>(false, default, deferredOutput.Remainder);
                }

                var output = this.Value.Realize();
                if (output.Success)
                {
                    return new Output<char, RealNullable<TRealizedAstNode>>(
                        true, 
                        new RealNullable<TRealizedAstNode>(output.Parsed), 
                        output.Remainder);
                }
                else
                {
                    return new Output<char, RealNullable<TRealizedAstNode>>(
                        true, 
                        new RealNullable<TRealizedAstNode>(), 
                        output.Remainder);
                }
            }

            IOutput<char, OptionalNode<TRealizedAstNode, TRealizedAstNode, ParseMode.Realized>> IDeferredAstNode<char, OptionalNode<TRealizedAstNode, TRealizedAstNode, ParseMode.Realized>>.Realize()
            {
                throw new NotImplementedException();
            }
        }*/

        public sealed class Segment<TMode> : IDeferredAstNode<char, Segment<ParseMode.Realized>> where TMode : ParseMode
        {
            private readonly Future<IDeferredOutput<char>> future;

            private readonly Future<Slash<TMode>> slash;
            private readonly Future<AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode>> characters;

            private readonly Future<IOutput<char, Segment<ParseMode.Realized>>> cachedOutput;

            public Segment(Future<IDeferredOutput<char>> future)
            {
                this.future = future;

                this.slash = new Future<Slash<TMode>>(() => new Slash<TMode>(this.future));
                this.characters = new Future<AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode>>(() => new AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode>(
                        Func.Lift(this.Slash.Realize, DeferredOutput.Create),
                        input => new AlphaNumericHolder(input)));

                this.cachedOutput = new Future<IOutput<char, Segment<ParseMode.Realized>>>(this.RealizeImpl);
            }

            private Segment(
                Slash<TMode> slash, 
                AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode> characters,
                Future<IOutput<char, Segment<ParseMode.Realized>>> cachedOutput)
            {
                this.slash = new Future<Slash<TMode>>(() => slash);

                this.characters = new Future<AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode>>(
                    () => characters);

                this.cachedOutput = cachedOutput;
            }

            public Slash<TMode> Slash
            {
                get
                {
                    return this.slash;
                }
            }

            public AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode> Characters
            {
                get
                {
                    return this.characters;
                }
            }

            public IOutput<char, Segment<ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IOutput<char, Segment<ParseMode.Realized>> RealizeImpl()
            {
                var output = this.Characters.Realize();
                if (output.Success)
                {
                    return new Output<char, Segment<ParseMode.Realized>>(
                        true,
                        new Segment<ParseMode.Realized>(
                            this.Slash.Realize().Parsed,
                            this.Characters.Realize().Parsed,
                            this.cachedOutput), 
                        output.Remainder);
                }
                else
                {
                    return new Output<char, Segment<ParseMode.Realized>>(false, default, output.Remainder);
                }
            }
        }

        public sealed class EqualsSign<TMode> : IDeferredAstNode<char, EqualsSign<ParseMode.Realized>> where TMode : ParseMode
        {
            private readonly Future<IDeferredOutput<char>> future;

            private readonly Future<IOutput<char, EqualsSign<ParseMode.Realized>>> cachedOutput;

            public EqualsSign(Future<IDeferredOutput<char>> future)
            {
                this.future = future;

                this.cachedOutput = new Future<IOutput<char, EqualsSign<ParseMode.Realized>>>(this.RealizeImpl);
            }

            private EqualsSign(Future<IOutput<char, EqualsSign<ParseMode.Realized>>> cachedOutput)
            {
                this.cachedOutput = cachedOutput;
            }

            public IOutput<char, EqualsSign<ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IOutput<char, EqualsSign<ParseMode.Realized>> RealizeImpl()
            {
                var output = this.future.Value;
                if (!output.Success)
                {
                    return new Output<char, EqualsSign<ParseMode.Realized>>(false, default, output.Remainder);
                }

                var input = output.Remainder;
                if (input == null)
                {
                    return new Output<char, EqualsSign<ParseMode.Realized>>(false, default, input);
                }

                if (input.Current == '=')
                {
                    return new Output<char, EqualsSign<ParseMode.Realized>>(
                        true, 
                        new EqualsSign<ParseMode.Realized>(this.cachedOutput), 
                        input.Next());
                }
                else
                {
                    return new Output<char, EqualsSign<ParseMode.Realized>>(false, default, input);
                }
            }
        }

        public sealed class OptionName<TMode> : IDeferredAstNode<char, OptionName<ParseMode.Realized>> where TMode : ParseMode
        {
            private readonly Future<IDeferredOutput<char>> future;

            private readonly Future
                <
                    AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode>
                > 
                    characters;

            private readonly Future<IOutput<char, OptionName<ParseMode.Realized>>> cachedOutput;

            public OptionName(Future<IDeferredOutput<char>> future)
            {
                this.future = future;

                this.characters = 
                    new Future<AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode>>(
                        () => 
                            new AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode>(
                                future, 
                                input => new AlphaNumericHolder(input)));

                this.cachedOutput = new Future<IOutput<char, OptionName<ParseMode.Realized>>>(this.RealizeImpl);
            }

            private OptionName(
                AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode> characters,
                Future<IOutput<char, OptionName<ParseMode.Realized>>> cachedOutput)
            {
                this.characters = 
                    new Future<AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode>>(
                        () => characters);

                this.cachedOutput = cachedOutput;
            }

            public AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode> Characters
            {
                get
                {
                    return this.characters;
                }
            }

            public IOutput<char, OptionName<ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IOutput<char, OptionName<ParseMode.Realized>> RealizeImpl()
            {
                var output = this.Characters.Realize();
                if (output.Success)
                {
                    return new Output<char, OptionName<ParseMode.Realized>>(
                        true,
                        new OptionName<ParseMode.Realized>(this.Characters.Realize().Parsed, this.cachedOutput),
                        output.Remainder);
                }
                else
                {
                    return new Output<char, OptionName<ParseMode.Realized>>(false, default, output.Remainder);
                }
            }
        }

        public sealed class OptionValue<TMode> : IDeferredAstNode<char, OptionValue<ParseMode.Realized>> where TMode : ParseMode
        {
            private readonly Future<IDeferredOutput<char>> future;

            private readonly Future<Many<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode>> characters;

            private readonly Future<IOutput<char, OptionValue<ParseMode.Realized>>> cachedOutput;

            public OptionValue(Future<IDeferredOutput<char>> future)
            {
                this.future = future;

                this.characters = new Future<Many<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode>>(
                    () =>
                        new Many<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode>(
                            this.future,
                            input => new AlphaNumericHolder(input)));

                this.cachedOutput = new Future<IOutput<char, OptionValue<ParseMode.Realized>>>(this.RealizeImpl);
            }

            private OptionValue(
                Many<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode> characters,
                Future<IOutput<char, OptionValue<ParseMode.Realized>>> cachedOutput)
            {
                this.characters = new Future<Many<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode>>(
                    () => characters);

                this.cachedOutput = cachedOutput;
            }

            public Many<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode> Characters
            {
                get
                {
                    return this.characters;
                }
            }

            public IOutput<char, OptionValue<ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IOutput<char, OptionValue<ParseMode.Realized>> RealizeImpl()
            {
                var output = this.Characters.Realize();
                if (output.Success)
                {
                    return new Output<char, OptionValue<ParseMode.Realized>>(
                        true,
                        new OptionValue<ParseMode.Realized>(this.Characters.Realize().Parsed, this.cachedOutput),
                        output.Remainder);
                }
                else
                {
                    return new Output<char, OptionValue<ParseMode.Realized>>(false, default, output.Remainder);
                }

            }
        }

        public sealed class QueryOption<TMode> : IDeferredAstNode<char, QueryOption<ParseMode.Realized>> where TMode : ParseMode
        {
            private readonly Future<IDeferredOutput<char>> future;

            private readonly Future<OptionName<TMode>> name;
            private readonly Future<EqualsSign<TMode>> equalsSign;
            private readonly Future<OptionValue<TMode>> optionValue;

            private Future<IOutput<char, QueryOption<ParseMode.Realized>>> cachedOutput;

            public QueryOption(Future<IDeferredOutput<char>> future)
            {
                this.future = future;

                this.name = new Future<OptionName<TMode>>(() => new OptionName<TMode>(this.future));
                this.equalsSign = new Future<EqualsSign<TMode>>(() => new EqualsSign<TMode>(
                    Func.Lift(this.Name.Realize, DeferredOutput.Create)));
                this.optionValue = new Future<OptionValue<TMode>>(() => new OptionValue<TMode>(
                    DeferredOutput.ToPromise(this.EqualsSign.Realize)));

                this.cachedOutput = new Future<IOutput<char, QueryOption<ParseMode.Realized>>>(this.RealizeImpl);
            }

            private QueryOption(
                OptionName<TMode> name, 
                EqualsSign<TMode> equalsSign, 
                OptionValue<TMode> optionValue, 
                Future<IOutput<char, QueryOption<ParseMode.Realized>>> cachedOutput)
            {
                this.name = new Future<OptionName<TMode>>(() => name);
                this.equalsSign = new Future<EqualsSign<TMode>>(() => equalsSign);
                this.optionValue = new Future<OptionValue<TMode>>(() => optionValue);

                this.cachedOutput = cachedOutput;
            }

            public OptionName<TMode> Name
            {
                get
                {
                    return this.name;
                }
            }

            public EqualsSign<TMode> EqualsSign
            {
                get
                {
                    return this.equalsSign;
                }
            }

            public OptionValue<TMode> OptionValue
            {
                get
                {
                    return this.optionValue;
                }
            }

            public IOutput<char, QueryOption<ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IOutput<char, QueryOption<ParseMode.Realized>> RealizeImpl()
            {
                var output = this.OptionValue.Realize();
                if (output.Success)
                {
                    return new Output<char, QueryOption<ParseMode.Realized>>(
                        true,
                        new QueryOption<ParseMode.Realized>(
                            this.Name.Realize().Parsed, 
                            this.EqualsSign.Realize().Parsed, 
                            this.OptionValue.Realize().Parsed,
                        this.cachedOutput),
                        output.Remainder);
                }
                else
                {
                    return new Output<char, QueryOption<ParseMode.Realized>>(false, default, output.Remainder);
                }
            }
        }

        public sealed class QuestionMark<TMode> : IDeferredAstNode<char, QuestionMark<ParseMode.Realized>> where TMode : ParseMode
        {
            private readonly Future<IDeferredOutput<char>> future;

            private readonly Future<IOutput<char, QuestionMark<ParseMode.Realized>>> cachedOutput;

            public QuestionMark(Future<IDeferredOutput<char>> future)
            {
                this.future = future;

                this.cachedOutput = new Future<IOutput<char, QuestionMark<ParseMode.Realized>>>(this.RealizeImpl);
            }

            private QuestionMark(Future<IOutput<char, QuestionMark<ParseMode.Realized>>> cachedOutput)
            {
                this.cachedOutput = cachedOutput;
            }

            public IOutput<char, QuestionMark<ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IOutput<char, QuestionMark<ParseMode.Realized>> RealizeImpl()
            {
                var output = this.future.Value;
                if (!output.Success)
                {
                    return new Output<char, QuestionMark<ParseMode.Realized>>(false, default, output.Remainder);
                }

                var input = output.Remainder;

                if (input.Current == '?')
                {
                    return new Output<char, QuestionMark<ParseMode.Realized>>(
                        true, 
                        new QuestionMark<ParseMode.Realized>(this.cachedOutput), 
                        input.Next());
                }
                else
                {
                    return new Output<char, QuestionMark<ParseMode.Realized>>(false, default, input);
                }
            }
        }

        public sealed class OdataUri<TMode> : IDeferredAstNode<char, OdataUri<ParseMode.Realized>> where TMode : ParseMode
        {
            private readonly Future<IDeferredOutput<char>> future;

            private readonly Future<AtLeastOne<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>, TMode>> segments;
            private readonly Future<QuestionMark<TMode>> questionMark;
            private readonly Future<Many<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>, TMode>> queryOptions;

            private readonly Future<IOutput<char, OdataUri<ParseMode.Realized>>> cachedOutput;

            public OdataUri(Future<IDeferredOutput<char>> future)
            {
                this.future = future;

                this.segments = new Future<AtLeastOne<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>, TMode>>(
                    () => new AtLeastOne<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>, TMode>(
                        this.future,
                        input => new Segment<ParseMode.Deferred>(input)));
                this.questionMark = new Future<QuestionMark<TMode>>(
                    () => new QuestionMark<TMode>(DeferredOutput.ToPromise(this.Segments.Realize)));
                this.queryOptions = new Future<Many<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>, TMode>>(
                    () => new Many<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>, TMode>(
                        DeferredOutput.ToPromise(this.QuestionMark.Realize), 
                        input => new QueryOption<ParseMode.Deferred>(input)));

                this.cachedOutput = new Future<IOutput<char, OdataUri<ParseMode.Realized>>>(this.RealizeImpl);
            }

            private OdataUri(
                AtLeastOne<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>, TMode> segments,
                QuestionMark<TMode> questionMark,
                Many<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>, TMode> queryOptions,
                Future<IOutput<char, OdataUri<ParseMode.Realized>>> cachedOutput)
            {
                this.segments = new Future<AtLeastOne<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>, TMode>>(
                    () => segments);
                this.questionMark = new Future<QuestionMark<TMode>>(() => questionMark);
                this.queryOptions = new Future<Many<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>, TMode>>(
                    () => queryOptions);

                this.cachedOutput = cachedOutput;
            }

            public AtLeastOne<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>, TMode> Segments
            {
                get
                {
                    return this.segments;
                }
            }

            public QuestionMark<TMode> QuestionMark
            {
                get
                {
                    return this.questionMark;
                }
            }

            public Many<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>, TMode> QueryOptions
            {
                get
                {
                    return this.queryOptions;
                }
            }

            public IOutput<char, OdataUri<ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IOutput<char, OdataUri<ParseMode.Realized>> RealizeImpl()
            {
                var output = this.QueryOptions.Realize();
                if (output.Success)
                {
                    return new Output<char, OdataUri<ParseMode.Realized>>(
                        true,
                        new OdataUri<ParseMode.Realized>(
                            this.Segments.Realize().Parsed,
                            this.QuestionMark.Realize().Parsed,
                            this.QueryOptions.Realize().Parsed,
                            this.cachedOutput),
                        output.Remainder);
                }
                else
                {
                    return new Output<char, OdataUri<ParseMode.Realized>>(false, default, output.Remainder);
                }
            }
        }
    }
}

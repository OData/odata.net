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
    using System.IO;
    using Sprache;
    using System.Diagnostics.Contracts;

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

    public static class FutureExtensions
    {
        public static IFuture<TResult> Select<TValue, TResult>(this IFuture<TValue> future, Func<TValue, TResult> selector)
        {
            //// TODO i think this might be a "lift"
            return new Future<TResult>(() => selector(future.Value));
        }
    }

    public static class Func
    {
        public static Func<TOutput> Compose<TInput, TOutput>(Func<TInput> inner, Func<TInput, TOutput> outer)
        {
            return () => outer(inner());
        }

        public static Func<T> Close<T>(T value)
        {
            return () => value;
        }

        public static IFuture<T> ToFuture<T>(this Func<T> func)
        {
            return new Future<T>(func);
        }
    }

    /// <summary>
    /// NOTE: you considered having a class variant of this for cases where the caller needs to avoid boxing, but based on nullabletests.test4 there is basically no different in perforamnce
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct RealNullable<T>
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
        public static TRealizedAstNode Parse<TToken, TRealizedAstNode>(this IDeferredAstNode<TToken, TRealizedAstNode> deferredAstNode)
        {
            var output = deferredAstNode.Realize();
            if (!output.Success)
            {
                throw new InvalidDataException("TODO parse failed");
            }

            if (output.Remainder != null)
            {
                throw new InvalidOperationException("TODO parse succeeded but there were still tokens in the input stream");
            }

            return output.Parsed;
        }

        public static class Slash
        {
            public static Slash<ParseMode.Deferred> Create(IFuture<IDeferredOutput<char>> previouslyParsedOutput)
            {
                return Slash<ParseMode.Deferred>.Create(previouslyParsedOutput);
            }
        }

        public sealed class Slash<TMode> : IDeferredAstNode<char, Slash<ParseMode.Realized>>, IFromRealizedable<Slash<ParseMode.Deferred>> where TMode : ParseMode
        {
            private readonly IFuture<IDeferredOutput<char>> previouslyParsedOutput;

            private readonly Future<IOutput<char, Slash<ParseMode.Realized>>> cachedOutput;

            internal static Slash<ParseMode.Deferred> Create(IFuture<IDeferredOutput<char>> previouslyParsedOutput)
            {
                return new Slash<ParseMode.Deferred>(previouslyParsedOutput);
            }

            internal Slash(IFuture<IDeferredOutput<char>> previouslyParsedOutput) //// TODO make sure this parameter is named correctly everywhere
            {
                if (typeof(TMode) != typeof(ParseMode.Deferred))
                {
                    throw new ArgumentException("TODO");
                }

                this.previouslyParsedOutput = previouslyParsedOutput;

                this.cachedOutput = new Future<IOutput<char, Slash<ParseMode.Realized>>>(() => this.RealizeImpl());
            }

            internal Slash(Future<IOutput<char, Slash<ParseMode.Realized>>> output)
            {
                this.cachedOutput = output;
            }

            public Slash<ParseMode.Deferred> Convert()
            {
                if (typeof(TMode) == typeof(ParseMode.Deferred))
                {
                    return new Slash<ParseMode.Deferred>(this.previouslyParsedOutput);
                }
                else
                {
                    return new Slash<ParseMode.Deferred>(this.cachedOutput);
                }
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
            private readonly IFuture<IDeferredOutput<char>> future;

            private readonly IFuture<IOutput<char, AlphaNumeric<ParseMode.Realized>>> cachedOutput;

            public AlphaNumericHolder(IFuture<IDeferredOutput<char>> future)
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
                var a = AlphaNumeric.A.Create(this.future).Realize();
                if (a.Success)
                {
                    return a;
                }

                var c = AlphaNumeric.C.Create(this.future).Realize();
                if (c.Success)
                {
                    return c;
                }

                return new Output<char, AlphaNumeric<ParseMode.Realized>>(false, default, this.future.Value.Remainder);
            }
        }

        public static class AlphaNumeric
        {
            public static class A
            {
                public static AlphaNumeric<ParseMode.Deferred>.A Create(IFuture<IDeferredOutput<char>> previouslyParsedOutput)
                {
                    return AlphaNumeric<ParseMode.Deferred>.A.Create(previouslyParsedOutput);
                }
            }

            public static class C
            {
                public static AlphaNumeric<ParseMode.Deferred>.C Create(IFuture<IDeferredOutput<char>> previouslyParsedOutput)
                {
                    return AlphaNumeric<ParseMode.Deferred>.C.Create(previouslyParsedOutput);
                }
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

            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(AlphaNumeric<TMode> node, TContext context)
                {
                    return node.Dispatch(this, context);
                }

                protected internal abstract TResult Accept(A node, TContext context);
                protected internal abstract TResult Accept(C node, TContext context);
            }

            public sealed class A : AlphaNumeric<TMode>, IDeferredAstNode<char, AlphaNumeric<ParseMode.Realized>.A>
            {
                private readonly IFuture<IDeferredOutput<char>> previouslyParsedOutput;

                private readonly Future<IOutput<char, AlphaNumeric<ParseMode.Realized>.A>> cachedOutput;

                internal static AlphaNumeric<ParseMode.Deferred>.A Create(IFuture<IDeferredOutput<char>> previouslyParsedOutput)
                {
                    return new AlphaNumeric<ParseMode.Deferred>.A(previouslyParsedOutput);
                }

                internal A(IFuture<IDeferredOutput<char>> previouslyParsedOutput)
                {
                    if (typeof(TMode) != typeof(ParseMode.Deferred))
                    {
                        //// TODO add this back?
                        //// throw new ArgumentException("tODO i think this will depend on what you decide for modeling options of the deferred vs realized nodes");
                    }

                    this.previouslyParsedOutput = previouslyParsedOutput;

                    this.cachedOutput = new Future<IOutput<char, AlphaNumeric<ParseMode.Realized>.A>>(() => this.RealizeImpl());
                }

                internal A(Future<IOutput<char, AlphaNumeric<ParseMode.Realized>.A>> cachedOutput)
                {
                    this.cachedOutput = cachedOutput;
                }

                public new IOutput<char, AlphaNumeric<ParseMode.Realized>.A> Realize()
                {
                    return this.cachedOutput.Value;
                }

                private IOutput<char, AlphaNumeric<ParseMode.Realized>.A> RealizeImpl()
                {
                    var output = this.previouslyParsedOutput.Value;
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

                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }

                public override AlphaNumericHolder Convert()
                {
                    if (typeof(TMode) == typeof(ParseMode.Deferred))
                    {
                        return new AlphaNumericHolder(this.previouslyParsedOutput);
                    }
                    else
                    {
                        return new AlphaNumericHolder(this.cachedOutput);
                    }
                }
            }

            public sealed class C : AlphaNumeric<TMode>, IDeferredAstNode<char, AlphaNumeric<ParseMode.Realized>.C>
            {
                private readonly IFuture<IDeferredOutput<char>> previouslyParsedOutput;

                private readonly Future<IOutput<char, AlphaNumeric<ParseMode.Realized>.C>> cachedOutput;

                internal static AlphaNumeric<ParseMode.Deferred>.C Create(IFuture<IDeferredOutput<char>> previouslyParsedOutput)
                {
                    return new AlphaNumeric<ParseMode.Deferred>.C(previouslyParsedOutput);
                }

                internal C(IFuture<IDeferredOutput<char>> previouslyParsedOutput)
                {
                    if (typeof(TMode) != typeof(ParseMode.Deferred))
                    {
                        //// TODO add this back?
                        ////throw new ArgumentException("tODO i think this will depend on what you decide for modeling options of the deferred vs realized nodes");
                    }

                    this.previouslyParsedOutput = previouslyParsedOutput;

                    this.cachedOutput = new Future<IOutput<char, AlphaNumeric<ParseMode.Realized>.C>>(() => this.RealizeImpl());
                }

                internal C(Future<IOutput<char, AlphaNumeric<ParseMode.Realized>.C>> cachedOutput)
                {
                    this.cachedOutput = cachedOutput;
                }

                public new IOutput<char, AlphaNumeric<ParseMode.Realized>.C> Realize()
                {
                    return this.cachedOutput.Value;
                }

                private IOutput<char, AlphaNumeric<ParseMode.Realized>.C> RealizeImpl()
                {
                    var output = this.previouslyParsedOutput.Value;
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

                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }

                public override AlphaNumericHolder Convert()
                {
                    if (typeof(TMode) == typeof(ParseMode.Deferred))
                    {
                        return new AlphaNumericHolder(this.previouslyParsedOutput);
                    }
                    else
                    {
                        return new AlphaNumericHolder(this.cachedOutput);
                    }
                }
            }
        }

        public static class AtLeastOne
        {
            public static AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Create<TDeferredAstNode, TRealizedAstNode>(
                IFuture<IDeferredOutput<char>> previouslyParsedOutput,
                Func<IFuture<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory)
                where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode>
                where TRealizedAstNode : IFromRealizedable<TDeferredAstNode>
            {
                return AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>.Create(previouslyParsedOutput, nodeFactory);
            }
        }

        public sealed class AtLeastOne<TDeferredAstNode, TRealizedAstNode, TMode> : 
            IDeferredAstNode<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>,
            IFromRealizedable<AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>
            where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode>
            where TRealizedAstNode : IFromRealizedable<TDeferredAstNode>
            where TMode : ParseMode
        {
            private readonly IFuture<TDeferredAstNode> __1;
            private readonly IFuture<ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>> node;

            private readonly 
                Future<IOutput<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> 
                cachedOutput;

            internal static AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Create(
                IFuture<IDeferredOutput<char>> previouslyParsedOutput,
                Func<IFuture<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory)
            {
                var __1 = new Future<TDeferredAstNode>(
                    () => nodeFactory(previouslyParsedOutput));
                var node = new Future<ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>(
                    () => ManyNode.Create<TDeferredAstNode, TRealizedAstNode>(
                        nodeFactory,
                        Func.Compose(() => __1.Value.Realize(), DeferredOutput.Create).ToFuture()));

                return new AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(
                    __1,
                    node);
            }

            internal AtLeastOne(
                IFuture<TDeferredAstNode> __1,
                IFuture<ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>> node)
            {
                this.__1 = __1;
                this.node = node;

                this.cachedOutput = new Future
                    <
                        IOutput<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>
                    >(
                        () => this.RealizeImpl());
            }

            internal AtLeastOne( //// TODO this should be private
                TRealizedAstNode _1,
                ManyNode<TDeferredAstNode, TRealizedAstNode, TMode> node,
                Future<IOutput<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput)
            {
                this.__1 = new Future<TDeferredAstNode>(_1.Convert);
                this.node = new Future<ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>>(() => node);

                this.cachedOutput = cachedOutput;
            }

            public TDeferredAstNode _1 //// TODO does the type of this property actually make sense once realized?
            {
                get
                {
                    return this.__1.Value;
                }
            }

            public ManyNode<TDeferredAstNode, TRealizedAstNode, TMode> Node
            {
                get
                {
                    return this.node.Value;
                }
            }

            public AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Convert()
            {
                if (typeof(TMode) == typeof(ParseMode.Deferred))
                {
                    return new AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(
                        this.__1,
                        this.node.Select(_ => (_ as ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>)!)); //// TODO this is very hacky
                }
                else
                {
                    return new AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(
                        this._1.Realize().Parsed,
                        this.node.Value.Convert(),
                        this.cachedOutput);
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

        public static class Many
        {
            public static Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Create<TDeferredAstNode, TRealizedAstNode>(
                Func<IFuture<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory,
                IFuture<IDeferredOutput<char>> previouslyParsedOutput)
                where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode>
            {
                return Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>.Create(nodeFactory, previouslyParsedOutput);
            }
        }

        public sealed class Many<TDeferredAstNode, TRealizedAstNode, TMode> :
            IDeferredAstNode<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>,
            IFromRealizedable<Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>
            where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode>
            where TMode : ParseMode
        {
            private readonly IFuture<ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>> node;

            private readonly Future<IOutput<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput;

            internal static Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Create(
                Func<IFuture<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory,
                IFuture<IDeferredOutput<char>> previouslyParsedOutput)
            {
                var node = new Future<ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>(
                    () => ManyNode.Create<TDeferredAstNode, TRealizedAstNode>(nodeFactory, previouslyParsedOutput));

                return new Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(
                    node);
            }

            internal Many(
                IFuture<ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>> node)
            {
                this.node = node;

                this.cachedOutput = new Future<IOutput<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>>(
                    () => this.RealizeImpl());
            }

            internal Many(
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
                    return this.node.Value;
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

            public Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Convert()
            {
                if (typeof(TMode) == typeof(ParseMode.Deferred))
                {
                    return new Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(
                        this.node.Select(_ => _.Convert()));
                }
                else
                {
                    return new Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(
                        this.node.Value.Convert(),
                        this.cachedOutput);
                }
            }
        }

        public static class ManyNode
        {
            public static ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Create<TDeferredAstNode, TRealizedAstNode>(
                Func<IFuture<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory,
                IFuture<IDeferredOutput<char>> previouslyParsedOutput) where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode>
            {
                return ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>.Create(nodeFactory, previouslyParsedOutput);
            }
        }

        public sealed class ManyNode<TDeferredAstNode, TRealizedAstNode, TMode> : IDeferredAstNode<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>, IFromRealizedable<ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>> where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode> where TMode : ParseMode
        {
            internal readonly IFuture<OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode>> element; //// TODO this hsould be private
            private readonly IFuture<ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>> next;

            internal readonly Future<IOutput<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput; //// TODO this hsould be private

            internal static ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Create(
                Func<IFuture<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory,
                IFuture<IDeferredOutput<char>> previouslyParsedOutput)
            {
                var element = new Future<OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>(
                    () => OptionalNode.Create<TDeferredAstNode, TRealizedAstNode>(nodeFactory, previouslyParsedOutput));
                var next = new Future<ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>(
                    () =>
                        ManyNode.Create<TDeferredAstNode, TRealizedAstNode>(
                            nodeFactory,
                            Func.Compose(() => element.Value.Realize(), DeferredOutput.Create).ToFuture()));

                return new ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(element, next);
            }

            internal ManyNode(
                IFuture<OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode>> element,
                IFuture<ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>> next)
            {
                this.element = element;
                this.next = next;

                this.cachedOutput = new Future
                    <
                        IOutput<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>
                    >(
                        () => this.RealizeImpl());
            }

            internal static ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> GetTerminalRealizedNode(
                Future<IOutput<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput,
                OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> element)
            {
                //// TODO this method hsould be priovate
                return new ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                    element, 
                    () => GetTerminalRealizedNode(cachedOutput, element),
                    cachedOutput);
            }

            internal ManyNode(
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
                    return this.element.Value;
                }
            }

            /// <summary>
            /// TODO realize should only be called on this if <see cref="Element"/> has a value
            /// </summary>
            public ManyNode<TDeferredAstNode, TRealizedAstNode, TMode> Next
            {
                get
                {
                    return this.next.Value;
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

            public ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Convert()
            {
                if (typeof(TMode) == typeof(ParseMode.Deferred))
                {
                    return new ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(this.element.Select(_ => (_ as OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>)!),
                        this.next.Select(_ => (_ as ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>)!));
                }
                else
                {
                    return new ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(
                        this.element.Value.Convert(),
                        this.next.Value.Convert,
                        this.cachedOutput);
                }
            }
        }

        public static class OptionalNode
        {
            public static OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Create<TDeferredAstNode, TRealizedAstNode>(
                Func<IFuture<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory, 
                IFuture<IDeferredOutput<char>> previouslyParsedOutput)
                where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode>
            {
                return OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>.Create(nodeFactory, previouslyParsedOutput);
            }
        }

        public sealed class OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode> :
            IDeferredAstNode<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>,
            IFromRealizedable<OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>
            where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode>
            where TMode : ParseMode
        {
            private readonly IFuture<IDeferredOutput<char>> previouslyParsedOutput;
            private readonly Func<IFuture<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory;

            private readonly RealNullable<RealNullable<TRealizedAstNode>> value;

            private readonly Future<IOutput<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput;

            internal static OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Create(
                Func<IFuture<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory,
                IFuture<IDeferredOutput<char>> previouslyParsedOutput)
            {
                var value = new RealNullable<RealNullable<TRealizedAstNode>>();

                return new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(nodeFactory, previouslyParsedOutput, value);
            }

            internal OptionalNode(
                Func<IFuture<IDeferredOutput<char>>, TDeferredAstNode> nodeFactory,
                IFuture<IDeferredOutput<char>> previouslyParsedOutput,
                RealNullable<RealNullable<TRealizedAstNode>> value)
            {
                this.nodeFactory = nodeFactory;
                this.previouslyParsedOutput = previouslyParsedOutput;

                this.value = value;

                this.cachedOutput = new Future<IOutput<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>>(this.RealizeImpl);
            }

            internal OptionalNode(RealNullable<TRealizedAstNode> value, Future<IOutput<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput)
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

            public OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Convert()
            {
                if (typeof(TMode) == typeof(ParseMode.Deferred))
                {
                    return new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(
                        this.nodeFactory,
                        this.previouslyParsedOutput,
                        this.value);
                }
                else
                {
                    return new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(
                        this.Value,
                        this.cachedOutput);
                }
            }

            public IOutput<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IOutput<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> RealizeImpl()
            {
                var deferredOutput = this.previouslyParsedOutput.Value;
                if (!deferredOutput.Success)
                {
                    return new Output<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(false, default, deferredOutput.Remainder);
                }

                var value = this.nodeFactory(this.previouslyParsedOutput);
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

        public static class Segment
        {
            public static Segment<ParseMode.Deferred> Create(IFuture<IDeferredOutput<char>> previouslyParsedOutput)
            {
                return Segment<ParseMode.Deferred>.Create(previouslyParsedOutput);
            }
        }

        public sealed class Segment<TMode> : IDeferredAstNode<char, Segment<ParseMode.Realized>>, IFromRealizedable<Segment<ParseMode.Deferred>> where TMode : ParseMode
        {
            private readonly IFuture<Slash<TMode>> slash;
            private readonly IFuture<AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode>> characters;

            private readonly Future<IOutput<char, Segment<ParseMode.Realized>>> cachedOutput;

            internal static Segment<ParseMode.Deferred> Create(IFuture<IDeferredOutput<char>> previouslyParsedOutput)
            {
                var slash = new Future<Slash<ParseMode.Deferred>>(() => V3ParserPlayground.Slash.Create(previouslyParsedOutput));
                var characters = new Future<AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, ParseMode.Deferred>>(() => AtLeastOne.Create<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>>(
                        Func.Compose(() => slash.Value.Realize(), DeferredOutput.Create).ToFuture(), //// TODO the first parameter has a closure...
                        input => new AlphaNumericHolder(input)));
                return new Segment<ParseMode.Deferred>(slash, characters);
            }

            internal Segment(
                IFuture<Slash<TMode>> slash, 
                IFuture<AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode>> characters)
            {
                this.slash = slash;
                this.characters = characters;

                this.cachedOutput = new Future<IOutput<char, Segment<ParseMode.Realized>>>(this.RealizeImpl);
            }

            internal Segment(
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
                    return this.slash.Value;
                }
            }

            public AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode> Characters
            {
                get
                {
                    return this.characters.Value;
                }
            }

            public Segment<ParseMode.Deferred> Convert()
            {
                if (typeof(TMode) == typeof(ParseMode.Deferred))
                {
                    return new Segment<ParseMode.Deferred>(
                        this.slash.Select(_ => (_ as Slash<ParseMode.Deferred>)!), //// TODO this is hacky to get around the parsemode type parameter
                        this.characters.Select(_ => (_ as AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, ParseMode.Deferred>)!));
                }
                else
                {
                    return new Segment<ParseMode.Deferred>(this.slash.Value.Convert(), this.characters.Value.Convert(), this.cachedOutput);
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

        public static class EqualsSign
        {
            public static EqualsSign<ParseMode.Deferred> Create(IFuture<IDeferredOutput<char>> previouslyParsedOutput)
            {
                return EqualsSign<ParseMode.Deferred>.Create(previouslyParsedOutput);
            }
        }

        public sealed class EqualsSign<TMode> : IDeferredAstNode<char, EqualsSign<ParseMode.Realized>>, IFromRealizedable<EqualsSign<ParseMode.Deferred>> where TMode : ParseMode
        {
            private readonly IFuture<IDeferredOutput<char>> previouslyParsedOutput;

            private readonly Future<IOutput<char, EqualsSign<ParseMode.Realized>>> cachedOutput;

            internal static EqualsSign<ParseMode.Deferred> Create(IFuture<IDeferredOutput<char>> previouslyParsedOutput)
            {
                return new EqualsSign<ParseMode.Deferred>(previouslyParsedOutput);
            }

            internal EqualsSign(IFuture<IDeferredOutput<char>> previouslyParsedOutput)
            {
                this.previouslyParsedOutput = previouslyParsedOutput;

                this.cachedOutput = new Future<IOutput<char, EqualsSign<ParseMode.Realized>>>(this.RealizeImpl);
            }

            internal EqualsSign(Future<IOutput<char, EqualsSign<ParseMode.Realized>>> cachedOutput)
            {
                this.cachedOutput = cachedOutput;
            }

            public IOutput<char, EqualsSign<ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IOutput<char, EqualsSign<ParseMode.Realized>> RealizeImpl()
            {
                var output = this.previouslyParsedOutput.Value;
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

            public EqualsSign<ParseMode.Deferred> Convert()
            {
                if (typeof(TMode) == typeof(ParseMode.Deferred))
                {
                    return new EqualsSign<ParseMode.Deferred>(this.previouslyParsedOutput);
                }
                else
                {
                    return new EqualsSign<ParseMode.Deferred>(this.cachedOutput);
                }
            }
        }

        public static class OptionName
        {
            public static OptionName<ParseMode.Deferred> Create(IFuture<IDeferredOutput<char>> previouslyParsedOutput)
            {
                return OptionName<ParseMode.Deferred>.Create(previouslyParsedOutput);
            }
        }

        public sealed class OptionName<TMode> : IDeferredAstNode<char, OptionName<ParseMode.Realized>>, IFromRealizedable<OptionName<ParseMode.Deferred>>
            where TMode : ParseMode
        {
            private readonly IFuture
                <
                    AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode>
                > 
                    characters;

            private readonly Future<IOutput<char, OptionName<ParseMode.Realized>>> cachedOutput;

            internal static OptionName<ParseMode.Deferred> Create(IFuture<IDeferredOutput<char>> previouslyParsedOutput)
            {
                var characters = new Future<AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, ParseMode.Deferred>>(
                        () =>
                            AtLeastOne.Create<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>>(
                                previouslyParsedOutput,
                                input => new AlphaNumericHolder(input)));
                return new OptionName<ParseMode.Deferred>(characters);
            }

            internal OptionName(IFuture<AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode>> characters)
            {
                this.characters = characters;

                this.cachedOutput = new Future<IOutput<char, OptionName<ParseMode.Realized>>>(this.RealizeImpl);
            }

            internal OptionName(
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
                    return this.characters.Value;
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

            public OptionName<ParseMode.Deferred> Convert()
            {
                if (typeof(TMode) == typeof(ParseMode.Deferred))
                {
                    return new OptionName<ParseMode.Deferred>(
                        this.characters.Select(_ => _.Convert()));
                }
                else
                {
                    return new OptionName<ParseMode.Deferred>(
                        this.characters.Value.Convert(),
                        this.cachedOutput);
                }
            }
        }

        public static class OptionValue
        {
            public static OptionValue<ParseMode.Deferred> Create(IFuture<IDeferredOutput<char>> previouslyParsedOutput)
            {
                return OptionValue<ParseMode.Deferred>.Create(previouslyParsedOutput);
            }
        }

        public sealed class OptionValue<TMode> : IDeferredAstNode<char, OptionValue<ParseMode.Realized>>, IFromRealizedable<OptionValue<ParseMode.Deferred>> where TMode : ParseMode
        {
            private readonly IFuture<Many<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode>> characters;

            private readonly Future<IOutput<char, OptionValue<ParseMode.Realized>>> cachedOutput;

            internal static OptionValue<ParseMode.Deferred> Create(IFuture<IDeferredOutput<char>> previouslyParsedOutput)
            {
                var characters = new Future<Many<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, ParseMode.Deferred>>(
                    () =>
                        Many.Create<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>>(
                            input => new AlphaNumericHolder(input),
                            previouslyParsedOutput));

                return new OptionValue<ParseMode.Deferred>(characters);
            }

            internal OptionValue(
                IFuture<Many<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode>> characters)
            {
                this.characters = characters;

                this.cachedOutput = new Future<IOutput<char, OptionValue<ParseMode.Realized>>>(this.RealizeImpl);
            }

            internal OptionValue(
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
                    return this.characters.Value;
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

            public OptionValue<ParseMode.Deferred> Convert()
            {
                if (typeof(TMode) == typeof(ParseMode.Deferred))
                {
                    return new OptionValue<ParseMode.Deferred>(
                        this.characters.Select(_ => _.Convert()));
                }
                else
                {
                    return new OptionValue<ParseMode.Deferred>(
                        this.characters.Value.Convert(),
                        this.cachedOutput);
                }
            }
        }

        public static class QueryOption
        {
            public static QueryOption<ParseMode.Deferred> Create(IFuture<IDeferredOutput<char>> previouslyParsedOutput)
            {
                return QueryOption<ParseMode.Deferred>.Create(previouslyParsedOutput);
            }
        }

        public sealed class QueryOption<TMode> : IDeferredAstNode<char, QueryOption<ParseMode.Realized>>, IFromRealizedable<QueryOption<ParseMode.Deferred>> where TMode : ParseMode
        {
            private readonly IFuture<OptionName<TMode>> name;
            private readonly IFuture<EqualsSign<TMode>> equalsSign;
            private readonly IFuture<OptionValue<TMode>> optionValue;

            private Future<IOutput<char, QueryOption<ParseMode.Realized>>> cachedOutput;

            internal static QueryOption<ParseMode.Deferred> Create(IFuture<IDeferredOutput<char>> previouslyParsedOutput)
            {
                var name = new Future<OptionName<ParseMode.Deferred>>(() => OptionName.Create(previouslyParsedOutput));
                var equalsSign = new Future<EqualsSign<ParseMode.Deferred>>(() => V3ParserPlayground.EqualsSign.Create(
                    Func.Compose(() => name.Value.Realize(), DeferredOutput.Create).ToFuture()));
                var optionValue = new Future<OptionValue<ParseMode.Deferred>>(() => V3ParserPlayground.OptionValue.Create(
                    DeferredOutput.ToPromise(() => equalsSign.Value.Realize()).ToFuture()));

                return new QueryOption<ParseMode.Deferred>(name, equalsSign, optionValue);
            }

            internal QueryOption(IFuture<OptionName<TMode>> name, IFuture<EqualsSign<TMode>> equalsSign, IFuture<OptionValue<TMode>> optionValue)
            {
                this.name = name;
                this.equalsSign = equalsSign;
                this.optionValue = optionValue;

                this.cachedOutput = new Future<IOutput<char, QueryOption<ParseMode.Realized>>>(this.RealizeImpl);
            }

            internal QueryOption(
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
                    return this.name.Value;
                }
            }

            public EqualsSign<TMode> EqualsSign
            {
                get
                {
                    return this.equalsSign.Value;
                }
            }

            public OptionValue<TMode> OptionValue
            {
                get
                {
                    return this.optionValue.Value;
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

            public QueryOption<ParseMode.Deferred> Convert()
            {
                if (typeof(TMode) == typeof(ParseMode.Deferred))
                {
                    return new QueryOption<ParseMode.Deferred>(
                        this.name.Select(_ => _.Convert()),
                        this.equalsSign.Select(_ => _.Convert()),
                        this.optionValue.Select(_ => _.Convert()));
                }
                else
                {
                    return new QueryOption<ParseMode.Deferred>(
                        this.name.Value.Convert(),
                        this.equalsSign.Value.Convert(),
                        this.optionValue.Value.Convert(),
                        this.cachedOutput);
                }
            }
        }

        public static class QuestionMark
        {
            public static QuestionMark<ParseMode.Deferred> Create(IFuture<IDeferredOutput<char>> previouslyParsedOutput)
            {
                return QuestionMark<ParseMode.Deferred>.Create(previouslyParsedOutput);
            }
        }

        public sealed class QuestionMark<TMode> : IDeferredAstNode<char, QuestionMark<ParseMode.Realized>> where TMode : ParseMode
        {
            private readonly IFuture<IDeferredOutput<char>> previouslyParsedOutput;

            private readonly Future<IOutput<char, QuestionMark<ParseMode.Realized>>> cachedOutput;

            internal static QuestionMark<ParseMode.Deferred> Create(IFuture<IDeferredOutput<char>> previouslyParsedOutput)
            {
                return new QuestionMark<ParseMode.Deferred>(previouslyParsedOutput);
            }

            internal QuestionMark(IFuture<IDeferredOutput<char>> previouslyParsedOutput) //// TODO should this have a "realized" singleton instead?
            {
                this.previouslyParsedOutput = previouslyParsedOutput;

                this.cachedOutput = new Future<IOutput<char, QuestionMark<ParseMode.Realized>>>(this.RealizeImpl);
            }

            internal QuestionMark(Future<IOutput<char, QuestionMark<ParseMode.Realized>>> cachedOutput)
            {
                this.cachedOutput = cachedOutput;
            }

            public IOutput<char, QuestionMark<ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IOutput<char, QuestionMark<ParseMode.Realized>> RealizeImpl()
            {
                var output = this.previouslyParsedOutput.Value;
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

        public static class OdataUri
        {
            public static OdataUri<ParseMode.Deferred> Create(IFuture<IDeferredOutput<char>> previouslyParsedOutput)
            {
                return OdataUri<ParseMode.Deferred>.Create(previouslyParsedOutput);
            }
        }

        public sealed class OdataUri<TMode> : IDeferredAstNode<char, OdataUri<ParseMode.Realized>> where TMode : ParseMode
        {
            private readonly IFuture<AtLeastOne<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>, TMode>> segments;
            private readonly IFuture<QuestionMark<TMode>> questionMark;
            private readonly IFuture<Many<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>, TMode>> queryOptions;

            private readonly Future<IOutput<char, OdataUri<ParseMode.Realized>>> cachedOutput;

            internal static OdataUri<ParseMode.Deferred> Create(IFuture<IDeferredOutput<char>> previouslyParsedOutput)
            {
                var segments = new Future<AtLeastOne<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>, ParseMode.Deferred>>(
                    () => AtLeastOne.Create<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>>(
                        previouslyParsedOutput,
                        input => Segment.Create(input)));
                var questionMark = new Future<QuestionMark<ParseMode.Deferred>>(
                    () => V3ParserPlayground.QuestionMark.Create(DeferredOutput.ToPromise(() => segments.Value.Realize()).ToFuture()));
                var queryOptions = new Future<Many<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>, ParseMode.Deferred>>(
                    () => Many.Create<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>>(
                        input => QueryOption.Create(input),
                        DeferredOutput.ToPromise(() => questionMark.Value.Realize()).ToFuture()));

                return new OdataUri<ParseMode.Deferred>(segments, questionMark, queryOptions);
            }

            internal OdataUri( //// TODO i think there should be a factory method for these constructors instead of making them internal
                IFuture<AtLeastOne<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>, TMode>> segments, 
                IFuture<QuestionMark<TMode>> questionMark, 
                IFuture<Many<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>, TMode>> queryOptions)
            {
                this.segments = segments;
                this.questionMark = questionMark;
                this.queryOptions = queryOptions;

                this.cachedOutput = new Future<IOutput<char, OdataUri<ParseMode.Realized>>>(this.RealizeImpl);
            }

            internal OdataUri(
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
                    return this.segments.Value;
                }
            }

            public QuestionMark<TMode> QuestionMark
            {
                get
                {
                    return this.questionMark.Value;
                }
            }

            public Many<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>, TMode> QueryOptions
            {
                get
                {
                    return this.queryOptions.Value;
                }
            }

            public IOutput<char, OdataUri<ParseMode.Realized>> Realize()
            {
                //// TODO async optimizations
                
                //// TODO what about the case where the input is a `stream` and so we don't necessarily have the next byte to further parse?

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

    /*public interface IOdataServiceDeferred
    {
        OdataGetResponseAst Get(OdataUri<ParseMode.Deferred> uri);

        ////OdataGetResponseAst Get(Stream input, IEnumerable<ITokenHandler> handlers);

        OdataPatchResponseAst Patch(OdataUriAst uri, OdataPatchRequestAst request);

        // other verbs here
    }

    public interface IOdataService
    {
        OdataGetResponseAst Get(OdataUri<ParseMode.Realized> uri);

        //// TODO how can you reconcile this with clement's proposal?
        ////OdataGetResponseAst Get(Stream input, IEnumerable<ITokenHandler> handlers);

        OdataPatchResponseAst Patch(OdataUriAst uri, OdataPatchRequestAst request);
        
        // other verbs here
    }

    public sealed class Client : IOdataService
    {
    }

    public class AgsController
    {
        [Route("*")]
        public IAsyncResult Get(string uri)
        {
            new Ags().Get(ParseUriIntoAst(uri));
        }
    }

    public sealed class Directory : IOdataService
    {
    }

    public sealed class Ags : IOdataServiceDeferred
    {
        private readonly IEnumerable<IOdataService> workloads;

        public Ags(IEnumerable<IOdataService> workloads)
        {
            this.workloads = workloads;
        }

        public OdataGetResponseAst Get(OdataUri<ParseMode.Deferred> uri)
        {
            var segmentsOutput = uri.Segments.Realize();
            if (!segmentsOutput.Success)
            {
                throw new Exception("TODO uri is not valid");
            }

            var workload = FindWorkloadFromSegents(segmentsOutput.Parsed);

            workload.Get(uri);
        }

        public OdataPatchResponseAst Patch(OdataUriAst uri, OdataPatchRequestAst request)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class OdataUriAst
    {
    }

    public sealed class OdataPatchRequestAst
    {
    }

    public sealed class OdataGetResponseAst
    {
    }

    public sealed class OdataPatchResponseAst
    {
    }*/
}

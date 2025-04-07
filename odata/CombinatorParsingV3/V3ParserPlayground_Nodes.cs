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

    /// <summary>
    /// NOTE: you considered having a class variant of this for cases where the caller needs to avoid boxing, but based on nullabletests.test4 there is basically no different in perforamnce
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct Optional<T>
    {
        private readonly T value;

        private readonly bool hasValue;

        public Optional(T value)
        {
            this.value = value;
            this.hasValue = true;
        }

        public bool TryGetValue([System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out T value)
        {
            value = this.value;
            return this.hasValue;
        }

        public static implicit operator Optional<T>(T value)
        {
            return new Optional<T>(value);
        }
    }

    public interface IFuture<out T> //// TODO i'm not sure i like having an interface...
    {
        T Value { get; }
    }

    public sealed class Future<T> : IFuture<T>
    {
        private readonly Func<T> promise;

        private Optional<T> value;

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
    }

    public static class Future
    {
        public static Future<T> Create<T>(Func<T> promise)
        {
            return new Future<T>(promise);
        }
    }

    public static class FutureExtensions
    {
        public static IFuture<TResult> Select<TValue, TResult>(this IFuture<TValue> future, Func<TValue, TResult> selector)
        {
            return new Future<TResult>(() => selector(future.Value));
        }
    }

    public static partial class V3ParserPlayground
    {
        public static TRealizedAstNode Parse<TToken, TRealizedAstNode>(this IAstNode<TToken, TRealizedAstNode> deferredAstNode)
        {
            var output = deferredAstNode.Realize();
            if (!output.Success)
            {
                throw new InvalidDataException("TODO parse failed");
            }

            if (output.RemainingTokens != null)
            {
                throw new InvalidOperationException("TODO parse succeeded but there were still tokens in the input stream");
            }

            return output.RealizedValue;
        }

        public static class Slash
        {
            public static Slash<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previouslyParsedOutput)
            {
                return Slash<ParseMode.Deferred>.Create(previouslyParsedOutput);
            }
        }

        public sealed class Slash<TMode> : IAstNode<char, Slash<ParseMode.Realized>>, IFromRealizedable<Slash<ParseMode.Deferred>> where TMode : ParseMode
        {
            private readonly IFuture<IRealizationResult<char>> previouslyParsedOutput;

            private readonly Future<IRealizationResult<char, Slash<ParseMode.Realized>>> cachedOutput;

            internal static Slash<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previouslyParsedOutput)
            {
                return new Slash<ParseMode.Deferred>(previouslyParsedOutput);
            }

            private Slash(IFuture<IRealizationResult<char>> previouslyParsedOutput)
            {
                if (typeof(TMode) != typeof(ParseMode.Deferred))
                {
                    throw new ArgumentException("TODO");
                }

                this.previouslyParsedOutput = previouslyParsedOutput;

                this.cachedOutput = new Future<IRealizationResult<char, Slash<ParseMode.Realized>>>(() => this.RealizeImpl());
            }

            private Slash(Future<IRealizationResult<char, Slash<ParseMode.Realized>>> output)
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

            public IRealizationResult<char, Slash<ParseMode.Realized>> Realize()
            {
                return cachedOutput.Value;
            }

            private IRealizationResult<char, Slash<ParseMode.Realized>> RealizeImpl()
            {
                var output = this.previouslyParsedOutput.Value;
                if (!output.Success)
                {
                    return new RealizationResult<char, Slash<ParseMode.Realized>>(false, default, output.RemainingTokens);
                }

                var input = output.RemainingTokens;

                if (input.Current == '/')
                {
                    return new RealizationResult<char, Slash<ParseMode.Realized>>(
                        true,
                        new Slash<ParseMode.Realized>(this.cachedOutput),
                        input.Next());
                }
                else
                {
                    return new RealizationResult<char, Slash<ParseMode.Realized>>(false, default, input);
                }
            }
        }

        public sealed class AlphaNumericHolder : IAstNode<char, AlphaNumeric<ParseMode.Realized>>
        {
            private readonly IFuture<IRealizationResult<char>> previouslyParsedOutput;

            private readonly IFuture<IRealizationResult<char, AlphaNumeric<ParseMode.Realized>>> cachedOutput;

            public AlphaNumericHolder(IFuture<IRealizationResult<char>> previouslyParsedOutput)
            {
                this.previouslyParsedOutput = previouslyParsedOutput;

                this.cachedOutput = new Future<IRealizationResult<char, AlphaNumeric<ParseMode.Realized>>>(this.RealizeImpl);
            }

            public AlphaNumericHolder(IFuture<IRealizationResult<char, AlphaNumeric<ParseMode.Realized>>> cachedOutput)
            {
                this.cachedOutput = cachedOutput;
            }

            public IRealizationResult<char, AlphaNumeric<ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IRealizationResult<char, AlphaNumeric<ParseMode.Realized>> RealizeImpl()
            {
                var a = AlphaNumeric.A.Create(this.previouslyParsedOutput).Realize();
                if (a.Success)
                {
                    return a;
                }

                var c = AlphaNumeric.C.Create(this.previouslyParsedOutput).Realize();
                if (c.Success)
                {
                    return c;
                }

                return new RealizationResult<char, AlphaNumeric<ParseMode.Realized>>(false, default, this.previouslyParsedOutput.Value.RemainingTokens);
            }
        }

        public static class AlphaNumeric
        {
            public static class A
            {
                public static AlphaNumeric<ParseMode.Deferred>.A Create(IFuture<IRealizationResult<char>> previouslyParsedOutput)
                {
                    return AlphaNumeric<ParseMode.Deferred>.A.Create(previouslyParsedOutput);
                }
            }

            public static class C
            {
                public static AlphaNumeric<ParseMode.Deferred>.C Create(IFuture<IRealizationResult<char>> previouslyParsedOutput)
                {
                    return AlphaNumeric<ParseMode.Deferred>.C.Create(previouslyParsedOutput);
                }
            }
        }

        public abstract class AlphaNumeric<TMode> : IAstNode<char, AlphaNumeric<ParseMode.Realized>>, IFromRealizedable<AlphaNumericHolder>
            where TMode : ParseMode
        {
            private AlphaNumeric()
            {
            }

            public IRealizationResult<char, AlphaNumeric<ParseMode.Realized>> Realize()
            {
                return this.DerivedRealize();
            }

            protected abstract IRealizationResult<char, AlphaNumeric<ParseMode.Realized>> DerivedRealize();

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

            public sealed class A : AlphaNumeric<TMode>, IAstNode<char, AlphaNumeric<ParseMode.Realized>.A>
            {
                private readonly IFuture<IRealizationResult<char>> previouslyParsedOutput;

                private readonly Future<IRealizationResult<char, AlphaNumeric<ParseMode.Realized>.A>> cachedOutput;

                internal static AlphaNumeric<ParseMode.Deferred>.A Create(IFuture<IRealizationResult<char>> previouslyParsedOutput)
                {
                    return new AlphaNumeric<ParseMode.Deferred>.A(previouslyParsedOutput);
                }

                private A(IFuture<IRealizationResult<char>> previouslyParsedOutput)
                {
                    if (typeof(TMode) != typeof(ParseMode.Deferred))
                    {
                        throw new ArgumentException("tODO i think this will depend on what you decide for modeling options of the deferred vs realized nodes");
                    }

                    this.previouslyParsedOutput = previouslyParsedOutput;

                    this.cachedOutput = new Future<IRealizationResult<char, AlphaNumeric<ParseMode.Realized>.A>>(() => this.RealizeImpl());
                }

                private A(Future<IRealizationResult<char, AlphaNumeric<ParseMode.Realized>.A>> cachedOutput)
                {
                    this.cachedOutput = cachedOutput;
                }

                public new IRealizationResult<char, AlphaNumeric<ParseMode.Realized>.A> Realize()
                {
                    return this.cachedOutput.Value;
                }

                private IRealizationResult<char, AlphaNumeric<ParseMode.Realized>.A> RealizeImpl()
                {
                    var output = this.previouslyParsedOutput.Value;
                    if (!output.Success)
                    {
                        return new RealizationResult<char, AlphaNumeric<ParseMode.Realized>.A>(false, default, output.RemainingTokens);
                    }

                    var input = output.RemainingTokens;
                    if (input == null)
                    {
                        return new RealizationResult<char, AlphaNumeric<ParseMode.Realized>.A>(false, default, input);
                    }

                    if (input.Current == 'A')
                    {
                        return new RealizationResult<char, AlphaNumeric<ParseMode.Realized>.A>(true, new AlphaNumeric<ParseMode.Realized>.A(this.cachedOutput), input.Next());
                    }
                    else
                    {
                        return new RealizationResult<char, AlphaNumeric<ParseMode.Realized>.A>(false, default, input);
                    }
                }

                protected override IRealizationResult<char, AlphaNumeric<ParseMode.Realized>> DerivedRealize()
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

            public sealed class C : AlphaNumeric<TMode>, IAstNode<char, AlphaNumeric<ParseMode.Realized>.C>
            {
                private readonly IFuture<IRealizationResult<char>> previouslyParsedOutput;

                private readonly Future<IRealizationResult<char, AlphaNumeric<ParseMode.Realized>.C>> cachedOutput;

                internal static AlphaNumeric<ParseMode.Deferred>.C Create(IFuture<IRealizationResult<char>> previouslyParsedOutput)
                {
                    return new AlphaNumeric<ParseMode.Deferred>.C(previouslyParsedOutput);
                }

                private C(IFuture<IRealizationResult<char>> previouslyParsedOutput)
                {
                    if (typeof(TMode) != typeof(ParseMode.Deferred))
                    {
                        throw new ArgumentException("tODO i think this will depend on what you decide for modeling options of the deferred vs realized nodes");
                    }

                    this.previouslyParsedOutput = previouslyParsedOutput;

                    this.cachedOutput = new Future<IRealizationResult<char, AlphaNumeric<ParseMode.Realized>.C>>(() => this.RealizeImpl());
                }

                private C(Future<IRealizationResult<char, AlphaNumeric<ParseMode.Realized>.C>> cachedOutput)
                {
                    this.cachedOutput = cachedOutput;
                }

                public new IRealizationResult<char, AlphaNumeric<ParseMode.Realized>.C> Realize()
                {
                    return this.cachedOutput.Value;
                }

                private IRealizationResult<char, AlphaNumeric<ParseMode.Realized>.C> RealizeImpl()
                {
                    var output = this.previouslyParsedOutput.Value;
                    if (!output.Success)
                    {
                        return new RealizationResult<char, AlphaNumeric<ParseMode.Realized>.C>(false, default, output.RemainingTokens);
                    }

                    var input = output.RemainingTokens;
                    if (input == null)
                    {
                        return new RealizationResult<char, AlphaNumeric<ParseMode.Realized>.C>(false, default, input);
                    }

                    if (input.Current == 'C')
                    {
                        return new RealizationResult<char, AlphaNumeric<ParseMode.Realized>.C>(true, new AlphaNumeric<ParseMode.Realized>.C(this.cachedOutput), input.Next());
                    }
                    else
                    {
                        return new RealizationResult<char, AlphaNumeric<ParseMode.Realized>.C>(false, default, input);
                    }
                }

                protected override IRealizationResult<char, AlphaNumeric<ParseMode.Realized>> DerivedRealize()
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
                IFuture<IRealizationResult<char>> previouslyParsedOutput,
                Func<IFuture<IRealizationResult<char>>, TDeferredAstNode> nodeFactory)
                where TDeferredAstNode : IAstNode<char, TRealizedAstNode>
                where TRealizedAstNode : IFromRealizedable<TDeferredAstNode>
            {
                return AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>.Create(previouslyParsedOutput, nodeFactory);
            }
        }

        public sealed class AtLeastOne<TDeferredAstNode, TRealizedAstNode, TMode> : 
            IAstNode<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>,
            IFromRealizedable<AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>
            where TDeferredAstNode : IAstNode<char, TRealizedAstNode>
            where TRealizedAstNode : IFromRealizedable<TDeferredAstNode>
            where TMode : ParseMode
        {
            private readonly IFuture<TDeferredAstNode> __1;
            private readonly IFuture<ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>> node;

            private readonly 
                Future<IRealizationResult<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> 
                cachedOutput;

            internal static AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Create(
                IFuture<IRealizationResult<char>> previouslyParsedOutput,
                Func<IFuture<IRealizationResult<char>>, TDeferredAstNode> nodeFactory)
            {
                var __1 = new Future<TDeferredAstNode>(
                    () => nodeFactory(previouslyParsedOutput));
                var node = new Future<ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>(
                    () => ManyNode.Create<TDeferredAstNode, TRealizedAstNode>(
                        nodeFactory,
                        Future.Create(() => __1.Value.Realize())));

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
                        IRealizationResult<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>
                    >(
                        () => this.RealizeImpl());
            }

            private AtLeastOne(
                TRealizedAstNode _1,
                ManyNode<TDeferredAstNode, TRealizedAstNode, TMode> node,
                Future<IRealizationResult<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput)
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
                        this.node.Select(_ => _.Convert()));
                }
                else
                {
                    return new AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(
                        this._1.Realize().RealizedValue,
                        this.node.Value.Convert(),
                        this.cachedOutput);
                }
            }

            public IRealizationResult<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IRealizationResult<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> RealizeImpl()
            {
                var output = this.Node.Realize();
                if (output.Success)
                {
                    return new RealizationResult<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        true,
                        new AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                            this._1.Realize().RealizedValue,
                            output.RealizedValue,
                            this.cachedOutput),
                        output.RemainingTokens);
                }
                else
                {
                    return new RealizationResult<char, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        false, 
                        default, 
                        output.RemainingTokens);
                }
            }
        }

        public static class Many
        {
            public static Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Create<TDeferredAstNode, TRealizedAstNode>(
                Func<IFuture<IRealizationResult<char>>, TDeferredAstNode> nodeFactory,
                IFuture<IRealizationResult<char>> previouslyParsedOutput)
                where TDeferredAstNode : IAstNode<char, TRealizedAstNode>
            {
                return Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>.Create(nodeFactory, previouslyParsedOutput);
            }
        }

        public sealed class Many<TDeferredAstNode, TRealizedAstNode, TMode> :
            IAstNode<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>,
            IFromRealizedable<Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>
            where TDeferredAstNode : IAstNode<char, TRealizedAstNode>
            where TMode : ParseMode
        {
            private readonly IFuture<ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>> node;

            private readonly Future<IRealizationResult<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput;

            internal static Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Create(
                Func<IFuture<IRealizationResult<char>>, TDeferredAstNode> nodeFactory,
                IFuture<IRealizationResult<char>> previouslyParsedOutput)
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

                this.cachedOutput = new Future<IRealizationResult<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>>(
                    () => this.RealizeImpl());
            }

            private Many(
                ManyNode<TDeferredAstNode, TRealizedAstNode, TMode> node, 
                Future<IRealizationResult<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput)
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

            public IRealizationResult<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IRealizationResult<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> RealizeImpl()
            {
                var output = this.Node.Realize();
                if (output.Success)
                {
                    return new RealizationResult<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        true,
                        new Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                            output.RealizedValue,
                            this.cachedOutput),
                        output.RemainingTokens);
                }
                else
                {
                    // if the optional failed to parse, it means that its dependencies failed to parse
                    return new RealizationResult<char, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        false, 
                        default, 
                        output.RemainingTokens);
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
                Func<IFuture<IRealizationResult<char>>, TDeferredAstNode> nodeFactory,
                IFuture<IRealizationResult<char>> previouslyParsedOutput) where TDeferredAstNode : IAstNode<char, TRealizedAstNode>
            {
                return ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>.Create(nodeFactory, previouslyParsedOutput);
            }
        }

        public sealed class ManyNode<TDeferredAstNode, TRealizedAstNode, TMode> : IAstNode<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>, IFromRealizedable<ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>> where TDeferredAstNode : IAstNode<char, TRealizedAstNode> where TMode : ParseMode
        {
            private readonly IFuture<OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode>> element;
            private readonly IFuture<ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>> next;

            private readonly Future<IRealizationResult<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput;

            internal static ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Create(
                Func<IFuture<IRealizationResult<char>>, TDeferredAstNode> nodeFactory,
                IFuture<IRealizationResult<char>> previouslyParsedOutput)
            {
                var element = new Future<OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>(
                    () => OptionalNode.Create<TDeferredAstNode, TRealizedAstNode>(nodeFactory, previouslyParsedOutput));
                var next = new Future<ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>(
                    () =>
                        ManyNode.Create<TDeferredAstNode, TRealizedAstNode>(
                            nodeFactory,
                            Future.Create(() => element.Value.Realize())));

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
                        IRealizationResult<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>
                    >(
                        () => this.RealizeImpl());
            }

            private static ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> GetTerminalRealizedNode(
                Future<IRealizationResult<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput,
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
                Future<IRealizationResult<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput)
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

            public IRealizationResult<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IRealizationResult<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> RealizeImpl()
            {
                var realizedElement = this.Element.Realize();
                if (!realizedElement.Success)
                {
                    // this means that the nullable parsing *didn't succeed*, which only happens if its dependencies couldn't be parsed; this means that we also can't succeed in parsing
                    return new RealizationResult<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        false,
                        default, 
                        realizedElement.RemainingTokens);
                }

                if (realizedElement.RealizedValue.Value.TryGetValue(out var parsed))
                {
                    var realizedNext = this.Next.Realize();
                    // *this* instance is the "dependency" for `next`, so we can only have success cases
                    return new RealizationResult<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        true,
                        new ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                            realizedElement.RealizedValue,
                            () => realizedNext.RealizedValue,
                            this.cachedOutput),
                        realizedNext.RemainingTokens);
                }
                else
                {
                    return new RealizationResult<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        true,
                        new ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                            realizedElement.RealizedValue,
                            () => GetTerminalRealizedNode(this.cachedOutput, realizedElement.RealizedValue),
                            this.cachedOutput),
                        realizedElement.RemainingTokens);
                }
            }

            public ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Convert()
            {
                if (typeof(TMode) == typeof(ParseMode.Deferred))
                {
                    return new ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(
                        this.element.Select(_ => _.Convert()),
                        this.next.Select(_ => _.Convert()));
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
                Func<IFuture<IRealizationResult<char>>, TDeferredAstNode> nodeFactory, 
                IFuture<IRealizationResult<char>> previouslyParsedOutput)
                where TDeferredAstNode : IAstNode<char, TRealizedAstNode>
            {
                return OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>.Create(nodeFactory, previouslyParsedOutput);
            }
        }

        public sealed class OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode> :
            IAstNode<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>,
            IFromRealizedable<OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>
            where TDeferredAstNode : IAstNode<char, TRealizedAstNode>
            where TMode : ParseMode
        {
            private readonly IFuture<IRealizationResult<char>> previouslyParsedOutput;
            private readonly Func<IFuture<IRealizationResult<char>>, TDeferredAstNode> nodeFactory;

            private readonly Optional<Optional<TRealizedAstNode>> value;

            private readonly Future<IRealizationResult<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput;

            internal static OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Create(
                Func<IFuture<IRealizationResult<char>>, TDeferredAstNode> nodeFactory,
                IFuture<IRealizationResult<char>> previouslyParsedOutput)
            {
                var value = new Optional<Optional<TRealizedAstNode>>();

                return new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(nodeFactory, previouslyParsedOutput, value);
            }

            internal OptionalNode(
                Func<IFuture<IRealizationResult<char>>, TDeferredAstNode> nodeFactory,
                IFuture<IRealizationResult<char>> previouslyParsedOutput,
                Optional<Optional<TRealizedAstNode>> value)
            {
                this.nodeFactory = nodeFactory;
                this.previouslyParsedOutput = previouslyParsedOutput;

                this.value = value;

                this.cachedOutput = new Future<IRealizationResult<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>>(this.RealizeImpl);
            }

            internal OptionalNode(Optional<TRealizedAstNode> value, Future<IRealizationResult<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput)
            {
                ///// TODO why is this constructor being used in the rewriters?
                this.value = new Optional<Optional<TRealizedAstNode>>(value);
                this.cachedOutput = cachedOutput;
            }

            public Optional<TRealizedAstNode> Value
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

            public IRealizationResult<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IRealizationResult<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>> RealizeImpl()
            {
                var deferredOutput = this.previouslyParsedOutput.Value;
                if (!deferredOutput.Success)
                {
                    return new RealizationResult<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(false, default, deferredOutput.RemainingTokens);
                }

                var value = this.nodeFactory(this.previouslyParsedOutput);
                var output = value.Realize();
                if (output.Success)
                {
                    return new RealizationResult<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        true,
                        new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                            new Optional<TRealizedAstNode>(output.RealizedValue),
                            this.cachedOutput),
                        output.RemainingTokens);
                }
                else
                {
                    return new RealizationResult<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(
                        true,
                        new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                            new Optional<TRealizedAstNode>(),
                            this.cachedOutput),
                        output.RemainingTokens);
                }
            }
        }

        public static class Segment
        {
            public static Segment<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previouslyParsedOutput)
            {
                return Segment<ParseMode.Deferred>.Create(previouslyParsedOutput);
            }
        }

        public sealed class Segment<TMode> : IAstNode<char, Segment<ParseMode.Realized>>, IFromRealizedable<Segment<ParseMode.Deferred>> where TMode : ParseMode
        {
            private readonly IFuture<Slash<TMode>> slash;
            private readonly IFuture<AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode>> characters;

            private readonly Future<IRealizationResult<char, Segment<ParseMode.Realized>>> cachedOutput;

            internal static Segment<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previouslyParsedOutput)
            {
                var slash = new Future<Slash<ParseMode.Deferred>>(() => V3ParserPlayground.Slash.Create(previouslyParsedOutput));
                var characters = new Future<AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, ParseMode.Deferred>>(() => AtLeastOne.Create<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>>(
                        Future.Create(() => slash.Value.Realize()), //// TODO the first parameter has a closure...
                        input => new AlphaNumericHolder(input)));
                return new Segment<ParseMode.Deferred>(slash, characters);
            }

            internal Segment(
                IFuture<Slash<TMode>> slash, 
                IFuture<AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode>> characters)
            {
                this.slash = slash;
                this.characters = characters;

                this.cachedOutput = new Future<IRealizationResult<char, Segment<ParseMode.Realized>>>(this.RealizeImpl);
            }

            private Segment(
                Slash<TMode> slash, 
                AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode> characters,
                Future<IRealizationResult<char, Segment<ParseMode.Realized>>> cachedOutput)
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
                        this.slash.Select(_ => _.Convert()),
                        this.characters.Select(_ => _.Convert()));
                }
                else
                {
                    return new Segment<ParseMode.Deferred>(this.slash.Value.Convert(), this.characters.Value.Convert(), this.cachedOutput);
                }
            }

            public IRealizationResult<char, Segment<ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IRealizationResult<char, Segment<ParseMode.Realized>> RealizeImpl()
            {
                var output = this.Characters.Realize();
                if (output.Success)
                {
                    return new RealizationResult<char, Segment<ParseMode.Realized>>(
                        true,
                        new Segment<ParseMode.Realized>(
                            this.Slash.Realize().RealizedValue,
                            this.Characters.Realize().RealizedValue,
                            this.cachedOutput), 
                        output.RemainingTokens);
                }
                else
                {
                    return new RealizationResult<char, Segment<ParseMode.Realized>>(false, default, output.RemainingTokens);
                }
            }
        }

        public static class EqualsSign
        {
            public static EqualsSign<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previouslyParsedOutput)
            {
                return EqualsSign<ParseMode.Deferred>.Create(previouslyParsedOutput);
            }
        }

        public sealed class EqualsSign<TMode> : IAstNode<char, EqualsSign<ParseMode.Realized>>, IFromRealizedable<EqualsSign<ParseMode.Deferred>> where TMode : ParseMode
        {
            private readonly IFuture<IRealizationResult<char>> previouslyParsedOutput;

            private readonly Future<IRealizationResult<char, EqualsSign<ParseMode.Realized>>> cachedOutput;

            internal static EqualsSign<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previouslyParsedOutput)
            {
                return new EqualsSign<ParseMode.Deferred>(previouslyParsedOutput);
            }

            private EqualsSign(IFuture<IRealizationResult<char>> previouslyParsedOutput)
            {
                this.previouslyParsedOutput = previouslyParsedOutput;

                this.cachedOutput = new Future<IRealizationResult<char, EqualsSign<ParseMode.Realized>>>(this.RealizeImpl);
            }

            private EqualsSign(Future<IRealizationResult<char, EqualsSign<ParseMode.Realized>>> cachedOutput)
            {
                this.cachedOutput = cachedOutput;
            }

            public IRealizationResult<char, EqualsSign<ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IRealizationResult<char, EqualsSign<ParseMode.Realized>> RealizeImpl()
            {
                var output = this.previouslyParsedOutput.Value;
                if (!output.Success)
                {
                    return new RealizationResult<char, EqualsSign<ParseMode.Realized>>(false, default, output.RemainingTokens);
                }

                var input = output.RemainingTokens;
                if (input == null)
                {
                    return new RealizationResult<char, EqualsSign<ParseMode.Realized>>(false, default, input);
                }

                if (input.Current == '=')
                {
                    return new RealizationResult<char, EqualsSign<ParseMode.Realized>>(
                        true, 
                        new EqualsSign<ParseMode.Realized>(this.cachedOutput), 
                        input.Next());
                }
                else
                {
                    return new RealizationResult<char, EqualsSign<ParseMode.Realized>>(false, default, input);
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
            public static OptionName<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previouslyParsedOutput)
            {
                return OptionName<ParseMode.Deferred>.Create(previouslyParsedOutput);
            }
        }

        public sealed class OptionName<TMode> : IAstNode<char, OptionName<ParseMode.Realized>>, IFromRealizedable<OptionName<ParseMode.Deferred>>
            where TMode : ParseMode
        {
            private readonly IFuture
                <
                    AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode>
                > 
                    characters;

            private readonly Future<IRealizationResult<char, OptionName<ParseMode.Realized>>> cachedOutput;

            internal static OptionName<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previouslyParsedOutput)
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

                this.cachedOutput = new Future<IRealizationResult<char, OptionName<ParseMode.Realized>>>(this.RealizeImpl);
            }

            private OptionName(
                AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode> characters,
                Future<IRealizationResult<char, OptionName<ParseMode.Realized>>> cachedOutput)
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

            public IRealizationResult<char, OptionName<ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IRealizationResult<char, OptionName<ParseMode.Realized>> RealizeImpl()
            {
                var output = this.Characters.Realize();
                if (output.Success)
                {
                    return new RealizationResult<char, OptionName<ParseMode.Realized>>(
                        true,
                        new OptionName<ParseMode.Realized>(this.Characters.Realize().RealizedValue, this.cachedOutput),
                        output.RemainingTokens);
                }
                else
                {
                    return new RealizationResult<char, OptionName<ParseMode.Realized>>(false, default, output.RemainingTokens);
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
            public static OptionValue<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previouslyParsedOutput)
            {
                return OptionValue<ParseMode.Deferred>.Create(previouslyParsedOutput);
            }
        }

        public sealed class OptionValue<TMode> : IAstNode<char, OptionValue<ParseMode.Realized>>, IFromRealizedable<OptionValue<ParseMode.Deferred>> where TMode : ParseMode
        {
            private readonly IFuture<Many<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode>> characters;

            private readonly Future<IRealizationResult<char, OptionValue<ParseMode.Realized>>> cachedOutput;

            internal static OptionValue<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previouslyParsedOutput)
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

                this.cachedOutput = new Future<IRealizationResult<char, OptionValue<ParseMode.Realized>>>(this.RealizeImpl);
            }

            private OptionValue(
                Many<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, TMode> characters,
                Future<IRealizationResult<char, OptionValue<ParseMode.Realized>>> cachedOutput)
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

            public IRealizationResult<char, OptionValue<ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IRealizationResult<char, OptionValue<ParseMode.Realized>> RealizeImpl()
            {
                var output = this.Characters.Realize();
                if (output.Success)
                {
                    return new RealizationResult<char, OptionValue<ParseMode.Realized>>(
                        true,
                        new OptionValue<ParseMode.Realized>(this.Characters.Realize().RealizedValue, this.cachedOutput),
                        output.RemainingTokens);
                }
                else
                {
                    return new RealizationResult<char, OptionValue<ParseMode.Realized>>(false, default, output.RemainingTokens);
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
            public static QueryOption<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previouslyParsedOutput)
            {
                return QueryOption<ParseMode.Deferred>.Create(previouslyParsedOutput);
            }
        }

        public sealed class QueryOption<TMode> : IAstNode<char, QueryOption<ParseMode.Realized>>, IFromRealizedable<QueryOption<ParseMode.Deferred>> where TMode : ParseMode
        {
            private readonly IFuture<OptionName<TMode>> name;
            private readonly IFuture<EqualsSign<TMode>> equalsSign;
            private readonly IFuture<OptionValue<TMode>> optionValue;

            private Future<IRealizationResult<char, QueryOption<ParseMode.Realized>>> cachedOutput;

            internal static QueryOption<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previouslyParsedOutput)
            {
                var name = new Future<OptionName<ParseMode.Deferred>>(() => OptionName.Create(previouslyParsedOutput));
                var equalsSign = new Future<EqualsSign<ParseMode.Deferred>>(() => V3ParserPlayground.EqualsSign.Create(
                    Future.Create(() => name.Value.Realize())));
                var optionValue = new Future<OptionValue<ParseMode.Deferred>>(() => V3ParserPlayground.OptionValue.Create(
                    Future.Create(() => equalsSign.Value.Realize())));

                return new QueryOption<ParseMode.Deferred>(name, equalsSign, optionValue);
            }

            internal QueryOption(IFuture<OptionName<TMode>> name, IFuture<EqualsSign<TMode>> equalsSign, IFuture<OptionValue<TMode>> optionValue)
            {
                this.name = name;
                this.equalsSign = equalsSign;
                this.optionValue = optionValue;

                this.cachedOutput = new Future<IRealizationResult<char, QueryOption<ParseMode.Realized>>>(this.RealizeImpl);
            }

            private QueryOption(
                OptionName<TMode> name, 
                EqualsSign<TMode> equalsSign, 
                OptionValue<TMode> optionValue, 
                Future<IRealizationResult<char, QueryOption<ParseMode.Realized>>> cachedOutput)
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

            public IRealizationResult<char, QueryOption<ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IRealizationResult<char, QueryOption<ParseMode.Realized>> RealizeImpl()
            {
                var output = this.OptionValue.Realize();
                if (output.Success)
                {
                    return new RealizationResult<char, QueryOption<ParseMode.Realized>>(
                        true,
                        new QueryOption<ParseMode.Realized>(
                            this.Name.Realize().RealizedValue, 
                            this.EqualsSign.Realize().RealizedValue, 
                            this.OptionValue.Realize().RealizedValue,
                        this.cachedOutput),
                        output.RemainingTokens);
                }
                else
                {
                    return new RealizationResult<char, QueryOption<ParseMode.Realized>>(false, default, output.RemainingTokens);
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
            public static QuestionMark<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previouslyParsedOutput)
            {
                return QuestionMark<ParseMode.Deferred>.Create(previouslyParsedOutput);
            }
        }

        public sealed class QuestionMark<TMode> : IAstNode<char, QuestionMark<ParseMode.Realized>>, IFromRealizedable<QuestionMark<ParseMode.Deferred>> where TMode : ParseMode
        {
            private readonly IFuture<IRealizationResult<char>> previouslyParsedOutput;

            private readonly Future<IRealizationResult<char, QuestionMark<ParseMode.Realized>>> cachedOutput;

            internal static QuestionMark<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previouslyParsedOutput)
            {
                return new QuestionMark<ParseMode.Deferred>(previouslyParsedOutput);
            }

            private QuestionMark(IFuture<IRealizationResult<char>> previouslyParsedOutput)
            {
                this.previouslyParsedOutput = previouslyParsedOutput;

                this.cachedOutput = new Future<IRealizationResult<char, QuestionMark<ParseMode.Realized>>>(this.RealizeImpl);
            }

            private QuestionMark(Future<IRealizationResult<char, QuestionMark<ParseMode.Realized>>> cachedOutput)
            {
                this.cachedOutput = cachedOutput;
            }

            public IRealizationResult<char, QuestionMark<ParseMode.Realized>> Realize()
            {
                return this.cachedOutput.Value;
            }

            private IRealizationResult<char, QuestionMark<ParseMode.Realized>> RealizeImpl()
            {
                var output = this.previouslyParsedOutput.Value;
                if (!output.Success)
                {
                    return new RealizationResult<char, QuestionMark<ParseMode.Realized>>(false, default, output.RemainingTokens);
                }

                var input = output.RemainingTokens;

                if (input.Current == '?')
                {
                    return new RealizationResult<char, QuestionMark<ParseMode.Realized>>(
                        true, 
                        new QuestionMark<ParseMode.Realized>(this.cachedOutput), 
                        input.Next());
                }
                else
                {
                    return new RealizationResult<char, QuestionMark<ParseMode.Realized>>(false, default, input);
                }
            }

            public QuestionMark<ParseMode.Deferred> Convert()
            {
                if (typeof(TMode) == typeof(ParseMode.Deferred))
                {
                    return new QuestionMark<ParseMode.Deferred>(
                        this.previouslyParsedOutput);
                }
                else
                {
                    return new QuestionMark<ParseMode.Deferred>(this.cachedOutput);
                }
            }
        }

        public static class OdataUri
        {
            public static OdataUri<ParseMode.Deferred> Create(ITokenStream<char> input)
            {
                return OdataUri.Create(Future.Create(() => new RealizationResult<char>(true, input)));
            }

            public static OdataUri<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previouslyParsedOutput)
            {
                return OdataUri<ParseMode.Deferred>.Create(previouslyParsedOutput);
            }
        }

        public sealed class OdataUri<TMode> : IAstNode<char, OdataUri<ParseMode.Realized>> where TMode : ParseMode
        {
            private readonly IFuture<AtLeastOne<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>, TMode>> segments;
            private readonly IFuture<QuestionMark<TMode>> questionMark;
            private readonly IFuture<Many<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>, TMode>> queryOptions;

            private readonly Future<IRealizationResult<char, OdataUri<ParseMode.Realized>>> cachedOutput;

            internal static OdataUri<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previouslyParsedOutput)
            {
                var segments = new Future<AtLeastOne<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>, ParseMode.Deferred>>(
                    () => AtLeastOne.Create<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>>(
                        previouslyParsedOutput,
                        input => Segment.Create(input)));
                var questionMark = new Future<QuestionMark<ParseMode.Deferred>>(
                    () => V3ParserPlayground.QuestionMark.Create(
                        Future.Create(() => segments.Value.Realize())));
                var queryOptions = new Future<Many<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>, ParseMode.Deferred>>(
                    () => Many.Create<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>>(
                        input => QueryOption.Create(input),
                        Future.Create(() => questionMark.Value.Realize())));

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

                this.cachedOutput = new Future<IRealizationResult<char, OdataUri<ParseMode.Realized>>>(this.RealizeImpl);
            }

            private OdataUri(
                AtLeastOne<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>, TMode> segments,
                QuestionMark<TMode> questionMark,
                Many<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>, TMode> queryOptions,
                Future<IRealizationResult<char, OdataUri<ParseMode.Realized>>> cachedOutput)
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

            public IRealizationResult<char, OdataUri<ParseMode.Realized>> Realize()
            {
                //// TODO async optimizations
                
                //// TODO what about the case where the input is a `stream` and so we don't necessarily have the next byte to further parse?

                return this.cachedOutput.Value;
            }

            private IRealizationResult<char, OdataUri<ParseMode.Realized>> RealizeImpl()
            {
                var output = this.QueryOptions.Realize();
                if (output.Success)
                {
                    return new RealizationResult<char, OdataUri<ParseMode.Realized>>(
                        true,
                        new OdataUri<ParseMode.Realized>(
                            this.Segments.Realize().RealizedValue,
                            this.QuestionMark.Realize().RealizedValue,
                            this.QueryOptions.Realize().RealizedValue,
                            this.cachedOutput),
                        output.RemainingTokens);
                }
                else
                {
                    return new RealizationResult<char, OdataUri<ParseMode.Realized>>(false, default, output.RemainingTokens);
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

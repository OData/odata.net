namespace CombinatorParsingV3
{
    using System;

    public static class AtLeastOne
    {
        public static AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Create<TDeferredAstNode, TRealizedAstNode>(
            IFuture<IRealizationResult<char>> previousNodeRealizationResult,
            Func<IFuture<IRealizationResult<char>>, TDeferredAstNode> nodeFactory)
            where TDeferredAstNode : IAstNode<char, TRealizedAstNode>
            where TRealizedAstNode : IFromRealizedable<TDeferredAstNode>
        {
            return AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>.Create(previousNodeRealizationResult, nodeFactory);
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
            IFuture<IRealizationResult<char>> previousNodeRealizationResult,
            Func<IFuture<IRealizationResult<char>>, TDeferredAstNode> nodeFactory)
        {
            var __1 = new Future<TDeferredAstNode>(
                () => nodeFactory(previousNodeRealizationResult));
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
            IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            where TDeferredAstNode : IAstNode<char, TRealizedAstNode>
        {
            return Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>.Create(nodeFactory, previousNodeRealizationResult);
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
            IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var node = new Future<ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>(
                () => ManyNode.Create<TDeferredAstNode, TRealizedAstNode>(nodeFactory, previousNodeRealizationResult));

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
            IFuture<IRealizationResult<char>> previousNodeRealizationResult) where TDeferredAstNode : IAstNode<char, TRealizedAstNode>
        {
            return ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>.Create(nodeFactory, previousNodeRealizationResult);
        }
    }

    public sealed class ManyNode<TDeferredAstNode, TRealizedAstNode, TMode> : IAstNode<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>, IFromRealizedable<ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>> where TDeferredAstNode : IAstNode<char, TRealizedAstNode> where TMode : ParseMode
    {
        private readonly IFuture<OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode>> element;
        private readonly IFuture<ManyNode<TDeferredAstNode, TRealizedAstNode, TMode>> next;

        private readonly Future<IRealizationResult<char, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput;

        internal static ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Create(
            Func<IFuture<IRealizationResult<char>>, TDeferredAstNode> nodeFactory,
            IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var element = new Future<OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>(
                () => OptionalNode.Create<TDeferredAstNode, TRealizedAstNode>(nodeFactory, previousNodeRealizationResult));
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
            IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            where TDeferredAstNode : IAstNode<char, TRealizedAstNode>
        {
            return OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>.Create(nodeFactory, previousNodeRealizationResult);
        }
    }

    public sealed class OptionalNode<TDeferredAstNode, TRealizedAstNode, TMode> :
        IAstNode<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>,
        IFromRealizedable<OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>
        where TDeferredAstNode : IAstNode<char, TRealizedAstNode>
        where TMode : ParseMode
    {
        private readonly IFuture<IRealizationResult<char>> previousNodeRealizationResult;
        private readonly Func<IFuture<IRealizationResult<char>>, TDeferredAstNode> nodeFactory;

        private readonly Optional<Optional<TRealizedAstNode>> value;

        private readonly Future<IRealizationResult<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>> cachedOutput;

        internal static OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Create(
            Func<IFuture<IRealizationResult<char>>, TDeferredAstNode> nodeFactory,
            IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var value = new Optional<Optional<TRealizedAstNode>>();

            return new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(nodeFactory, previousNodeRealizationResult, value);
        }

        internal OptionalNode(
            Func<IFuture<IRealizationResult<char>>, TDeferredAstNode> nodeFactory,
            IFuture<IRealizationResult<char>> previousNodeRealizationResult,
            Optional<Optional<TRealizedAstNode>> value)
        {
            this.nodeFactory = nodeFactory;
            this.previousNodeRealizationResult = previousNodeRealizationResult;

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
                    this.previousNodeRealizationResult,
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
            var deferredOutput = this.previousNodeRealizationResult.Value;
            if (!deferredOutput.Success)
            {
                return new RealizationResult<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(false, default, deferredOutput.RemainingTokens);
            }

            var value = this.nodeFactory(this.previousNodeRealizationResult);
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
}

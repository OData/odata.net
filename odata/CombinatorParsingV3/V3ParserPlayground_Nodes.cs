namespace CombinatorParsingV3
{
    using System;

    public static partial class V3ParserPlayground
    {
        public static class Slash
        {
            public static Slash<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return Slash<ParseMode.Deferred>.Create(previousNodeRealizationResult);
            }
        }

        public sealed class Slash<TMode> : IAstNode<char, Slash<ParseMode.Realized>>, IFromRealizedable<Slash<ParseMode.Deferred>> where TMode : ParseMode
        {
            private readonly IFuture<IRealizationResult<char>> previousNodeRealizationResult;

            private readonly Future<IRealizationResult<char, Slash<ParseMode.Realized>>> cachedOutput;

            internal static Slash<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new Slash<ParseMode.Deferred>(previousNodeRealizationResult);
            }

            private Slash(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                if (typeof(TMode) != typeof(ParseMode.Deferred))
                {
                    throw new ArgumentException("TODO");
                }

                this.previousNodeRealizationResult = previousNodeRealizationResult;

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
                    return new Slash<ParseMode.Deferred>(this.previousNodeRealizationResult);
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
                var output = this.previousNodeRealizationResult.Value;
                if (!output.Success)
                {
                    return new RealizationResult<char, Slash<ParseMode.Realized>>(false, default, output.RemainingTokens);
                }

                var input = output.RemainingTokens;
                if (input == null)
                {
                    return new RealizationResult<char, Slash<ParseMode.Realized>>(false, default, output.RemainingTokens);
                }

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

        public static class AlphaNumeric
        {
            public static AlphaNumeric<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return AlphaNumeric<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
            }
        }

        public abstract class AlphaNumeric<TMode> : IAstNode<char, AlphaNumeric<ParseMode.Realized>>, IFromRealizedable<AlphaNumeric<ParseMode.Deferred>> where TMode : ParseMode
        {
            public static AlphaNumeric<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return AlphaNumeric<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
            }

            private AlphaNumeric()
            {
                //// TODO get all of the access modifiers correct
            }

            public abstract AlphaNumeric<ParseMode.Deferred> Convert();

            public abstract IRealizationResult<char, AlphaNumeric<ParseMode.Realized>> Realize();

            public sealed class Deferred : AlphaNumeric<ParseMode.Deferred>
            {
                private readonly IFuture<IRealizationResult<char>> previousNodeRealizationResult;

                private readonly IFuture<IRealizationResult<char, AlphaNumeric<ParseMode.Realized>>> realizationResult;

                internal static AlphaNumeric<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    return new AlphaNumeric<ParseMode.Deferred>.Deferred(previousNodeRealizationResult);
                }

                private Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    this.previousNodeRealizationResult = previousNodeRealizationResult;

                    this.realizationResult = Future.Create(() => this.RealizeImpl());
                }

                internal Deferred(IFuture<IRealizationResult<char, AlphaNumeric<ParseMode.Realized>>> realizationResult)
                {
                    this.realizationResult = realizationResult;
                }

                public override AlphaNumeric<ParseMode.Deferred> Convert()
                {
                    return this;
                }

                public override IRealizationResult<char, AlphaNumeric<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult.Value;
                }

                private IRealizationResult<char, AlphaNumeric<ParseMode.Realized>> RealizeImpl()
                {
                    if (!this.previousNodeRealizationResult.Value.Success)
                    {
                        return new RealizationResult<char, AlphaNumeric<ParseMode.Realized>>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
                    }

                    var a = AlphaNumeric<ParseMode.Realized>.Realized.A.Create(this.previousNodeRealizationResult);
                    if (a.Success)
                    {
                        return a;
                    }

                    var c = AlphaNumeric<ParseMode.Realized>.Realized.C.Create(this.previousNodeRealizationResult);
                    if (c.Success)
                    {
                        return c;
                    }

                    return new RealizationResult<char, AlphaNumeric<ParseMode.Realized>>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
                }
            }


            public abstract class Realized : AlphaNumeric<ParseMode.Realized>
            {
                private Realized()
                {
                }

                protected abstract TResult Dispatch<TResult, TContext>(AlphaNumeric<TMode>.Realized.Visitor<TResult, TContext> visitor, TContext context);

                public abstract class Visitor<TResult, TContext>
                {
                    public TResult Visit(AlphaNumeric<ParseMode.Realized> node, TContext context)
                    {
                        //// TODO is there a way to avoid this cast?
                        return (node as AlphaNumeric<TMode>.Realized)!.Dispatch(this, context);
                    }

                    public TResult Visit(AlphaNumeric<TMode>.Realized node, TContext context)
                    {
                        return node.Dispatch(this, context);
                    }

                    protected internal abstract TResult Accept(AlphaNumeric<TMode>.Realized.A node, TContext context);
                    protected internal abstract TResult Accept(AlphaNumeric<TMode>.Realized.C node, TContext context);
                }

                public sealed class A : AlphaNumeric<TMode>.Realized
                {
                    /*
                    internal static IRealizationResult<char, _alphaNumeric<ParseMode.Realized>.Realized._ʺx41ʺ> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _ʺx41ʺ_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx41ʺ.Create(previousNodeRealizationResult).Realize();
                    if (!_ʺx41ʺ_1.Success)
                    {
                        return new RealizationResult<char, _alphaNumeric<ParseMode.Realized>.Realized._ʺx41ʺ>(false, default, previousNodeRealizationResult.Value.RemainingTokens);
                    }

                    var node = new _ʺx41ʺ(_ʺx41ʺ_1.RealizedValue, _ʺx41ʺ_1.RemainingTokens);
                    return node.realizationResult;
                }
                    */
                    internal static IRealizationResult<char, AlphaNumeric<ParseMode.Realized>.Realized.A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                    {
                        var output = previousNodeRealizationResult.Value;
                        if (!output.Success)
                        {
                            return new RealizationResult<char, AlphaNumeric<ParseMode.Realized>.Realized.A>(false, default, output.RemainingTokens);
                        }

                        var input = output.RemainingTokens;
                        if (input == null)
                        {
                            return new RealizationResult<char, AlphaNumeric<ParseMode.Realized>.Realized.A>(false, default, input);
                        }

                        if (input.Current == 'A')
                        {
                            var a = new AlphaNumeric<ParseMode.Realized>.Realized.A(input.Next());
                            return a.RealizationResult;
                        }
                        else
                        {
                            return new RealizationResult<char, AlphaNumeric<ParseMode.Realized>.Realized.A>(false, default, input);
                        }
                    }

                    private A(ITokenStream<char>? nextTokens)
                    {
                        if (typeof(TMode) != typeof(ParseMode.Realized))
                        {
                            throw new Exception("tODO");
                        }

                        this.RealizationResult = new RealizationResult<char, AlphaNumeric<TMode>.Realized.A>(true, this, nextTokens);
                    }

                    private IRealizationResult<char, AlphaNumeric<TMode>.Realized.A> RealizationResult { get; }

                    public override IRealizationResult<char, AlphaNumeric<ParseMode.Realized>> Realize()
                    {
                        return this.RealizationResult;
                    }

                    public override AlphaNumeric<ParseMode.Deferred> Convert()
                    {
                        return new AlphaNumeric<ParseMode.Deferred>.Deferred(Future.Create(() => this.RealizationResult));
                    }

                    protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                    {
                        return visitor.Accept(this, context);
                    }
                }

                public sealed class C : AlphaNumeric<TMode>.Realized
                {
                    internal static IRealizationResult<char, AlphaNumeric<ParseMode.Realized>.Realized.C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                    {
                        var output = previousNodeRealizationResult.Value;
                        if (!output.Success)
                        {
                            return new RealizationResult<char, AlphaNumeric<ParseMode.Realized>.Realized.C>(false, default, output.RemainingTokens);
                        }

                        var input = output.RemainingTokens;
                        if (input == null)
                        {
                            return new RealizationResult<char, AlphaNumeric<ParseMode.Realized>.Realized.C>(false, default, input);
                        }

                        if (input.Current == 'C')
                        {
                            var a = new AlphaNumeric<ParseMode.Realized>.Realized.C(input.Next());
                            return a.RealizationResult;
                        }
                        else
                        {
                            return new RealizationResult<char, AlphaNumeric<ParseMode.Realized>.Realized.C>(false, default, input);
                        }
                    }

                    private C(ITokenStream<char>? nextTokens)
                    {
                        if (typeof(TMode) != typeof(ParseMode.Realized))
                        {
                            throw new Exception("tODO");
                        }

                        this.RealizationResult = new RealizationResult<char, AlphaNumeric<TMode>.Realized.C>(true, this, nextTokens);
                    }

                    private IRealizationResult<char, AlphaNumeric<TMode>.Realized.C> RealizationResult { get; }

                    public override IRealizationResult<char, AlphaNumeric<ParseMode.Realized>> Realize()
                    {
                        return this.RealizationResult;
                    }

                    public override AlphaNumeric<ParseMode.Deferred> Convert()
                    {
                        return new AlphaNumeric<ParseMode.Deferred>.Deferred(Future.Create(() => this.RealizationResult));
                    }

                    protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                    {
                        return visitor.Accept(this, context);
                    }
                }
            }
        }

        public sealed class AlphaNumericDeferred : IAstNode<char, AlphaNumericRealized>
        {
            private readonly IFuture<IRealizationResult<char>> previousNodeRealizationResult;

            private readonly IFuture<IRealizationResult<char, AlphaNumericRealized>> realizationResult;

            public AlphaNumericDeferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                //// TODO get constructor accessibility correct
                this.previousNodeRealizationResult = previousNodeRealizationResult;

                this.realizationResult = Future.Create(() => this.RealizeImpl());
            }

            public AlphaNumericDeferred(IFuture<IRealizationResult<char, AlphaNumericRealized>> realizationResult)
            {
                this.realizationResult = realizationResult;
            }

            public IRealizationResult<char, AlphaNumericRealized> Realize()
            {
                return this.realizationResult.Value;
            }

            private IRealizationResult<char, AlphaNumericRealized> RealizeImpl()
            {
                if (!this.previousNodeRealizationResult.Value.Success)
                {
                    return new RealizationResult<char, AlphaNumericRealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
                }

                var a = AlphaNumericRealized.A.Create(this.previousNodeRealizationResult);
                if (a.Success)
                {
                    return a;
                }

                var c = AlphaNumericRealized.C.Create(this.previousNodeRealizationResult);
                if (c.Success)
                {
                    return c;
                }

                return new RealizationResult<char, AlphaNumericRealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
            }
        }

        public abstract class AlphaNumericRealized : IFromRealizedable<AlphaNumericDeferred>
        {
            private AlphaNumericRealized()
            {
            }

            public abstract AlphaNumericDeferred Convert();

            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(AlphaNumericRealized node, TContext context)
                {
                    return node.Dispatch(this, context);
                }

                protected internal abstract TResult Accept(A node, TContext context);
                protected internal abstract TResult Accept(C node, TContext context);
            }

            public sealed class A : AlphaNumericRealized
            {
                public static IRealizationResult<char, AlphaNumericRealized.A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var output = previousNodeRealizationResult.Value;
                    if (!output.Success)
                    {
                        return new RealizationResult<char, AlphaNumericRealized.A>(false, default, output.RemainingTokens);
                    }

                    var input = output.RemainingTokens;
                    if (input == null)
                    {
                        return new RealizationResult<char, AlphaNumericRealized.A>(false, default, input);
                    }

                    if (input.Current == 'A')
                    {
                        var a = new AlphaNumericRealized.A(input.Next());
                        return a.RealizationResult;
                    }
                    else
                    {
                        return new RealizationResult<char, AlphaNumericRealized.A>(false, default, input);
                    }
                }

                private A(ITokenStream<char>? nextTokens)
                {
                    this.RealizationResult = new RealizationResult<char, AlphaNumericRealized.A>(true, this, nextTokens);
                }

                private IRealizationResult<char, AlphaNumericRealized.A> RealizationResult { get; }

                public override AlphaNumericDeferred Convert()
                {
                    return new AlphaNumericDeferred(Future.Create(() => this.RealizationResult));
                }

                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }

            public sealed class C : AlphaNumericRealized
            {
                public static IRealizationResult<char, AlphaNumericRealized.C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var output = previousNodeRealizationResult.Value;
                    if (!output.Success)
                    {
                        return new RealizationResult<char, AlphaNumericRealized.C>(false, default, output.RemainingTokens);
                    }

                    var input = output.RemainingTokens;
                    if (input == null)
                    {
                        return new RealizationResult<char, AlphaNumericRealized.C>(false, default, input);
                    }

                    if (input.Current == 'C')
                    {
                        var c = new AlphaNumericRealized.C(input.Next());
                        return c.RealizationResult;
                    }
                    else
                    {
                        return new RealizationResult<char, AlphaNumericRealized.C>(false, default, input);
                    }
                }

                private C(ITokenStream<char>? nextTokens)
                {
                    this.RealizationResult = new RealizationResult<char, AlphaNumericRealized.C>(true, this, nextTokens);
                }

                private IRealizationResult<char, AlphaNumericRealized.C> RealizationResult { get; }

                public override AlphaNumericDeferred Convert()
                {
                    return new AlphaNumericDeferred(Future.Create(() => this.RealizationResult));
                }

                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }

        public static class Segment
        {
            public static Segment<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return Segment<ParseMode.Deferred>.Create(previousNodeRealizationResult);
            }

            public static Segment<ParseMode.Deferred> Create(
                IFuture<Slash<ParseMode.Deferred>> slash,
                IFuture<AtLeastOne<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, ParseMode.Deferred>> characters)
            {
                return Segment<ParseMode.Deferred>.Create(slash, characters);
            }
        }

        public sealed class Segment<TMode> : IAstNode<char, Segment<ParseMode.Realized>>, IFromRealizedable<Segment<ParseMode.Deferred>> where TMode : ParseMode
        {
            private readonly IFuture<Slash<TMode>> slash;
            private readonly IFuture<AtLeastOne<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, TMode>> characters;

            private readonly Future<IRealizationResult<char, Segment<ParseMode.Realized>>> cachedOutput;

            internal static Segment<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var slash = new Future<Slash<ParseMode.Deferred>>(() => V3ParserPlayground.Slash.Create(previousNodeRealizationResult));
                var characters = new Future<AtLeastOne<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, ParseMode.Deferred>>(() => AtLeastOne.Create<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>>(
                        Future.Create(() => slash.Value.Realize()), //// TODO the first parameter has a closure...
                        input => AlphaNumeric.Create(input)));
                return new Segment<ParseMode.Deferred>(slash, characters);
            }

            internal static Segment<ParseMode.Deferred> Create(
                IFuture<Slash<ParseMode.Deferred>> slash,
                IFuture<AtLeastOne<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, ParseMode.Deferred>> characters)
            {
                return new Segment<ParseMode.Deferred>(slash, characters);
            }

            private Segment(
                IFuture<Slash<TMode>> slash, 
                IFuture<AtLeastOne<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, TMode>> characters)
            {
                this.slash = slash;
                this.characters = characters;

                this.cachedOutput = new Future<IRealizationResult<char, Segment<ParseMode.Realized>>>(this.RealizeImpl);
            }

            private Segment(
                Slash<TMode> slash, 
                AtLeastOne<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, TMode> characters,
                Future<IRealizationResult<char, Segment<ParseMode.Realized>>> cachedOutput)
            {
                this.slash = new Future<Slash<TMode>>(() => slash);

                this.characters = new Future<AtLeastOne<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, TMode>>(
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

            public AtLeastOne<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, TMode> Characters
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
            public static EqualsSign<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return EqualsSign<ParseMode.Deferred>.Create(previousNodeRealizationResult);
            }
        }

        public sealed class EqualsSign<TMode> : IAstNode<char, EqualsSign<ParseMode.Realized>>, IFromRealizedable<EqualsSign<ParseMode.Deferred>> where TMode : ParseMode
        {
            private readonly IFuture<IRealizationResult<char>> previousNodeRealizationResult;

            private readonly Future<IRealizationResult<char, EqualsSign<ParseMode.Realized>>> cachedOutput;

            internal static EqualsSign<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new EqualsSign<ParseMode.Deferred>(previousNodeRealizationResult);
            }

            private EqualsSign(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;

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
                var output = this.previousNodeRealizationResult.Value;
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
                    return new EqualsSign<ParseMode.Deferred>(this.previousNodeRealizationResult);
                }
                else
                {
                    return new EqualsSign<ParseMode.Deferred>(this.cachedOutput);
                }
            }
        }

        public static class OptionName
        {
            public static OptionName<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return OptionName<ParseMode.Deferred>.Create(previousNodeRealizationResult);
            }
        }

        public sealed class OptionName<TMode> : IAstNode<char, OptionName<ParseMode.Realized>>, IFromRealizedable<OptionName<ParseMode.Deferred>>
            where TMode : ParseMode
        {
            private readonly IFuture
                <
                    AtLeastOne<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, TMode>
                > 
                    characters;

            private readonly Future<IRealizationResult<char, OptionName<ParseMode.Realized>>> cachedOutput;

            internal static OptionName<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var characters = new Future<AtLeastOne<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, ParseMode.Deferred>>(
                        () =>
                            AtLeastOne.Create<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>>(
                                previousNodeRealizationResult,
                                input => AlphaNumeric.Create(input)));
                return new OptionName<ParseMode.Deferred>(characters);
            }

            internal OptionName(IFuture<AtLeastOne<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, TMode>> characters)
            {
                this.characters = characters;

                this.cachedOutput = new Future<IRealizationResult<char, OptionName<ParseMode.Realized>>>(this.RealizeImpl);
            }

            private OptionName(
                AtLeastOne<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, TMode> characters,
                Future<IRealizationResult<char, OptionName<ParseMode.Realized>>> cachedOutput)
            {
                this.characters = 
                    new Future<AtLeastOne<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, TMode>>(
                        () => characters);

                this.cachedOutput = cachedOutput;
            }

            public AtLeastOne<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, TMode> Characters
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
            public static OptionValue<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return OptionValue<ParseMode.Deferred>.Create(previousNodeRealizationResult);
            }
        }

        public sealed class OptionValue<TMode> : IAstNode<char, OptionValue<ParseMode.Realized>>, IFromRealizedable<OptionValue<ParseMode.Deferred>> where TMode : ParseMode
        {
            private readonly IFuture<Many<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, TMode>> characters;

            private readonly Future<IRealizationResult<char, OptionValue<ParseMode.Realized>>> cachedOutput;

            internal static OptionValue<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var characters = new Future<Many<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, ParseMode.Deferred>>(
                    () =>
                        Many.Create<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>> (
                            input => AlphaNumeric.Create(input),
                            previousNodeRealizationResult));

                return new OptionValue<ParseMode.Deferred>(characters);
            }

            internal OptionValue(
                IFuture<Many<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, TMode>> characters)
            {
                this.characters = characters;

                this.cachedOutput = new Future<IRealizationResult<char, OptionValue<ParseMode.Realized>>>(this.RealizeImpl);
            }

            private OptionValue(
                Many<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, TMode> characters,
                Future<IRealizationResult<char, OptionValue<ParseMode.Realized>>> cachedOutput)
            {
                this.characters = new Future<Many<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, TMode>>(
                    () => characters);

                this.cachedOutput = cachedOutput;
            }

            public Many<AlphaNumeric<ParseMode.Deferred>, AlphaNumeric<ParseMode.Realized>, TMode> Characters
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
            public static QueryOption<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return QueryOption<ParseMode.Deferred>.Create(previousNodeRealizationResult);
            }
        }

        public sealed class QueryOption<TMode> : IAstNode<char, QueryOption<ParseMode.Realized>>, IFromRealizedable<QueryOption<ParseMode.Deferred>> where TMode : ParseMode
        {
            private readonly IFuture<OptionName<TMode>> name;
            private readonly IFuture<EqualsSign<TMode>> equalsSign;
            private readonly IFuture<OptionValue<TMode>> optionValue;

            private Future<IRealizationResult<char, QueryOption<ParseMode.Realized>>> cachedOutput;

            internal static QueryOption<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var name = new Future<OptionName<ParseMode.Deferred>>(() => OptionName.Create(previousNodeRealizationResult));
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
            public static QuestionMark<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return QuestionMark<ParseMode.Deferred>.Create(previousNodeRealizationResult);
            }
        }

        public sealed class QuestionMark<TMode> : IAstNode<char, QuestionMark<ParseMode.Realized>>, IFromRealizedable<QuestionMark<ParseMode.Deferred>> where TMode : ParseMode
        {
            private readonly IFuture<IRealizationResult<char>> previousNodeRealizationResult;

            private readonly Future<IRealizationResult<char, QuestionMark<ParseMode.Realized>>> cachedOutput;

            internal static QuestionMark<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new QuestionMark<ParseMode.Deferred>(previousNodeRealizationResult);
            }

            private QuestionMark(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;

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
                var output = this.previousNodeRealizationResult.Value;
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
                        this.previousNodeRealizationResult);
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

            public static OdataUri<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return OdataUri<ParseMode.Deferred>.Create(previousNodeRealizationResult);
            }
        }

        public sealed class OdataUri<TMode> : IAstNode<char, OdataUri<ParseMode.Realized>> where TMode : ParseMode
        {
            private readonly IFuture<AtLeastOne<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>, TMode>> segments;
            private readonly IFuture<QuestionMark<TMode>> questionMark;
            private readonly IFuture<Many<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>, TMode>> queryOptions;

            private readonly Future<IRealizationResult<char, OdataUri<ParseMode.Realized>>> cachedOutput;

            internal static OdataUri<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var segments = new Future<AtLeastOne<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>, ParseMode.Deferred>>(
                    () => AtLeastOne.Create<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>>(
                        previousNodeRealizationResult,
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

namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx61ⲻ7A<TMode> : IAstNode<char, _Ⰳx61ⲻ7A<ParseMode.Realized>>, IFromRealizedable<_Ⰳx61ⲻ7A<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _Ⰳx61ⲻ7A()
        {
        }
        
        internal static _Ⰳx61ⲻ7A<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _Ⰳx61ⲻ7A<ParseMode.Deferred>
        {
            private Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;
                
                this.realizationResult = Future.Create(() => this.RealizeImpl());
            }
            internal Deferred(IFuture<IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>>> realizationResult)
            {
                this.realizationResult = realizationResult;
            }
            
            private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
            private IFuture<IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>>> realizationResult { get; }
            
            internal static _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(previousNodeRealizationResult);
            }
            
            public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
            {
                return this;
            }
            
            public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
            {
                return this.realizationResult.Value;
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
        
        public abstract class Realized : _Ⰳx61ⲻ7A<ParseMode.Realized>
        {
            private Realized()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(_Ⰳx61ⲻ7A<TMode>.Realized.Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_Ⰳx61ⲻ7A<TMode>.Realized node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                public TResult Visit(_Ⰳx61ⲻ7A<ParseMode.Realized> node, TContext context)
                {
                    //// TODO is there a way to avoid this cast?
return (node as _Ⰳx61ⲻ7A<TMode>.Realized)!.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._61 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._62 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._63 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._64 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._65 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._66 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._67 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._68 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._69 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._6A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._6B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._6C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._6D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._6E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._6F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._70 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._71 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._72 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._73 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._74 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._75 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._76 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._77 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._78 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._79 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx61ⲻ7A<TMode>.Realized._7A node, TContext context);
            }
            
            public sealed class _61 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _61(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._61>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._61> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._61> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _62 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _62(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._62>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._62> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._62> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _63 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _63(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._63>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._63> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._63> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _64 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _64(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._64>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._64> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._64> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _65 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _65(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._65>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._65> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._65> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _66 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _66(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._6_2 = _6_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._66>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._66> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._66> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _67 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _67(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._67>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._67> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._67> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _68 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _68(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._68>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._68> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._68> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _69 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _69(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._69>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._69> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._69> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6A : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _6A(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._6A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._6A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._6A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6B : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _6B(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._6B>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._6B> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._6B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6C : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _6C(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._6C>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._6C> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._6C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6D : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _6D(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._6D>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._6D> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._6D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6E : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _6E(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._6E>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._6E> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._6E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6F : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _6F(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._6F>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._6F> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._6F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _70 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _70(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._70>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._70> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._70> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _71 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _71(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._71>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._71> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._71> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _72 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _72(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._72>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._72> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._72> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _73 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _73(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._73>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._73> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._73> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _74 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _74(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._74>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._74> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._74> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _75 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _75(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._75>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._75> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._75> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _76 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _76(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._76>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._76> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._76> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _77 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _77(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._7_2 = _7_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._77>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._77> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._77> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _78 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _78(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._78>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._78> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._78> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _79 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _79(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._79>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._79> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._79> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7A : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                private _7A(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._7A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx61ⲻ7A<TMode>.Realized._7A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>.Realized._7A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception("TODO");
                }
                
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
    }
    
}

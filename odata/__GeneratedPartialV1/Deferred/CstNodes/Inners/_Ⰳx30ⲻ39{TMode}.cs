namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx30ⲻ39<TMode> : IAstNode<char, _Ⰳx30ⲻ39<ParseMode.Realized>>, IFromRealizedable<_Ⰳx30ⲻ39<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _Ⰳx30ⲻ39()
        {
        }
        
        internal static _Ⰳx30ⲻ39<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _Ⰳx30ⲻ39<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _Ⰳx30ⲻ39<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _Ⰳx30ⲻ39<ParseMode.Deferred>
        {
            private Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;
                
                this.realizationResult = Future.Create(() => this.RealizeImpl());
            }
            internal Deferred(IFuture<IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>>> realizationResult)
            {
                this.realizationResult = realizationResult;
            }
            
            private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
            private IFuture<IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>>> realizationResult { get; }
            
            internal static _Ⰳx30ⲻ39<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new _Ⰳx30ⲻ39<ParseMode.Deferred>.Deferred(previousNodeRealizationResult);
            }
            
            public override _Ⰳx30ⲻ39<ParseMode.Deferred> Convert()
            {
                return this;
            }
            
            public override IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>> Realize()
            {
                return this.realizationResult.Value;
            }
            
            private IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
        
        public abstract class Realized : _Ⰳx30ⲻ39<ParseMode.Realized>
        {
            private Realized()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(_Ⰳx30ⲻ39<TMode>.Realized.Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_Ⰳx30ⲻ39<TMode>.Realized node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                public TResult Visit(_Ⰳx30ⲻ39<ParseMode.Realized> node, TContext context)
                {
                    //// TODO is there a way to avoid this cast?
return (node as _Ⰳx30ⲻ39<TMode>.Realized)!.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_Ⰳx30ⲻ39<TMode>.Realized._30 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx30ⲻ39<TMode>.Realized._31 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx30ⲻ39<TMode>.Realized._32 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx30ⲻ39<TMode>.Realized._33 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx30ⲻ39<TMode>.Realized._34 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx30ⲻ39<TMode>.Realized._35 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx30ⲻ39<TMode>.Realized._36 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx30ⲻ39<TMode>.Realized._37 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx30ⲻ39<TMode>.Realized._38 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx30ⲻ39<TMode>.Realized._39 node, TContext context);
            }
            
            public sealed class _30 : _Ⰳx30ⲻ39<TMode>.Realized
            {
                private _30(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._30>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._30> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                
                public override _Ⰳx30ⲻ39<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx30ⲻ39<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>.Realized._30> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _31 : _Ⰳx30ⲻ39<TMode>.Realized
            {
                private _31(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._31>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._31> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                
                public override _Ⰳx30ⲻ39<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx30ⲻ39<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>.Realized._31> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _32 : _Ⰳx30ⲻ39<TMode>.Realized
            {
                private _32(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._32>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._32> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                
                public override _Ⰳx30ⲻ39<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx30ⲻ39<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>.Realized._32> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _33 : _Ⰳx30ⲻ39<TMode>.Realized
            {
                private _33(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._3_2 = _3_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._33>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._33> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_2 { get; }
                
                public override _Ⰳx30ⲻ39<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx30ⲻ39<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>.Realized._33> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _34 : _Ⰳx30ⲻ39<TMode>.Realized
            {
                private _34(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._34>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._34> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                
                public override _Ⰳx30ⲻ39<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx30ⲻ39<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>.Realized._34> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _35 : _Ⰳx30ⲻ39<TMode>.Realized
            {
                private _35(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._35>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._35> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                
                public override _Ⰳx30ⲻ39<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx30ⲻ39<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>.Realized._35> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _36 : _Ⰳx30ⲻ39<TMode>.Realized
            {
                private _36(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._36>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._36> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                
                public override _Ⰳx30ⲻ39<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx30ⲻ39<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>.Realized._36> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _37 : _Ⰳx30ⲻ39<TMode>.Realized
            {
                private _37(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._37>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._37> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                
                public override _Ⰳx30ⲻ39<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx30ⲻ39<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>.Realized._37> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _38 : _Ⰳx30ⲻ39<TMode>.Realized
            {
                private _38(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._38>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._38> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                
                public override _Ⰳx30ⲻ39<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx30ⲻ39<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>.Realized._38> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _39 : _Ⰳx30ⲻ39<TMode>.Realized
            {
                private _39(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._39>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._39> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                
                public override _Ⰳx30ⲻ39<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx30ⲻ39<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>.Realized._39> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
        }
    }
    
}

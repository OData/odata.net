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
                if (!this.previousNodeRealizationResult.Value.Success)
{
return new RealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
}
return new RealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);

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
                private _30(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, ITokenStream<char>? nextTokens)
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
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._30> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._30>(false, default, _3_1.RemainingTokens);
}


var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _3_1)).Realize();
if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._30>(false, default, _0_1.RemainingTokens);
}


var node = new _30(_3_1.RealizedValue, _0_1.RealizedValue, _0_1.RemainingTokens);
return node.realizationResult;
                }
                
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
            }
            
            public sealed class _31 : _Ⰳx30ⲻ39<TMode>.Realized
            {
                private _31(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
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
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._31> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._31>(false, default, _3_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _3_1)).Realize();
if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._31>(false, default, _1_1.RemainingTokens);
}


var node = new _31(_3_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
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
            }
            
            public sealed class _32 : _Ⰳx30ⲻ39<TMode>.Realized
            {
                private _32(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, ITokenStream<char>? nextTokens)
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
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._32> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._32>(false, default, _3_1.RemainingTokens);
}


var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _3_1)).Realize();
if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._32>(false, default, _2_1.RemainingTokens);
}


var node = new _32(_3_1.RealizedValue, _2_1.RealizedValue, _2_1.RemainingTokens);
return node.realizationResult;
                }
                
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
            }
            
            public sealed class _33 : _Ⰳx30ⲻ39<TMode>.Realized
            {
                private _33(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_2, ITokenStream<char>? nextTokens)
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
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._33> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._33>(false, default, _3_1.RemainingTokens);
}


var _3_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _3_1)).Realize();
if (!_3_2.Success)
{
return new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._33>(false, default, _3_2.RemainingTokens);
}


var node = new _33(_3_1.RealizedValue, _3_2.RealizedValue, _3_2.RemainingTokens);
return node.realizationResult;
                }
                
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
            }
            
            public sealed class _34 : _Ⰳx30ⲻ39<TMode>.Realized
            {
                private _34(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, ITokenStream<char>? nextTokens)
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
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._34> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._34>(false, default, _3_1.RemainingTokens);
}


var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _3_1)).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._34>(false, default, _4_1.RemainingTokens);
}


var node = new _34(_3_1.RealizedValue, _4_1.RealizedValue, _4_1.RemainingTokens);
return node.realizationResult;
                }
                
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
            }
            
            public sealed class _35 : _Ⰳx30ⲻ39<TMode>.Realized
            {
                private _35(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, ITokenStream<char>? nextTokens)
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
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._35> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._35>(false, default, _3_1.RemainingTokens);
}


var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _3_1)).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._35>(false, default, _5_1.RemainingTokens);
}


var node = new _35(_3_1.RealizedValue, _5_1.RealizedValue, _5_1.RemainingTokens);
return node.realizationResult;
                }
                
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
            }
            
            public sealed class _36 : _Ⰳx30ⲻ39<TMode>.Realized
            {
                private _36(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, ITokenStream<char>? nextTokens)
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
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._36> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._36>(false, default, _3_1.RemainingTokens);
}


var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _3_1)).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._36>(false, default, _6_1.RemainingTokens);
}


var node = new _36(_3_1.RealizedValue, _6_1.RealizedValue, _6_1.RemainingTokens);
return node.realizationResult;
                }
                
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
            }
            
            public sealed class _37 : _Ⰳx30ⲻ39<TMode>.Realized
            {
                private _37(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, ITokenStream<char>? nextTokens)
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
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._37> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._37>(false, default, _3_1.RemainingTokens);
}


var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _3_1)).Realize();
if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._37>(false, default, _7_1.RemainingTokens);
}


var node = new _37(_3_1.RealizedValue, _7_1.RealizedValue, _7_1.RemainingTokens);
return node.realizationResult;
                }
                
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
            }
            
            public sealed class _38 : _Ⰳx30ⲻ39<TMode>.Realized
            {
                private _38(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
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
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._38> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._38>(false, default, _3_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _3_1)).Realize();
if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._38>(false, default, _8_1.RemainingTokens);
}


var node = new _38(_3_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
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
            }
            
            public sealed class _39 : _Ⰳx30ⲻ39<TMode>.Realized
            {
                private _39(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
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
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._39> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._39>(false, default, _3_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _3_1)).Realize();
if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx30ⲻ39<TMode>.Realized._39>(false, default, _9_1.RemainingTokens);
}


var node = new _39(_3_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
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
            }
        }
    }
    
}

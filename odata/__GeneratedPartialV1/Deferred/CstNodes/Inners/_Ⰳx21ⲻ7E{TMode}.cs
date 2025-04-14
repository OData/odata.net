namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx21ⲻ7E<TMode> : IAstNode<char, _Ⰳx21ⲻ7E<ParseMode.Realized>>, IFromRealizedable<_Ⰳx21ⲻ7E<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _Ⰳx21ⲻ7E()
        {
        }
        
        internal static _Ⰳx21ⲻ7E<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _Ⰳx21ⲻ7E<ParseMode.Deferred>
        {
            private Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;
                
                this.realizationResult = Future.Create(() => this.RealizeImpl());
            }
            internal Deferred(IFuture<IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>>> realizationResult)
            {
                this.realizationResult = realizationResult;
            }
            
            private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
            private IFuture<IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>>> realizationResult { get; }
            
            internal static _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(previousNodeRealizationResult);
            }
            
            public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
            {
                return this;
            }
            
            public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
            {
                return this.realizationResult.Value;
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
        
        public abstract class Realized : _Ⰳx21ⲻ7E<ParseMode.Realized>
        {
            private Realized()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(_Ⰳx21ⲻ7E<TMode>.Realized.Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_Ⰳx21ⲻ7E<TMode>.Realized node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                public TResult Visit(_Ⰳx21ⲻ7E<ParseMode.Realized> node, TContext context)
                {
                    //// TODO is there a way to avoid this cast?
return (node as _Ⰳx21ⲻ7E<TMode>.Realized)!.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._21 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._22 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._23 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._24 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._25 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._26 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._27 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._28 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._29 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._2A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._2B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._2C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._2D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._2E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._2F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._30 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._31 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._32 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._33 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._34 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._35 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._36 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._37 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._38 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._39 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._3A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._3B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._3C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._3D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._3E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._3F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._40 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._41 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._42 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._43 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._44 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._45 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._46 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._47 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._48 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._49 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._4A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._4B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._4C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._4D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._4E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._4F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._50 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._51 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._52 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._53 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._54 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._55 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._56 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._57 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._58 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._59 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._5A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._5B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._5C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._5D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._5E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._5F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._60 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._61 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._62 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._63 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._64 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._65 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._66 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._67 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._68 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._69 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._6A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._6B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._6C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._6D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._6E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._6F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._70 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._71 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._72 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._73 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._74 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._75 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._76 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._77 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._78 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._79 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._7A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._7B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._7C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._7D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx21ⲻ7E<TMode>.Realized._7E node, TContext context);
            }
            
            public sealed class _21 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _21(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._21>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._21> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._21> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();
if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._21>(false, default, _2_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _2_1)).Realize();
if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._21>(false, default, _1_1.RemainingTokens);
}


var node = new _21(_2_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _22 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _22(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._2_2 = _2_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._22>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._22> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._22> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();
if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._22>(false, default, _2_1.RemainingTokens);
}


var _2_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _2_1)).Realize();
if (!_2_2.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._22>(false, default, _2_2.RemainingTokens);
}


var node = new _22(_2_1.RealizedValue, _2_2.RealizedValue, _2_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _23 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _23(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._23>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._23> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._23> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();
if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._23>(false, default, _2_1.RemainingTokens);
}


var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _2_1)).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._23>(false, default, _3_1.RemainingTokens);
}


var node = new _23(_2_1.RealizedValue, _3_1.RealizedValue, _3_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _24 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _24(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._24>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._24> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._24> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();
if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._24>(false, default, _2_1.RemainingTokens);
}


var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _2_1)).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._24>(false, default, _4_1.RemainingTokens);
}


var node = new _24(_2_1.RealizedValue, _4_1.RealizedValue, _4_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _25 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _25(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._25>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._25> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._25> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();
if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._25>(false, default, _2_1.RemainingTokens);
}


var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _2_1)).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._25>(false, default, _5_1.RemainingTokens);
}


var node = new _25(_2_1.RealizedValue, _5_1.RealizedValue, _5_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _26 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _26(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._26>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._26> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._26> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();
if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._26>(false, default, _2_1.RemainingTokens);
}


var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _2_1)).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._26>(false, default, _6_1.RemainingTokens);
}


var node = new _26(_2_1.RealizedValue, _6_1.RealizedValue, _6_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _27 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _27(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._27>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._27> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._27> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();
if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._27>(false, default, _2_1.RemainingTokens);
}


var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _2_1)).Realize();
if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._27>(false, default, _7_1.RemainingTokens);
}


var node = new _27(_2_1.RealizedValue, _7_1.RealizedValue, _7_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _28 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _28(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._28>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._28> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._28> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();
if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._28>(false, default, _2_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _2_1)).Realize();
if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._28>(false, default, _8_1.RemainingTokens);
}


var node = new _28(_2_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _29 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _29(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._29>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._29> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._29> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();
if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._29>(false, default, _2_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _2_1)).Realize();
if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._29>(false, default, _9_1.RemainingTokens);
}


var node = new _29(_2_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2A : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _2A(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();
if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2A>(false, default, _2_1.RemainingTokens);
}


var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _2_1)).Realize();
if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2A>(false, default, _A_1.RemainingTokens);
}


var node = new _2A(_2_1.RealizedValue, _A_1.RealizedValue, _A_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2B : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _2B(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2B>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2B> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();
if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2B>(false, default, _2_1.RemainingTokens);
}


var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(Future.Create(() => _2_1)).Realize();
if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2B>(false, default, _B_1.RemainingTokens);
}


var node = new _2B(_2_1.RealizedValue, _B_1.RealizedValue, _B_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2C : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _2C(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2C>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2C> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();
if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2C>(false, default, _2_1.RemainingTokens);
}


var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(Future.Create(() => _2_1)).Realize();
if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2C>(false, default, _C_1.RemainingTokens);
}


var node = new _2C(_2_1.RealizedValue, _C_1.RealizedValue, _C_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2D : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _2D(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2D>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2D> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();
if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2D>(false, default, _2_1.RemainingTokens);
}


var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _2_1)).Realize();
if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2D>(false, default, _D_1.RemainingTokens);
}


var node = new _2D(_2_1.RealizedValue, _D_1.RealizedValue, _D_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2E : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _2E(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2E>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2E> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();
if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2E>(false, default, _2_1.RemainingTokens);
}


var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(Future.Create(() => _2_1)).Realize();
if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2E>(false, default, _E_1.RemainingTokens);
}


var node = new _2E(_2_1.RealizedValue, _E_1.RealizedValue, _E_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2F : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _2F(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2F>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2F> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();
if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2F>(false, default, _2_1.RemainingTokens);
}


var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(Future.Create(() => _2_1)).Realize();
if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._2F>(false, default, _F_1.RemainingTokens);
}


var node = new _2F(_2_1.RealizedValue, _F_1.RealizedValue, _F_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _30 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _30(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._30>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._30> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._30> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._30>(false, default, _3_1.RemainingTokens);
}


var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _3_1)).Realize();
if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._30>(false, default, _0_1.RemainingTokens);
}


var node = new _30(_3_1.RealizedValue, _0_1.RealizedValue, _0_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _31 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _31(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._31>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._31> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._31> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._31>(false, default, _3_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _3_1)).Realize();
if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._31>(false, default, _1_1.RemainingTokens);
}


var node = new _31(_3_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _32 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _32(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._32>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._32> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._32> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._32>(false, default, _3_1.RemainingTokens);
}


var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _3_1)).Realize();
if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._32>(false, default, _2_1.RemainingTokens);
}


var node = new _32(_3_1.RealizedValue, _2_1.RealizedValue, _2_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _33 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _33(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._3_2 = _3_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._33>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._33> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._33> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._33>(false, default, _3_1.RemainingTokens);
}


var _3_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _3_1)).Realize();
if (!_3_2.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._33>(false, default, _3_2.RemainingTokens);
}


var node = new _33(_3_1.RealizedValue, _3_2.RealizedValue, _3_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _34 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _34(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._34>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._34> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._34> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._34>(false, default, _3_1.RemainingTokens);
}


var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _3_1)).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._34>(false, default, _4_1.RemainingTokens);
}


var node = new _34(_3_1.RealizedValue, _4_1.RealizedValue, _4_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _35 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _35(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._35>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._35> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._35> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._35>(false, default, _3_1.RemainingTokens);
}


var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _3_1)).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._35>(false, default, _5_1.RemainingTokens);
}


var node = new _35(_3_1.RealizedValue, _5_1.RealizedValue, _5_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _36 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _36(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._36>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._36> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._36> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._36>(false, default, _3_1.RemainingTokens);
}


var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _3_1)).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._36>(false, default, _6_1.RemainingTokens);
}


var node = new _36(_3_1.RealizedValue, _6_1.RealizedValue, _6_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _37 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _37(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._37>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._37> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._37> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._37>(false, default, _3_1.RemainingTokens);
}


var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _3_1)).Realize();
if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._37>(false, default, _7_1.RemainingTokens);
}


var node = new _37(_3_1.RealizedValue, _7_1.RealizedValue, _7_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _38 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _38(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._38>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._38> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._38> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._38>(false, default, _3_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _3_1)).Realize();
if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._38>(false, default, _8_1.RemainingTokens);
}


var node = new _38(_3_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _39 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _39(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._39>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._39> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._39> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._39>(false, default, _3_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _3_1)).Realize();
if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._39>(false, default, _9_1.RemainingTokens);
}


var node = new _39(_3_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3A : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _3A(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3A>(false, default, _3_1.RemainingTokens);
}


var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _3_1)).Realize();
if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3A>(false, default, _A_1.RemainingTokens);
}


var node = new _3A(_3_1.RealizedValue, _A_1.RealizedValue, _A_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3B : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _3B(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3B>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3B> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3B>(false, default, _3_1.RemainingTokens);
}


var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(Future.Create(() => _3_1)).Realize();
if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3B>(false, default, _B_1.RemainingTokens);
}


var node = new _3B(_3_1.RealizedValue, _B_1.RealizedValue, _B_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3C : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _3C(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3C>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3C> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3C>(false, default, _3_1.RemainingTokens);
}


var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(Future.Create(() => _3_1)).Realize();
if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3C>(false, default, _C_1.RemainingTokens);
}


var node = new _3C(_3_1.RealizedValue, _C_1.RealizedValue, _C_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3D : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _3D(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3D>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3D> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3D>(false, default, _3_1.RemainingTokens);
}


var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _3_1)).Realize();
if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3D>(false, default, _D_1.RemainingTokens);
}


var node = new _3D(_3_1.RealizedValue, _D_1.RealizedValue, _D_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3E : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _3E(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3E>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3E> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3E>(false, default, _3_1.RemainingTokens);
}


var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(Future.Create(() => _3_1)).Realize();
if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3E>(false, default, _E_1.RemainingTokens);
}


var node = new _3E(_3_1.RealizedValue, _E_1.RealizedValue, _E_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3F : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _3F(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3F>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3F> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3F>(false, default, _3_1.RemainingTokens);
}


var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(Future.Create(() => _3_1)).Realize();
if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._3F>(false, default, _F_1.RemainingTokens);
}


var node = new _3F(_3_1.RealizedValue, _F_1.RealizedValue, _F_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _40 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _40(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._40>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._40> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._40> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._40>(false, default, _4_1.RemainingTokens);
}


var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _4_1)).Realize();
if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._40>(false, default, _0_1.RemainingTokens);
}


var node = new _40(_4_1.RealizedValue, _0_1.RealizedValue, _0_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _41 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _41(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._41>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._41> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._41> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._41>(false, default, _4_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _4_1)).Realize();
if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._41>(false, default, _1_1.RemainingTokens);
}


var node = new _41(_4_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _42 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _42(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._42>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._42> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._42> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._42>(false, default, _4_1.RemainingTokens);
}


var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _4_1)).Realize();
if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._42>(false, default, _2_1.RemainingTokens);
}


var node = new _42(_4_1.RealizedValue, _2_1.RealizedValue, _2_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _43 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _43(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._43>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._43> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._43> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._43>(false, default, _4_1.RemainingTokens);
}


var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _4_1)).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._43>(false, default, _3_1.RemainingTokens);
}


var node = new _43(_4_1.RealizedValue, _3_1.RealizedValue, _3_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _44 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _44(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._4_2 = _4_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._44>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._44> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._44> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._44>(false, default, _4_1.RemainingTokens);
}


var _4_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _4_1)).Realize();
if (!_4_2.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._44>(false, default, _4_2.RemainingTokens);
}


var node = new _44(_4_1.RealizedValue, _4_2.RealizedValue, _4_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _45 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _45(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._45>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._45> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._45> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._45>(false, default, _4_1.RemainingTokens);
}


var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _4_1)).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._45>(false, default, _5_1.RemainingTokens);
}


var node = new _45(_4_1.RealizedValue, _5_1.RealizedValue, _5_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _46 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _46(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._46>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._46> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._46> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._46>(false, default, _4_1.RemainingTokens);
}


var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _4_1)).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._46>(false, default, _6_1.RemainingTokens);
}


var node = new _46(_4_1.RealizedValue, _6_1.RealizedValue, _6_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _47 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _47(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._47>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._47> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._47> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._47>(false, default, _4_1.RemainingTokens);
}


var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _4_1)).Realize();
if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._47>(false, default, _7_1.RemainingTokens);
}


var node = new _47(_4_1.RealizedValue, _7_1.RealizedValue, _7_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _48 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _48(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._48>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._48> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._48> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._48>(false, default, _4_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _4_1)).Realize();
if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._48>(false, default, _8_1.RemainingTokens);
}


var node = new _48(_4_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _49 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _49(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._49>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._49> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._49> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._49>(false, default, _4_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _4_1)).Realize();
if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._49>(false, default, _9_1.RemainingTokens);
}


var node = new _49(_4_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4A : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _4A(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4A>(false, default, _4_1.RemainingTokens);
}


var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _4_1)).Realize();
if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4A>(false, default, _A_1.RemainingTokens);
}


var node = new _4A(_4_1.RealizedValue, _A_1.RealizedValue, _A_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4B : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _4B(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4B>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4B> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4B>(false, default, _4_1.RemainingTokens);
}


var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(Future.Create(() => _4_1)).Realize();
if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4B>(false, default, _B_1.RemainingTokens);
}


var node = new _4B(_4_1.RealizedValue, _B_1.RealizedValue, _B_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4C : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _4C(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4C>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4C> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4C>(false, default, _4_1.RemainingTokens);
}


var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(Future.Create(() => _4_1)).Realize();
if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4C>(false, default, _C_1.RemainingTokens);
}


var node = new _4C(_4_1.RealizedValue, _C_1.RealizedValue, _C_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4D : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _4D(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4D>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4D> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4D>(false, default, _4_1.RemainingTokens);
}


var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _4_1)).Realize();
if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4D>(false, default, _D_1.RemainingTokens);
}


var node = new _4D(_4_1.RealizedValue, _D_1.RealizedValue, _D_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4E : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _4E(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4E>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4E> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4E>(false, default, _4_1.RemainingTokens);
}


var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(Future.Create(() => _4_1)).Realize();
if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4E>(false, default, _E_1.RemainingTokens);
}


var node = new _4E(_4_1.RealizedValue, _E_1.RealizedValue, _E_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4F : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _4F(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4F>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4F> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4F>(false, default, _4_1.RemainingTokens);
}


var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(Future.Create(() => _4_1)).Realize();
if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._4F>(false, default, _F_1.RemainingTokens);
}


var node = new _4F(_4_1.RealizedValue, _F_1.RealizedValue, _F_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _50 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _50(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._50>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._50> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._50> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._50>(false, default, _5_1.RemainingTokens);
}


var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _5_1)).Realize();
if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._50>(false, default, _0_1.RemainingTokens);
}


var node = new _50(_5_1.RealizedValue, _0_1.RealizedValue, _0_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _51 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _51(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._51>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._51> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._51> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._51>(false, default, _5_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _5_1)).Realize();
if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._51>(false, default, _1_1.RemainingTokens);
}


var node = new _51(_5_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _52 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _52(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._52>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._52> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._52> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._52>(false, default, _5_1.RemainingTokens);
}


var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _5_1)).Realize();
if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._52>(false, default, _2_1.RemainingTokens);
}


var node = new _52(_5_1.RealizedValue, _2_1.RealizedValue, _2_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _53 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _53(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._53>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._53> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._53> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._53>(false, default, _5_1.RemainingTokens);
}


var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _5_1)).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._53>(false, default, _3_1.RemainingTokens);
}


var node = new _53(_5_1.RealizedValue, _3_1.RealizedValue, _3_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _54 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _54(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._54>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._54> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._54> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._54>(false, default, _5_1.RemainingTokens);
}


var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _5_1)).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._54>(false, default, _4_1.RemainingTokens);
}


var node = new _54(_5_1.RealizedValue, _4_1.RealizedValue, _4_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _55 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _55(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._5_2 = _5_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._55>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._55> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._55> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._55>(false, default, _5_1.RemainingTokens);
}


var _5_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _5_1)).Realize();
if (!_5_2.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._55>(false, default, _5_2.RemainingTokens);
}


var node = new _55(_5_1.RealizedValue, _5_2.RealizedValue, _5_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _56 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _56(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._56>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._56> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._56> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._56>(false, default, _5_1.RemainingTokens);
}


var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _5_1)).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._56>(false, default, _6_1.RemainingTokens);
}


var node = new _56(_5_1.RealizedValue, _6_1.RealizedValue, _6_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _57 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _57(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._57>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._57> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._57> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._57>(false, default, _5_1.RemainingTokens);
}


var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _5_1)).Realize();
if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._57>(false, default, _7_1.RemainingTokens);
}


var node = new _57(_5_1.RealizedValue, _7_1.RealizedValue, _7_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _58 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _58(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._58>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._58> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._58> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._58>(false, default, _5_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _5_1)).Realize();
if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._58>(false, default, _8_1.RemainingTokens);
}


var node = new _58(_5_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _59 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _59(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._59>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._59> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._59> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._59>(false, default, _5_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _5_1)).Realize();
if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._59>(false, default, _9_1.RemainingTokens);
}


var node = new _59(_5_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5A : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _5A(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5A>(false, default, _5_1.RemainingTokens);
}


var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _5_1)).Realize();
if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5A>(false, default, _A_1.RemainingTokens);
}


var node = new _5A(_5_1.RealizedValue, _A_1.RealizedValue, _A_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5B : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _5B(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5B>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5B> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5B>(false, default, _5_1.RemainingTokens);
}


var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(Future.Create(() => _5_1)).Realize();
if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5B>(false, default, _B_1.RemainingTokens);
}


var node = new _5B(_5_1.RealizedValue, _B_1.RealizedValue, _B_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5C : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _5C(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5C>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5C> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5C>(false, default, _5_1.RemainingTokens);
}


var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(Future.Create(() => _5_1)).Realize();
if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5C>(false, default, _C_1.RemainingTokens);
}


var node = new _5C(_5_1.RealizedValue, _C_1.RealizedValue, _C_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5D : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _5D(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5D>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5D> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5D>(false, default, _5_1.RemainingTokens);
}


var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _5_1)).Realize();
if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5D>(false, default, _D_1.RemainingTokens);
}


var node = new _5D(_5_1.RealizedValue, _D_1.RealizedValue, _D_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5E : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _5E(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5E>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5E> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5E>(false, default, _5_1.RemainingTokens);
}


var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(Future.Create(() => _5_1)).Realize();
if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5E>(false, default, _E_1.RemainingTokens);
}


var node = new _5E(_5_1.RealizedValue, _E_1.RealizedValue, _E_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5F : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _5F(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5F>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5F> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5F>(false, default, _5_1.RemainingTokens);
}


var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(Future.Create(() => _5_1)).Realize();
if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._5F>(false, default, _F_1.RemainingTokens);
}


var node = new _5F(_5_1.RealizedValue, _F_1.RealizedValue, _F_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _60 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _60(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._60>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._60> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._60> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._60>(false, default, _6_1.RemainingTokens);
}


var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _6_1)).Realize();
if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._60>(false, default, _0_1.RemainingTokens);
}


var node = new _60(_6_1.RealizedValue, _0_1.RealizedValue, _0_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _61 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _61(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._61>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._61> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._61> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._61>(false, default, _6_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _6_1)).Realize();
if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._61>(false, default, _1_1.RemainingTokens);
}


var node = new _61(_6_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _62 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _62(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._62>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._62> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._62> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._62>(false, default, _6_1.RemainingTokens);
}


var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _6_1)).Realize();
if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._62>(false, default, _2_1.RemainingTokens);
}


var node = new _62(_6_1.RealizedValue, _2_1.RealizedValue, _2_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _63 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _63(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._63>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._63> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._63> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._63>(false, default, _6_1.RemainingTokens);
}


var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _6_1)).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._63>(false, default, _3_1.RemainingTokens);
}


var node = new _63(_6_1.RealizedValue, _3_1.RealizedValue, _3_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _64 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _64(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._64>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._64> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._64> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._64>(false, default, _6_1.RemainingTokens);
}


var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _6_1)).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._64>(false, default, _4_1.RemainingTokens);
}


var node = new _64(_6_1.RealizedValue, _4_1.RealizedValue, _4_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _65 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _65(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._65>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._65> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._65> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._65>(false, default, _6_1.RemainingTokens);
}


var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _6_1)).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._65>(false, default, _5_1.RemainingTokens);
}


var node = new _65(_6_1.RealizedValue, _5_1.RealizedValue, _5_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _66 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _66(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._6_2 = _6_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._66>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._66> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._66> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._66>(false, default, _6_1.RemainingTokens);
}


var _6_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _6_1)).Realize();
if (!_6_2.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._66>(false, default, _6_2.RemainingTokens);
}


var node = new _66(_6_1.RealizedValue, _6_2.RealizedValue, _6_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _67 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _67(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._67>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._67> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._67> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._67>(false, default, _6_1.RemainingTokens);
}


var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _6_1)).Realize();
if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._67>(false, default, _7_1.RemainingTokens);
}


var node = new _67(_6_1.RealizedValue, _7_1.RealizedValue, _7_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _68 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _68(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._68>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._68> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._68> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._68>(false, default, _6_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _6_1)).Realize();
if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._68>(false, default, _8_1.RemainingTokens);
}


var node = new _68(_6_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _69 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _69(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._69>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._69> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._69> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._69>(false, default, _6_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _6_1)).Realize();
if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._69>(false, default, _9_1.RemainingTokens);
}


var node = new _69(_6_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6A : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _6A(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6A>(false, default, _6_1.RemainingTokens);
}


var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _6_1)).Realize();
if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6A>(false, default, _A_1.RemainingTokens);
}


var node = new _6A(_6_1.RealizedValue, _A_1.RealizedValue, _A_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6B : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _6B(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6B>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6B> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6B>(false, default, _6_1.RemainingTokens);
}


var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(Future.Create(() => _6_1)).Realize();
if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6B>(false, default, _B_1.RemainingTokens);
}


var node = new _6B(_6_1.RealizedValue, _B_1.RealizedValue, _B_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6C : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _6C(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6C>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6C> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6C>(false, default, _6_1.RemainingTokens);
}


var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(Future.Create(() => _6_1)).Realize();
if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6C>(false, default, _C_1.RemainingTokens);
}


var node = new _6C(_6_1.RealizedValue, _C_1.RealizedValue, _C_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6D : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _6D(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6D>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6D> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6D>(false, default, _6_1.RemainingTokens);
}


var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _6_1)).Realize();
if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6D>(false, default, _D_1.RemainingTokens);
}


var node = new _6D(_6_1.RealizedValue, _D_1.RealizedValue, _D_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6E : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _6E(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6E>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6E> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6E>(false, default, _6_1.RemainingTokens);
}


var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(Future.Create(() => _6_1)).Realize();
if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6E>(false, default, _E_1.RemainingTokens);
}


var node = new _6E(_6_1.RealizedValue, _E_1.RealizedValue, _E_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6F : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _6F(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6F>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6F> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6F>(false, default, _6_1.RemainingTokens);
}


var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(Future.Create(() => _6_1)).Realize();
if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._6F>(false, default, _F_1.RemainingTokens);
}


var node = new _6F(_6_1.RealizedValue, _F_1.RealizedValue, _F_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _70 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _70(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._70>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._70> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._70> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();
if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._70>(false, default, _7_1.RemainingTokens);
}


var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _7_1)).Realize();
if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._70>(false, default, _0_1.RemainingTokens);
}


var node = new _70(_7_1.RealizedValue, _0_1.RealizedValue, _0_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _71 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _71(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._71>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._71> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._71> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();
if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._71>(false, default, _7_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _7_1)).Realize();
if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._71>(false, default, _1_1.RemainingTokens);
}


var node = new _71(_7_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _72 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _72(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._72>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._72> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._72> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();
if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._72>(false, default, _7_1.RemainingTokens);
}


var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _7_1)).Realize();
if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._72>(false, default, _2_1.RemainingTokens);
}


var node = new _72(_7_1.RealizedValue, _2_1.RealizedValue, _2_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _73 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _73(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._73>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._73> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._73> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();
if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._73>(false, default, _7_1.RemainingTokens);
}


var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _7_1)).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._73>(false, default, _3_1.RemainingTokens);
}


var node = new _73(_7_1.RealizedValue, _3_1.RealizedValue, _3_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _74 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _74(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._74>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._74> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._74> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();
if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._74>(false, default, _7_1.RemainingTokens);
}


var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _7_1)).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._74>(false, default, _4_1.RemainingTokens);
}


var node = new _74(_7_1.RealizedValue, _4_1.RealizedValue, _4_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _75 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _75(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._75>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._75> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._75> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();
if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._75>(false, default, _7_1.RemainingTokens);
}


var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _7_1)).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._75>(false, default, _5_1.RemainingTokens);
}


var node = new _75(_7_1.RealizedValue, _5_1.RealizedValue, _5_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _76 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _76(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._76>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._76> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._76> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();
if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._76>(false, default, _7_1.RemainingTokens);
}


var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _7_1)).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._76>(false, default, _6_1.RemainingTokens);
}


var node = new _76(_7_1.RealizedValue, _6_1.RealizedValue, _6_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _77 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _77(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._7_2 = _7_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._77>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._77> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._77> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();
if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._77>(false, default, _7_1.RemainingTokens);
}


var _7_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _7_1)).Realize();
if (!_7_2.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._77>(false, default, _7_2.RemainingTokens);
}


var node = new _77(_7_1.RealizedValue, _7_2.RealizedValue, _7_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _78 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _78(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._78>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._78> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._78> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();
if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._78>(false, default, _7_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _7_1)).Realize();
if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._78>(false, default, _8_1.RemainingTokens);
}


var node = new _78(_7_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _79 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _79(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._79>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._79> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._79> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();
if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._79>(false, default, _7_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _7_1)).Realize();
if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._79>(false, default, _9_1.RemainingTokens);
}


var node = new _79(_7_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7A : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _7A(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();
if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7A>(false, default, _7_1.RemainingTokens);
}


var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _7_1)).Realize();
if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7A>(false, default, _A_1.RemainingTokens);
}


var node = new _7A(_7_1.RealizedValue, _A_1.RealizedValue, _A_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7B : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _7B(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7B>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7B> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();
if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7B>(false, default, _7_1.RemainingTokens);
}


var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(Future.Create(() => _7_1)).Realize();
if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7B>(false, default, _B_1.RemainingTokens);
}


var node = new _7B(_7_1.RealizedValue, _B_1.RealizedValue, _B_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7C : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _7C(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7C>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7C> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();
if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7C>(false, default, _7_1.RemainingTokens);
}


var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(Future.Create(() => _7_1)).Realize();
if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7C>(false, default, _C_1.RemainingTokens);
}


var node = new _7C(_7_1.RealizedValue, _C_1.RealizedValue, _C_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7D : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _7D(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7D>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7D> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();
if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7D>(false, default, _7_1.RemainingTokens);
}


var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _7_1)).Realize();
if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7D>(false, default, _D_1.RemainingTokens);
}


var node = new _7D(_7_1.RealizedValue, _D_1.RealizedValue, _D_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7E : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                private _7E(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7E>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7E> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();
if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7E>(false, default, _7_1.RemainingTokens);
}


var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(Future.Create(() => _7_1)).Realize();
if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx21ⲻ7E<TMode>.Realized._7E>(false, default, _E_1.RemainingTokens);
}


var node = new _7E(_7_1.RealizedValue, _E_1.RealizedValue, _E_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
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

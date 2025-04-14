namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx41ⲻ5A<TMode> : IAstNode<char, _Ⰳx41ⲻ5A<ParseMode.Realized>>, IFromRealizedable<_Ⰳx41ⲻ5A<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _Ⰳx41ⲻ5A()
        {
        }
        
        internal static _Ⰳx41ⲻ5A<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _Ⰳx41ⲻ5A<ParseMode.Deferred>
        {
            private Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;
                
                this.realizationResult = Future.Create(() => this.RealizeImpl());
            }
            internal Deferred(IFuture<IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>>> realizationResult)
            {
                this.realizationResult = realizationResult;
            }
            
            private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
            private IFuture<IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>>> realizationResult { get; }
            
            internal static _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(previousNodeRealizationResult);
            }
            
            public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
            {
                return this;
            }
            
            public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
            {
                return this.realizationResult.Value;
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> RealizeImpl()
            {
                if (!this.previousNodeRealizationResult.Value.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
}
var _41 = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._41.Create(this.previousNodeRealizationResult);
if (_41.Success)
{
return _41;
}
var _42 = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._42.Create(this.previousNodeRealizationResult);
if (_42.Success)
{
return _42;
}
var _43 = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._43.Create(this.previousNodeRealizationResult);
if (_43.Success)
{
return _43;
}
var _44 = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._44.Create(this.previousNodeRealizationResult);
if (_44.Success)
{
return _44;
}
var _45 = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._45.Create(this.previousNodeRealizationResult);
if (_45.Success)
{
return _45;
}
var _46 = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._46.Create(this.previousNodeRealizationResult);
if (_46.Success)
{
return _46;
}
var _47 = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._47.Create(this.previousNodeRealizationResult);
if (_47.Success)
{
return _47;
}
var _48 = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._48.Create(this.previousNodeRealizationResult);
if (_48.Success)
{
return _48;
}
var _49 = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._49.Create(this.previousNodeRealizationResult);
if (_49.Success)
{
return _49;
}
var _4A = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._4A.Create(this.previousNodeRealizationResult);
if (_4A.Success)
{
return _4A;
}
var _4B = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._4B.Create(this.previousNodeRealizationResult);
if (_4B.Success)
{
return _4B;
}
var _4C = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._4C.Create(this.previousNodeRealizationResult);
if (_4C.Success)
{
return _4C;
}
var _4D = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._4D.Create(this.previousNodeRealizationResult);
if (_4D.Success)
{
return _4D;
}
var _4E = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._4E.Create(this.previousNodeRealizationResult);
if (_4E.Success)
{
return _4E;
}
var _4F = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._4F.Create(this.previousNodeRealizationResult);
if (_4F.Success)
{
return _4F;
}
var _50 = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._50.Create(this.previousNodeRealizationResult);
if (_50.Success)
{
return _50;
}
var _51 = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._51.Create(this.previousNodeRealizationResult);
if (_51.Success)
{
return _51;
}
var _52 = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._52.Create(this.previousNodeRealizationResult);
if (_52.Success)
{
return _52;
}
var _53 = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._53.Create(this.previousNodeRealizationResult);
if (_53.Success)
{
return _53;
}
var _54 = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._54.Create(this.previousNodeRealizationResult);
if (_54.Success)
{
return _54;
}
var _55 = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._55.Create(this.previousNodeRealizationResult);
if (_55.Success)
{
return _55;
}
var _56 = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._56.Create(this.previousNodeRealizationResult);
if (_56.Success)
{
return _56;
}
var _57 = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._57.Create(this.previousNodeRealizationResult);
if (_57.Success)
{
return _57;
}
var _58 = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._58.Create(this.previousNodeRealizationResult);
if (_58.Success)
{
return _58;
}
var _59 = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._59.Create(this.previousNodeRealizationResult);
if (_59.Success)
{
return _59;
}
var _5A = _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._5A.Create(this.previousNodeRealizationResult);
if (_5A.Success)
{
return _5A;
}

return new RealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);

            }
        }
        
        public abstract class Realized : _Ⰳx41ⲻ5A<ParseMode.Realized>
        {
            private Realized()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(_Ⰳx41ⲻ5A<TMode>.Realized.Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_Ⰳx41ⲻ5A<TMode>.Realized node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                public TResult Visit(_Ⰳx41ⲻ5A<ParseMode.Realized> node, TContext context)
                {
                    //// TODO is there a way to avoid this cast?
return (node as _Ⰳx41ⲻ5A<TMode>.Realized)!.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._41 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._42 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._43 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._44 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._45 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._46 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._47 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._48 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._49 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._4A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._4B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._4C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._4D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._4E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._4F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._50 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._51 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._52 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._53 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._54 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._55 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._56 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._57 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._58 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._59 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._5A node, TContext context);
            }
            
            public sealed class _41 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _41(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._41>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._41> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._41> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._41>(false, default, _4_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _4_1)).Realize();
if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._41>(false, default, _1_1.RemainingTokens);
}


var node = new _41(_4_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _42 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _42(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._42>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._42> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._42> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._42>(false, default, _4_1.RemainingTokens);
}


var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _4_1)).Realize();
if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._42>(false, default, _2_1.RemainingTokens);
}


var node = new _42(_4_1.RealizedValue, _2_1.RealizedValue, _2_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _43 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _43(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._43>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._43> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._43> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._43>(false, default, _4_1.RemainingTokens);
}


var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _4_1)).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._43>(false, default, _3_1.RemainingTokens);
}


var node = new _43(_4_1.RealizedValue, _3_1.RealizedValue, _3_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _44 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _44(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._4_2 = _4_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._44>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._44> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._44> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._44>(false, default, _4_1.RemainingTokens);
}


var _4_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _4_1)).Realize();
if (!_4_2.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._44>(false, default, _4_2.RemainingTokens);
}


var node = new _44(_4_1.RealizedValue, _4_2.RealizedValue, _4_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _45 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _45(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._45>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._45> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._45> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._45>(false, default, _4_1.RemainingTokens);
}


var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _4_1)).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._45>(false, default, _5_1.RemainingTokens);
}


var node = new _45(_4_1.RealizedValue, _5_1.RealizedValue, _5_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _46 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _46(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._46>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._46> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._46> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._46>(false, default, _4_1.RemainingTokens);
}


var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _4_1)).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._46>(false, default, _6_1.RemainingTokens);
}


var node = new _46(_4_1.RealizedValue, _6_1.RealizedValue, _6_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _47 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _47(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._47>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._47> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._47> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._47>(false, default, _4_1.RemainingTokens);
}


var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _4_1)).Realize();
if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._47>(false, default, _7_1.RemainingTokens);
}


var node = new _47(_4_1.RealizedValue, _7_1.RealizedValue, _7_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _48 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _48(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._48>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._48> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._48> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._48>(false, default, _4_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _4_1)).Realize();
if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._48>(false, default, _8_1.RemainingTokens);
}


var node = new _48(_4_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _49 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _49(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._49>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._49> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._49> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._49>(false, default, _4_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _4_1)).Realize();
if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._49>(false, default, _9_1.RemainingTokens);
}


var node = new _49(_4_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4A : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _4A(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4A>(false, default, _4_1.RemainingTokens);
}


var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _4_1)).Realize();
if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4A>(false, default, _A_1.RemainingTokens);
}


var node = new _4A(_4_1.RealizedValue, _A_1.RealizedValue, _A_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4B : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _4B(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4B>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4B> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4B>(false, default, _4_1.RemainingTokens);
}


var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(Future.Create(() => _4_1)).Realize();
if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4B>(false, default, _B_1.RemainingTokens);
}


var node = new _4B(_4_1.RealizedValue, _B_1.RealizedValue, _B_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4C : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _4C(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4C>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4C> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4C>(false, default, _4_1.RemainingTokens);
}


var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(Future.Create(() => _4_1)).Realize();
if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4C>(false, default, _C_1.RemainingTokens);
}


var node = new _4C(_4_1.RealizedValue, _C_1.RealizedValue, _C_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4D : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _4D(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4D>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4D> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4D>(false, default, _4_1.RemainingTokens);
}


var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _4_1)).Realize();
if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4D>(false, default, _D_1.RemainingTokens);
}


var node = new _4D(_4_1.RealizedValue, _D_1.RealizedValue, _D_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4E : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _4E(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4E>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4E> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4E>(false, default, _4_1.RemainingTokens);
}


var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(Future.Create(() => _4_1)).Realize();
if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4E>(false, default, _E_1.RemainingTokens);
}


var node = new _4E(_4_1.RealizedValue, _E_1.RealizedValue, _E_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4F : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _4F(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4F>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4F> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4F>(false, default, _4_1.RemainingTokens);
}


var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(Future.Create(() => _4_1)).Realize();
if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4F>(false, default, _F_1.RemainingTokens);
}


var node = new _4F(_4_1.RealizedValue, _F_1.RealizedValue, _F_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _50 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _50(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._50>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._50> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._50> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._50>(false, default, _5_1.RemainingTokens);
}


var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _5_1)).Realize();
if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._50>(false, default, _0_1.RemainingTokens);
}


var node = new _50(_5_1.RealizedValue, _0_1.RealizedValue, _0_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _51 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _51(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._51>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._51> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._51> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._51>(false, default, _5_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _5_1)).Realize();
if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._51>(false, default, _1_1.RemainingTokens);
}


var node = new _51(_5_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _52 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _52(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._52>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._52> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._52> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._52>(false, default, _5_1.RemainingTokens);
}


var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _5_1)).Realize();
if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._52>(false, default, _2_1.RemainingTokens);
}


var node = new _52(_5_1.RealizedValue, _2_1.RealizedValue, _2_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _53 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _53(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._53>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._53> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._53> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._53>(false, default, _5_1.RemainingTokens);
}


var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _5_1)).Realize();
if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._53>(false, default, _3_1.RemainingTokens);
}


var node = new _53(_5_1.RealizedValue, _3_1.RealizedValue, _3_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _54 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _54(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._54>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._54> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._54> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._54>(false, default, _5_1.RemainingTokens);
}


var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _5_1)).Realize();
if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._54>(false, default, _4_1.RemainingTokens);
}


var node = new _54(_5_1.RealizedValue, _4_1.RealizedValue, _4_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _55 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _55(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._5_2 = _5_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._55>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._55> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._55> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._55>(false, default, _5_1.RemainingTokens);
}


var _5_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _5_1)).Realize();
if (!_5_2.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._55>(false, default, _5_2.RemainingTokens);
}


var node = new _55(_5_1.RealizedValue, _5_2.RealizedValue, _5_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _56 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _56(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._56>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._56> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._56> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._56>(false, default, _5_1.RemainingTokens);
}


var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _5_1)).Realize();
if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._56>(false, default, _6_1.RemainingTokens);
}


var node = new _56(_5_1.RealizedValue, _6_1.RealizedValue, _6_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _57 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _57(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._57>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._57> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._57> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._57>(false, default, _5_1.RemainingTokens);
}


var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _5_1)).Realize();
if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._57>(false, default, _7_1.RemainingTokens);
}


var node = new _57(_5_1.RealizedValue, _7_1.RealizedValue, _7_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _58 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _58(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._58>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._58> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._58> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._58>(false, default, _5_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _5_1)).Realize();
if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._58>(false, default, _8_1.RemainingTokens);
}


var node = new _58(_5_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _59 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _59(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._59>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._59> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._59> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._59>(false, default, _5_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _5_1)).Realize();
if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._59>(false, default, _9_1.RemainingTokens);
}


var node = new _59(_5_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5A : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _5A(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._5A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._5A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._5A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();
if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._5A>(false, default, _5_1.RemainingTokens);
}


var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _5_1)).Realize();
if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._5A>(false, default, _A_1.RemainingTokens);
}


var node = new _5A(_5_1.RealizedValue, _A_1.RealizedValue, _A_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
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

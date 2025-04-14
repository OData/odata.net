namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _BIT<TMode> : IAstNode<char, _BIT<ParseMode.Realized>>, IFromRealizedable<_BIT<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _BIT()
        {
        }
        
        internal static _BIT<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _BIT<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _BIT<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _BIT<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _BIT<ParseMode.Deferred>
        {
            private Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;
                
                this.realizationResult = Future.Create(() => this.RealizeImpl());
            }
            internal Deferred(IFuture<IRealizationResult<char, _BIT<ParseMode.Realized>>> realizationResult)
            {
                this.realizationResult = realizationResult;
            }
            
            private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
            private IFuture<IRealizationResult<char, _BIT<ParseMode.Realized>>> realizationResult { get; }
            
            internal static _BIT<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new _BIT<ParseMode.Deferred>.Deferred(previousNodeRealizationResult);
            }
            
            public override _BIT<ParseMode.Deferred> Convert()
            {
                return this;
            }
            
            public override IRealizationResult<char, _BIT<ParseMode.Realized>> Realize()
            {
                return this.realizationResult.Value;
            }
            
            private IRealizationResult<char, _BIT<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
        
        public abstract class Realized : _BIT<ParseMode.Realized>
        {
            private Realized()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(_BIT<TMode>.Realized.Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_BIT<TMode>.Realized node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                public TResult Visit(_BIT<ParseMode.Realized> node, TContext context)
                {
                    //// TODO is there a way to avoid this cast?
return (node as _BIT<TMode>.Realized)!.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_BIT<TMode>.Realized._ʺx30ʺ node, TContext context);
                protected internal abstract TResult Accept(_BIT<TMode>.Realized._ʺx31ʺ node, TContext context);
            }
            
            public sealed class _ʺx30ʺ : _BIT<TMode>.Realized
            {
                private _ʺx30ʺ(__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx30ʺ<ParseMode.Realized> _ʺx30ʺ_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._ʺx30ʺ_1 = _ʺx30ʺ_1;
                    this.realizationResult = new RealizationResult<char, _BIT<TMode>.Realized._ʺx30ʺ>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _BIT<TMode>.Realized._ʺx30ʺ> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx30ʺ<ParseMode.Realized> _ʺx30ʺ_1 { get; }
                
                internal static IRealizationResult<char, _BIT<TMode>.Realized._ʺx30ʺ> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _ʺx30ʺ_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx30ʺ.Create(previousNodeRealizationResult).Realize();
if (!_ʺx30ʺ_1.Success)
{
return new RealizationResult<char, _BIT<TMode>.Realized._ʺx30ʺ>(false, default, _ʺx30ʺ_1.RemainingTokens);
}


var node = new _ʺx30ʺ(_ʺx30ʺ_1.RealizedValue, _ʺx30ʺ_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _BIT<ParseMode.Deferred> Convert()
                {
                    return new _BIT<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _BIT<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ʺx31ʺ : _BIT<TMode>.Realized
            {
                private _ʺx31ʺ(__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx31ʺ<ParseMode.Realized> _ʺx31ʺ_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._ʺx31ʺ_1 = _ʺx31ʺ_1;
                    this.realizationResult = new RealizationResult<char, _BIT<TMode>.Realized._ʺx31ʺ>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _BIT<TMode>.Realized._ʺx31ʺ> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx31ʺ<ParseMode.Realized> _ʺx31ʺ_1 { get; }
                
                internal static IRealizationResult<char, _BIT<TMode>.Realized._ʺx31ʺ> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _ʺx31ʺ_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx31ʺ.Create(previousNodeRealizationResult).Realize();
if (!_ʺx31ʺ_1.Success)
{
return new RealizationResult<char, _BIT<TMode>.Realized._ʺx31ʺ>(false, default, _ʺx31ʺ_1.RemainingTokens);
}


var node = new _ʺx31ʺ(_ʺx31ʺ_1.RealizedValue, _ʺx31ʺ_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _BIT<ParseMode.Deferred> Convert()
                {
                    return new _BIT<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _BIT<ParseMode.Realized>> Realize()
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

namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _alphaNumeric<TMode> : IAstNode<char, _alphaNumeric<ParseMode.Realized>>, IFromRealizedable<_alphaNumeric<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _alphaNumeric()
        {
        }
        
        internal static _alphaNumeric<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _alphaNumeric<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _alphaNumeric<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _alphaNumeric<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _alphaNumeric<ParseMode.Deferred>
        {
            private Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;
                
                this.realizationResult = Future.Create(() => this.RealizeImpl());
            }
            internal Deferred(IFuture<IRealizationResult<char, _alphaNumeric<ParseMode.Realized>>> realizationResult)
            {
                this.realizationResult = realizationResult;
            }
            
            private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
            private IFuture<IRealizationResult<char, _alphaNumeric<ParseMode.Realized>>> realizationResult { get; }
            
            internal static _alphaNumeric<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new _alphaNumeric<ParseMode.Deferred>.Deferred(previousNodeRealizationResult);
            }
            
            public override _alphaNumeric<ParseMode.Deferred> Convert()
            {
                return this;
            }
            
            public override IRealizationResult<char, _alphaNumeric<ParseMode.Realized>> Realize()
            {
                return this.realizationResult.Value;
            }
            
            private IRealizationResult<char, _alphaNumeric<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
        
        public abstract class Realized : _alphaNumeric<ParseMode.Realized>
        {
            private Realized()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(_alphaNumeric<TMode>.Realized.Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_alphaNumeric<TMode>.Realized node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                public TResult Visit(_alphaNumeric<ParseMode.Realized> node, TContext context)
                {
                    //// TODO is there a way to avoid this cast?
return (node as _alphaNumeric<TMode>.Realized)!.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_alphaNumeric<TMode>.Realized._ʺx41ʺ node, TContext context);
                protected internal abstract TResult Accept(_alphaNumeric<TMode>.Realized._ʺx43ʺ node, TContext context);
            }
            
            public sealed class _ʺx41ʺ : _alphaNumeric<TMode>.Realized
            {
                private _ʺx41ʺ(__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx41ʺ<ParseMode.Realized> _ʺx41ʺ_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._ʺx41ʺ_1 = _ʺx41ʺ_1;
                    this.realizationResult = new RealizationResult<char, _alphaNumeric<TMode>.Realized._ʺx41ʺ>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _alphaNumeric<TMode>.Realized._ʺx41ʺ> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx41ʺ<ParseMode.Realized> _ʺx41ʺ_1 { get; }
                
                internal static IRealizationResult<char, _alphaNumeric<TMode>.Realized._ʺx41ʺ> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _ʺx41ʺ_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx41ʺ.Create(previousNodeRealizationResult).Realize();
var node = new _ʺx41ʺ(_ʺx41ʺ_1.RealizedValue, _ʺx41ʺ_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _alphaNumeric<ParseMode.Deferred> Convert()
                {
                    return new _alphaNumeric<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _alphaNumeric<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ʺx43ʺ : _alphaNumeric<TMode>.Realized
            {
                private _ʺx43ʺ(__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx43ʺ<ParseMode.Realized> _ʺx43ʺ_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._ʺx43ʺ_1 = _ʺx43ʺ_1;
                    this.realizationResult = new RealizationResult<char, _alphaNumeric<TMode>.Realized._ʺx43ʺ>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _alphaNumeric<TMode>.Realized._ʺx43ʺ> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx43ʺ<ParseMode.Realized> _ʺx43ʺ_1 { get; }
                
                internal static IRealizationResult<char, _alphaNumeric<TMode>.Realized._ʺx43ʺ> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _ʺx43ʺ_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx43ʺ.Create(previousNodeRealizationResult).Realize();
var node = new _ʺx43ʺ(_ʺx43ʺ_1.RealizedValue, _ʺx43ʺ_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _alphaNumeric<ParseMode.Deferred> Convert()
                {
                    return new _alphaNumeric<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _alphaNumeric<ParseMode.Realized>> Realize()
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

namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _CTL<TMode> : IAstNode<char, _CTL<ParseMode.Realized>>, IFromRealizedable<_CTL<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _CTL()
        {
        }
        
        internal static _CTL<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _CTL<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _CTL<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _CTL<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _CTL<ParseMode.Deferred>
        {
            private Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;
                
                this.realizationResult = Future.Create(() => this.RealizeImpl());
            }
            internal Deferred(IFuture<IRealizationResult<char, _CTL<ParseMode.Realized>>> realizationResult)
            {
                this.realizationResult = realizationResult;
            }
            
            private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
            private IFuture<IRealizationResult<char, _CTL<ParseMode.Realized>>> realizationResult { get; }
            
            internal static _CTL<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new _CTL<ParseMode.Deferred>.Deferred(previousNodeRealizationResult);
            }
            
            public override _CTL<ParseMode.Deferred> Convert()
            {
                return this;
            }
            
            public override IRealizationResult<char, _CTL<ParseMode.Realized>> Realize()
            {
                return this.realizationResult.Value;
            }
            
            private IRealizationResult<char, _CTL<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
        
        public abstract class Realized : _CTL<ParseMode.Realized>
        {
            private Realized()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(_CTL<TMode>.Realized.Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_CTL<TMode>.Realized node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                public TResult Visit(_CTL<ParseMode.Realized> node, TContext context)
                {
                    //// TODO is there a way to avoid this cast?
return (node as _CTL<TMode>.Realized)!.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_CTL<TMode>.Realized._Ⰳx00ⲻ1F node, TContext context);
                protected internal abstract TResult Accept(_CTL<TMode>.Realized._Ⰳx7F node, TContext context);
            }
            
            public sealed class _Ⰳx00ⲻ1F : _CTL<TMode>.Realized
            {
                private _Ⰳx00ⲻ1F(__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx00ⲻ1F<ParseMode.Realized> _Ⰳx00ⲻ1F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._Ⰳx00ⲻ1F_1 = _Ⰳx00ⲻ1F_1;
                    this.realizationResult = new RealizationResult<char, _CTL<TMode>.Realized._Ⰳx00ⲻ1F>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _CTL<TMode>.Realized._Ⰳx00ⲻ1F> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx00ⲻ1F<ParseMode.Realized> _Ⰳx00ⲻ1F_1 { get; }
                
                internal static IRealizationResult<char, _CTL<TMode>.Realized._Ⰳx00ⲻ1F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _Ⰳx00ⲻ1F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx00ⲻ1F.Create(previousNodeRealizationResult).Realize();
var node = new _Ⰳx00ⲻ1F(_Ⰳx00ⲻ1F_1.RealizedValue, _Ⰳx00ⲻ1F_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _CTL<ParseMode.Deferred> Convert()
                {
                    return new _CTL<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _CTL<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _Ⰳx7F : _CTL<TMode>.Realized
            {
                private _Ⰳx7F(__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx7F<ParseMode.Realized> _Ⰳx7F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._Ⰳx7F_1 = _Ⰳx7F_1;
                    this.realizationResult = new RealizationResult<char, _CTL<TMode>.Realized._Ⰳx7F>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _CTL<TMode>.Realized._Ⰳx7F> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx7F<ParseMode.Realized> _Ⰳx7F_1 { get; }
                
                internal static IRealizationResult<char, _CTL<TMode>.Realized._Ⰳx7F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _Ⰳx7F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx7F.Create(previousNodeRealizationResult).Realize();
var node = new _Ⰳx7F(_Ⰳx7F_1.RealizedValue, _Ⰳx7F_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _CTL<ParseMode.Deferred> Convert()
                {
                    return new _CTL<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _CTL<ParseMode.Realized>> Realize()
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

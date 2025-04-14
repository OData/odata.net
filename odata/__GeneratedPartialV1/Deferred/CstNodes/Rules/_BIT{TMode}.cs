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
                private _ʺx30ʺ(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx30ʺ<TMode>> _ʺx30ʺ_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this.realizationResult = new RealizationResult<char, _BIT<TMode>.Realized._ʺx30ʺ>(true, this, nextTokens);
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx30ʺ<TMode>> __ʺx30ʺ_1 { get; }
                private IRealizationResult<char, _BIT<TMode>.Realized._ʺx30ʺ> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx30ʺ<TMode> _ʺx30ʺ_1 { get; }
                
                public override _BIT<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _BIT<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _ʺx31ʺ : _BIT<TMode>.Realized
            {
                private _ʺx31ʺ(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx31ʺ<TMode>> _ʺx31ʺ_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this.realizationResult = new RealizationResult<char, _BIT<TMode>.Realized._ʺx31ʺ>(true, this, nextTokens);
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx31ʺ<TMode>> __ʺx31ʺ_1 { get; }
                private IRealizationResult<char, _BIT<TMode>.Realized._ʺx31ʺ> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx31ʺ<TMode> _ʺx31ʺ_1 { get; }
                
                public override _BIT<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _BIT<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
        }
    }
    
}

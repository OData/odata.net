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
                public TResult Visit(_Ⰳx30ⲻ39<ParseMode.Realized> node, TContext context)
                {
                    //// TODO is there a way to avoid this cast?
return (node as _Ⰳx30ⲻ39<TMode>.Realized)!.Dispatch(this, context);
                }
                
                public TResult Visit(_Ⰳx30ⲻ39<TMode>.Realized node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
            }
        }
    }
    
}

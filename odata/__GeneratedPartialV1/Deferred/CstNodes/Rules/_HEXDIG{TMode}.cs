namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _HEXDIG<TMode> : IAstNode<char, _HEXDIG<ParseMode.Realized>>, IFromRealizedable<_HEXDIG<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _HEXDIG()
        {
        }
        
        internal static _HEXDIG<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _HEXDIG<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _HEXDIG<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _HEXDIG<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _HEXDIG<ParseMode.Deferred>
        {
            private Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;
                
                this.realizationResult = Future.Create(() => this.RealizeImpl());
            }
            internal Deferred(IFuture<IRealizationResult<char, _HEXDIG<ParseMode.Realized>>> realizationResult)
            {
                this.realizationResult = realizationResult;
            }
            
            private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
            private IFuture<IRealizationResult<char, _HEXDIG<ParseMode.Realized>>> realizationResult { get; }
            
            internal static _HEXDIG<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new _HEXDIG<ParseMode.Deferred>.Deferred(previousNodeRealizationResult);
            }
            
            public override _HEXDIG<ParseMode.Deferred> Convert()
            {
                return this;
            }
            
            public override IRealizationResult<char, _HEXDIG<ParseMode.Realized>> Realize()
            {
                return this.realizationResult.Value;
            }
            
            private IRealizationResult<char, _HEXDIG<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
        
        public abstract class Realized : _HEXDIG<ParseMode.Realized>
        {
            private Realized()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(_HEXDIG<TMode>.Realized.Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_HEXDIG<ParseMode.Realized> node, TContext context)
                {
                    //// TODO is there a way to avoid this cast?
return (node as _HEXDIG<TMode>.Realized)!.Dispatch(this, context);
                }
                
                public TResult Visit(_HEXDIG<TMode>.Realized node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
            }
        }
    }
    
}

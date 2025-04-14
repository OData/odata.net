namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _ALPHA<TMode> : IAstNode<char, _ALPHA<ParseMode.Realized>>, IFromRealizedable<_ALPHA<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _ALPHA()
        {
        }
        
        internal static _ALPHA<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _ALPHA<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _ALPHA<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _ALPHA<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _ALPHA<ParseMode.Deferred>
        {
            private Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;
                
                this.realizationResult = Future.Create(() => this.RealizeImpl());
            }
            internal Deferred(IFuture<IRealizationResult<char, _ALPHA<ParseMode.Realized>>> realizationResult)
            {
                this.realizationResult = realizationResult;
            }
            
            private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
            private IFuture<IRealizationResult<char, _ALPHA<ParseMode.Realized>>> realizationResult { get; }
            
            internal static _ALPHA<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new _ALPHA<ParseMode.Deferred>.Deferred(previousNodeRealizationResult);
            }
            
            public override _ALPHA<ParseMode.Deferred> Convert()
            {
                return this;
            }
            
            public override IRealizationResult<char, _ALPHA<ParseMode.Realized>> Realize()
            {
                return this.realizationResult.Value;
            }
            
            private IRealizationResult<char, _ALPHA<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
    }
    
}

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
                throw new Exception("TODO");
            }
            
            private IRealizationResult<char, _CTL<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
    }
    
}

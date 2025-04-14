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
                throw new Exception("TODO");
            }
            
            private IRealizationResult<char, _BIT<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
    }
    
}

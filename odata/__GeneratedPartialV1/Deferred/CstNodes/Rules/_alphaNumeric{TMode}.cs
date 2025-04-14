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
                throw new Exception("TODO");
            }
            
            public override IRealizationResult<char, _alphaNumeric<ParseMode.Realized>> Realize()
            {
                throw new Exception("TODO");
            }
            
            private IRealizationResult<char, _alphaNumeric<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
    }
    
}

namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx01ⲻ7F<TMode> : IAstNode<char, _Ⰳx01ⲻ7F<ParseMode.Realized>>, IFromRealizedable<_Ⰳx01ⲻ7F<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _Ⰳx01ⲻ7F()
        {
        }
        
        internal static _Ⰳx01ⲻ7F<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _Ⰳx01ⲻ7F<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _Ⰳx01ⲻ7F<ParseMode.Deferred>
        {
            private Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;
                
                this.realizationResult = Future.Create(() => this.RealizeImpl());
            }
            internal Deferred(IFuture<IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>>> realizationResult)
            {
                this.realizationResult = realizationResult;
            }
            
            private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
            private IFuture<IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>>> realizationResult { get; }
            
            internal static _Ⰳx01ⲻ7F<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new _Ⰳx01ⲻ7F<ParseMode.Deferred>.Deferred(previousNodeRealizationResult);
            }
            
            public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
            {
                return this;
            }
            
            public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
            {
                throw new Exception("TODO");
            }
            
            private IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
    }
    
}

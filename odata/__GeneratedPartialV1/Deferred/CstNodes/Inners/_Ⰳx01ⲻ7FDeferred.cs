namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _Ⰳx01ⲻ7FDeferred : IAstNode<char, _Ⰳx01ⲻ7FRealized>
    {
        public _Ⰳx01ⲻ7FDeferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            //// TODO get constructor accessibility correct
            this.previousNodeRealizationResult = previousNodeRealizationResult;
            
            this.realizationResult = Future.Create(() => this.RealizeImpl());
        }
        public _Ⰳx01ⲻ7FDeferred(IFuture<IRealizationResult<char, _Ⰳx01ⲻ7FRealized>> realizationResult)
        {
            this.realizationResult = realizationResult;
        }
        
        private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
        private IFuture<IRealizationResult<char, _Ⰳx01ⲻ7FRealized>> realizationResult { get; }
        
        public IRealizationResult<char, _Ⰳx01ⲻ7FRealized> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _Ⰳx01ⲻ7FRealized> RealizeImpl()
        {
            if (!this.previousNodeRealizationResult.Value.Success)
{
    return new RealizationResult<char, _Ⰳx01ⲻ7FRealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
}

//// TODO
throw new Exception("TODO");
        }
    }
    
}

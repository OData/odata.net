namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _Ⰳx41ⲻ5ADeferred : IAstNode<char, _Ⰳx41ⲻ5ARealized>
    {
        public _Ⰳx41ⲻ5ADeferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            //// TODO get constructor accessibility correct
            this.previousNodeRealizationResult = previousNodeRealizationResult;
            
            this.realizationResult = Future.Create(() => this.RealizeImpl());
        }
        public _Ⰳx41ⲻ5ADeferred(IFuture<IRealizationResult<char, _Ⰳx41ⲻ5ARealized>> realizationResult)
        {
            this.realizationResult = realizationResult;
        }
        
        private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
        private IFuture<IRealizationResult<char, _Ⰳx41ⲻ5ARealized>> realizationResult { get; }
        
        public IRealizationResult<char, _Ⰳx41ⲻ5ARealized> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _Ⰳx41ⲻ5ARealized> RealizeImpl()
        {
            if (!this.previousNodeRealizationResult.Value.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
}

//// TODO
throw new Exception("TODO");
        }
    }
    
}

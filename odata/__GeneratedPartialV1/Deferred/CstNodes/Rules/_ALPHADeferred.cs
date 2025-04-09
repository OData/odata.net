namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _ALPHADeferred : IAstNode<char, _ALPHARealized>
    {
        public _ALPHADeferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            //// TODO get constructor accessibility correct
            this.previousNodeRealizationResult = previousNodeRealizationResult;
            
            this.realizationResult = Future.Create(() => this.RealizeImpl());
        }
        public _ALPHADeferred(IFuture<IRealizationResult<char, _ALPHARealized>> realizationResult)
        {
            this.realizationResult = realizationResult;
        }
        
        private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
        private IFuture<IRealizationResult<char, _ALPHARealized>> realizationResult { get; }
        
        public IRealizationResult<char, _ALPHARealized> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _ALPHARealized> RealizeImpl()
        {
            if (!this.previousNodeRealizationResult.Value.Success)
{
    return new RealizationResult<char, _ALPHARealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
}

//// TODO
throw new Exception("TODO");
        }
    }
    
}

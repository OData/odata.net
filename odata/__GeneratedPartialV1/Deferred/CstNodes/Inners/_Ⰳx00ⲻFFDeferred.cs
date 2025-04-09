namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _Ⰳx00ⲻFFDeferred : IAstNode<char, _Ⰳx00ⲻFFRealized>
    {
        public _Ⰳx00ⲻFFDeferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            //// TODO get constructor accessibility correct
            this.previousNodeRealizationResult = previousNodeRealizationResult;
            
            this.realizationResult = Future.Create(() => this.RealizeImpl());
        }
        public _Ⰳx00ⲻFFDeferred(IFuture<IRealizationResult<char, _Ⰳx00ⲻFFRealized>> realizationResult)
        {
            this.realizationResult = realizationResult;
        }
        
        private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
        private IFuture<IRealizationResult<char, _Ⰳx00ⲻFFRealized>> realizationResult { get; }
        
        public IRealizationResult<char, _Ⰳx00ⲻFFRealized> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _Ⰳx00ⲻFFRealized> RealizeImpl()
        {
            if (!this.previousNodeRealizationResult.Value.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻFFRealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
}

//// TODO
throw new Exception("TODO");
        }
    }
    
}

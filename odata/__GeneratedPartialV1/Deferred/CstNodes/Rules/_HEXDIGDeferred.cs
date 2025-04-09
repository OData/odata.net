namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _HEXDIGDeferred : IAstNode<char, _HEXDIGRealized>
    {
        public _HEXDIGDeferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            //// TODO get constructor accessibility correct
            this.previousNodeRealizationResult = previousNodeRealizationResult;
            
            this.realizationResult = Future.Create(() => this.RealizeImpl());
        }
        public _HEXDIGDeferred(IFuture<IRealizationResult<char, _HEXDIGRealized>> realizationResult)
        {
            this.realizationResult = realizationResult;
        }
        
        private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
        private IFuture<IRealizationResult<char, _HEXDIGRealized>> realizationResult { get; }
        
        public IRealizationResult<char, _HEXDIGRealized> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _HEXDIGRealized> RealizeImpl()
        {
            if (!this.previousNodeRealizationResult.Value.Success)
{
    return new RealizationResult<char, _HEXDIGRealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
}

//// TODO
throw new Exception("TODO");
        }
    }
    
}

namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _alphaNumericDeferred : IAstNode<char, _alphaNumericRealized>
    {
        public _alphaNumericDeferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            //// TODO get constructor accessibility correct
            this.previousNodeRealizationResult = previousNodeRealizationResult;
            
            this.realizationResult = Future.Create(() => this.RealizeImpl());
        }
        public _alphaNumericDeferred(IFuture<IRealizationResult<char, _alphaNumericRealized>> realizationResult)
        {
            this.realizationResult = realizationResult;
        }
        
        private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
        private IFuture<IRealizationResult<char, _alphaNumericRealized>> realizationResult { get; }
        
        public IRealizationResult<char, _alphaNumericRealized> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _alphaNumericRealized> RealizeImpl()
        {
            if (!this.previousNodeRealizationResult.Value.Success)
{
    return new RealizationResult<char, _alphaNumericRealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
}

//// TODO
throw new Exception("TODO");
        }
    }
    
}

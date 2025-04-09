namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _BITDeferred : IAstNode<char, _BITRealized>
    {
        public _BITDeferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            //// TODO get constructor accessibility correct
            this.previousNodeRealizationResult = previousNodeRealizationResult;
            
            this.realizationResult = Future.Create(() => this.RealizeImpl());
        }
        public _BITDeferred(IFuture<IRealizationResult<char, _BITRealized>> realizationResult)
        {
            this.realizationResult = realizationResult;
        }
        
        private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
        private IFuture<IRealizationResult<char, _BITRealized>> realizationResult { get; }
        
        public IRealizationResult<char, _BITRealized> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _BITRealized> RealizeImpl()
        {
            if (!this.previousNodeRealizationResult.Value.Success)
{
    return new RealizationResult<char, _BITRealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
}
var _ʺx30ʺ = _BITRealized._ʺx30ʺ.Create(this.previousNodeRealizationResult);
if (_ʺx30ʺ.Success)
{
    return _ʺx30ʺ;
}

var _ʺx31ʺ = _BITRealized._ʺx31ʺ.Create(this.previousNodeRealizationResult);
if (_ʺx31ʺ.Success)
{
    return _ʺx31ʺ;
}
return new RealizationResult<char, _BITRealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
        }
    }
    
}

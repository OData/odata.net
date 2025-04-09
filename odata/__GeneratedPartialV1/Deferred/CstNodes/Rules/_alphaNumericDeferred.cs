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
var _ʺx41ʺ = _alphaNumericRealized._ʺx41ʺ.Create(this.previousNodeRealizationResult);
if (_ʺx41ʺ.Success)
{
    return _ʺx41ʺ;
}

var _ʺx43ʺ = _alphaNumericRealized._ʺx43ʺ.Create(this.previousNodeRealizationResult);
if (_ʺx43ʺ.Success)
{
    return _ʺx43ʺ;
}
return new RealizationResult<char, _alphaNumericRealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
        }
    }
    
}

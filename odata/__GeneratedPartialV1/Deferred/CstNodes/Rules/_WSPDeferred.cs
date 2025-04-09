namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _WSPDeferred : IAstNode<char, _WSPRealized>
    {
        public _WSPDeferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            //// TODO get constructor accessibility correct
            this.previousNodeRealizationResult = previousNodeRealizationResult;
            
            this.realizationResult = Future.Create(() => this.RealizeImpl());
        }
        public _WSPDeferred(IFuture<IRealizationResult<char, _WSPRealized>> realizationResult)
        {
            this.realizationResult = realizationResult;
        }
        
        private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
        private IFuture<IRealizationResult<char, _WSPRealized>> realizationResult { get; }
        
        public IRealizationResult<char, _WSPRealized> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _WSPRealized> RealizeImpl()
        {
            if (!this.previousNodeRealizationResult.Value.Success)
{
    return new RealizationResult<char, _WSPRealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
}
var _SP = _WSPRealized._SP.Create(this.previousNodeRealizationResult);
if (_SP.Success)
{
    return _SP;
}

var _HTAB = _WSPRealized._HTAB.Create(this.previousNodeRealizationResult);
if (_HTAB.Success)
{
    return _HTAB;
}
return new RealizationResult<char, _WSPRealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
        }
    }
    
}

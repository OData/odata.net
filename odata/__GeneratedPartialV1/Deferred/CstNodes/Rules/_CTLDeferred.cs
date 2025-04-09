namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _CTLDeferred : IAstNode<char, _CTLRealized>
    {
        public _CTLDeferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            //// TODO get constructor accessibility correct
            this.previousNodeRealizationResult = previousNodeRealizationResult;
            
            this.realizationResult = Future.Create(() => this.RealizeImpl());
        }
        public _CTLDeferred(IFuture<IRealizationResult<char, _CTLRealized>> realizationResult)
        {
            this.realizationResult = realizationResult;
        }
        
        private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
        private IFuture<IRealizationResult<char, _CTLRealized>> realizationResult { get; }
        
        public IRealizationResult<char, _CTLRealized> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _CTLRealized> RealizeImpl()
        {
            if (!this.previousNodeRealizationResult.Value.Success)
{
    return new RealizationResult<char, _CTLRealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
}
var _Ⰳx00ⲻ1F = _CTLRealized._Ⰳx00ⲻ1F.Create(this.previousNodeRealizationResult);
if (_Ⰳx00ⲻ1F.Success)
{
    return _Ⰳx00ⲻ1F;
}

var _Ⰳx7F = _CTLRealized._Ⰳx7F.Create(this.previousNodeRealizationResult);
if (_Ⰳx7F.Success)
{
    return _Ⰳx7F;
}
return new RealizationResult<char, _CTLRealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
        }
    }
    
}

namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _Ⰳx30ⲻ39Deferred : IAstNode<char, _Ⰳx30ⲻ39Realized>
    {
        public _Ⰳx30ⲻ39Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            //// TODO get constructor accessibility correct
            this.previousNodeRealizationResult = previousNodeRealizationResult;
            
            this.realizationResult = Future.Create(() => this.RealizeImpl());
        }
        public _Ⰳx30ⲻ39Deferred(IFuture<IRealizationResult<char, _Ⰳx30ⲻ39Realized>> realizationResult)
        {
            this.realizationResult = realizationResult;
        }
        
        private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
        private IFuture<IRealizationResult<char, _Ⰳx30ⲻ39Realized>> realizationResult { get; }
        
        public IRealizationResult<char, _Ⰳx30ⲻ39Realized> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _Ⰳx30ⲻ39Realized> RealizeImpl()
        {
            if (!this.previousNodeRealizationResult.Value.Success)
{
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
}
var _30 = _Ⰳx30ⲻ39Realized._30.Create(this.previousNodeRealizationResult);
if (_30.Success)
{
    return _30;
}

var _31 = _Ⰳx30ⲻ39Realized._31.Create(this.previousNodeRealizationResult);
if (_31.Success)
{
    return _31;
}

var _32 = _Ⰳx30ⲻ39Realized._32.Create(this.previousNodeRealizationResult);
if (_32.Success)
{
    return _32;
}

var _33 = _Ⰳx30ⲻ39Realized._33.Create(this.previousNodeRealizationResult);
if (_33.Success)
{
    return _33;
}

var _34 = _Ⰳx30ⲻ39Realized._34.Create(this.previousNodeRealizationResult);
if (_34.Success)
{
    return _34;
}

var _35 = _Ⰳx30ⲻ39Realized._35.Create(this.previousNodeRealizationResult);
if (_35.Success)
{
    return _35;
}

var _36 = _Ⰳx30ⲻ39Realized._36.Create(this.previousNodeRealizationResult);
if (_36.Success)
{
    return _36;
}

var _37 = _Ⰳx30ⲻ39Realized._37.Create(this.previousNodeRealizationResult);
if (_37.Success)
{
    return _37;
}

var _38 = _Ⰳx30ⲻ39Realized._38.Create(this.previousNodeRealizationResult);
if (_38.Success)
{
    return _38;
}

var _39 = _Ⰳx30ⲻ39Realized._39.Create(this.previousNodeRealizationResult);
if (_39.Success)
{
    return _39;
}
return new RealizationResult<char, _Ⰳx30ⲻ39Realized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
        }
    }
    
}

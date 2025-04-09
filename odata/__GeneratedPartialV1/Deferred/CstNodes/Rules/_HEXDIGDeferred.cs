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
var _DIGIT = _HEXDIGRealized._DIGIT.Create(this.previousNodeRealizationResult);
if (_DIGIT.Success)
{
    return _DIGIT;
}

var _ʺx41ʺ = _HEXDIGRealized._ʺx41ʺ.Create(this.previousNodeRealizationResult);
if (_ʺx41ʺ.Success)
{
    return _ʺx41ʺ;
}

var _ʺx42ʺ = _HEXDIGRealized._ʺx42ʺ.Create(this.previousNodeRealizationResult);
if (_ʺx42ʺ.Success)
{
    return _ʺx42ʺ;
}

var _ʺx43ʺ = _HEXDIGRealized._ʺx43ʺ.Create(this.previousNodeRealizationResult);
if (_ʺx43ʺ.Success)
{
    return _ʺx43ʺ;
}

var _ʺx44ʺ = _HEXDIGRealized._ʺx44ʺ.Create(this.previousNodeRealizationResult);
if (_ʺx44ʺ.Success)
{
    return _ʺx44ʺ;
}

var _ʺx45ʺ = _HEXDIGRealized._ʺx45ʺ.Create(this.previousNodeRealizationResult);
if (_ʺx45ʺ.Success)
{
    return _ʺx45ʺ;
}

var _ʺx46ʺ = _HEXDIGRealized._ʺx46ʺ.Create(this.previousNodeRealizationResult);
if (_ʺx46ʺ.Success)
{
    return _ʺx46ʺ;
}
return new RealizationResult<char, _HEXDIGRealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
        }
    }
    
}

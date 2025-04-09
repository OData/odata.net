namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _Ⰳx41ⲻ5ADeferred : IAstNode<char, _Ⰳx41ⲻ5ARealized>
    {
        public _Ⰳx41ⲻ5ADeferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            //// TODO get constructor accessibility correct
            this.previousNodeRealizationResult = previousNodeRealizationResult;
            
            this.realizationResult = Future.Create(() => this.RealizeImpl());
        }
        public _Ⰳx41ⲻ5ADeferred(IFuture<IRealizationResult<char, _Ⰳx41ⲻ5ARealized>> realizationResult)
        {
            this.realizationResult = realizationResult;
        }
        
        private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
        private IFuture<IRealizationResult<char, _Ⰳx41ⲻ5ARealized>> realizationResult { get; }
        
        public IRealizationResult<char, _Ⰳx41ⲻ5ARealized> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _Ⰳx41ⲻ5ARealized> RealizeImpl()
        {
            if (!this.previousNodeRealizationResult.Value.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
}
var _41 = _Ⰳx41ⲻ5ARealized._41.Create(this.previousNodeRealizationResult);
if (_41.Success)
{
    return _41;
}

var _42 = _Ⰳx41ⲻ5ARealized._42.Create(this.previousNodeRealizationResult);
if (_42.Success)
{
    return _42;
}

var _43 = _Ⰳx41ⲻ5ARealized._43.Create(this.previousNodeRealizationResult);
if (_43.Success)
{
    return _43;
}

var _44 = _Ⰳx41ⲻ5ARealized._44.Create(this.previousNodeRealizationResult);
if (_44.Success)
{
    return _44;
}

var _45 = _Ⰳx41ⲻ5ARealized._45.Create(this.previousNodeRealizationResult);
if (_45.Success)
{
    return _45;
}

var _46 = _Ⰳx41ⲻ5ARealized._46.Create(this.previousNodeRealizationResult);
if (_46.Success)
{
    return _46;
}

var _47 = _Ⰳx41ⲻ5ARealized._47.Create(this.previousNodeRealizationResult);
if (_47.Success)
{
    return _47;
}

var _48 = _Ⰳx41ⲻ5ARealized._48.Create(this.previousNodeRealizationResult);
if (_48.Success)
{
    return _48;
}

var _49 = _Ⰳx41ⲻ5ARealized._49.Create(this.previousNodeRealizationResult);
if (_49.Success)
{
    return _49;
}

var _4A = _Ⰳx41ⲻ5ARealized._4A.Create(this.previousNodeRealizationResult);
if (_4A.Success)
{
    return _4A;
}

var _4B = _Ⰳx41ⲻ5ARealized._4B.Create(this.previousNodeRealizationResult);
if (_4B.Success)
{
    return _4B;
}

var _4C = _Ⰳx41ⲻ5ARealized._4C.Create(this.previousNodeRealizationResult);
if (_4C.Success)
{
    return _4C;
}

var _4D = _Ⰳx41ⲻ5ARealized._4D.Create(this.previousNodeRealizationResult);
if (_4D.Success)
{
    return _4D;
}

var _4E = _Ⰳx41ⲻ5ARealized._4E.Create(this.previousNodeRealizationResult);
if (_4E.Success)
{
    return _4E;
}

var _4F = _Ⰳx41ⲻ5ARealized._4F.Create(this.previousNodeRealizationResult);
if (_4F.Success)
{
    return _4F;
}

var _50 = _Ⰳx41ⲻ5ARealized._50.Create(this.previousNodeRealizationResult);
if (_50.Success)
{
    return _50;
}

var _51 = _Ⰳx41ⲻ5ARealized._51.Create(this.previousNodeRealizationResult);
if (_51.Success)
{
    return _51;
}

var _52 = _Ⰳx41ⲻ5ARealized._52.Create(this.previousNodeRealizationResult);
if (_52.Success)
{
    return _52;
}

var _53 = _Ⰳx41ⲻ5ARealized._53.Create(this.previousNodeRealizationResult);
if (_53.Success)
{
    return _53;
}

var _54 = _Ⰳx41ⲻ5ARealized._54.Create(this.previousNodeRealizationResult);
if (_54.Success)
{
    return _54;
}

var _55 = _Ⰳx41ⲻ5ARealized._55.Create(this.previousNodeRealizationResult);
if (_55.Success)
{
    return _55;
}

var _56 = _Ⰳx41ⲻ5ARealized._56.Create(this.previousNodeRealizationResult);
if (_56.Success)
{
    return _56;
}

var _57 = _Ⰳx41ⲻ5ARealized._57.Create(this.previousNodeRealizationResult);
if (_57.Success)
{
    return _57;
}

var _58 = _Ⰳx41ⲻ5ARealized._58.Create(this.previousNodeRealizationResult);
if (_58.Success)
{
    return _58;
}

var _59 = _Ⰳx41ⲻ5ARealized._59.Create(this.previousNodeRealizationResult);
if (_59.Success)
{
    return _59;
}

var _5A = _Ⰳx41ⲻ5ARealized._5A.Create(this.previousNodeRealizationResult);
if (_5A.Success)
{
    return _5A;
}
return new RealizationResult<char, _Ⰳx41ⲻ5ARealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
        }
    }
    
}

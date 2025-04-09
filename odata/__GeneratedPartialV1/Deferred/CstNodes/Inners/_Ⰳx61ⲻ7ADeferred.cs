namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _Ⰳx61ⲻ7ADeferred : IAstNode<char, _Ⰳx61ⲻ7ARealized>
    {
        public _Ⰳx61ⲻ7ADeferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            //// TODO get constructor accessibility correct
            this.previousNodeRealizationResult = previousNodeRealizationResult;
            
            this.realizationResult = Future.Create(() => this.RealizeImpl());
        }
        public _Ⰳx61ⲻ7ADeferred(IFuture<IRealizationResult<char, _Ⰳx61ⲻ7ARealized>> realizationResult)
        {
            this.realizationResult = realizationResult;
        }
        
        private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
        private IFuture<IRealizationResult<char, _Ⰳx61ⲻ7ARealized>> realizationResult { get; }
        
        public IRealizationResult<char, _Ⰳx61ⲻ7ARealized> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _Ⰳx61ⲻ7ARealized> RealizeImpl()
        {
            if (!this.previousNodeRealizationResult.Value.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
}
var _61 = _Ⰳx61ⲻ7ARealized._61.Create(this.previousNodeRealizationResult);
if (_61.Success)
{
    return _61;
}

var _62 = _Ⰳx61ⲻ7ARealized._62.Create(this.previousNodeRealizationResult);
if (_62.Success)
{
    return _62;
}

var _63 = _Ⰳx61ⲻ7ARealized._63.Create(this.previousNodeRealizationResult);
if (_63.Success)
{
    return _63;
}

var _64 = _Ⰳx61ⲻ7ARealized._64.Create(this.previousNodeRealizationResult);
if (_64.Success)
{
    return _64;
}

var _65 = _Ⰳx61ⲻ7ARealized._65.Create(this.previousNodeRealizationResult);
if (_65.Success)
{
    return _65;
}

var _66 = _Ⰳx61ⲻ7ARealized._66.Create(this.previousNodeRealizationResult);
if (_66.Success)
{
    return _66;
}

var _67 = _Ⰳx61ⲻ7ARealized._67.Create(this.previousNodeRealizationResult);
if (_67.Success)
{
    return _67;
}

var _68 = _Ⰳx61ⲻ7ARealized._68.Create(this.previousNodeRealizationResult);
if (_68.Success)
{
    return _68;
}

var _69 = _Ⰳx61ⲻ7ARealized._69.Create(this.previousNodeRealizationResult);
if (_69.Success)
{
    return _69;
}

var _6A = _Ⰳx61ⲻ7ARealized._6A.Create(this.previousNodeRealizationResult);
if (_6A.Success)
{
    return _6A;
}

var _6B = _Ⰳx61ⲻ7ARealized._6B.Create(this.previousNodeRealizationResult);
if (_6B.Success)
{
    return _6B;
}

var _6C = _Ⰳx61ⲻ7ARealized._6C.Create(this.previousNodeRealizationResult);
if (_6C.Success)
{
    return _6C;
}

var _6D = _Ⰳx61ⲻ7ARealized._6D.Create(this.previousNodeRealizationResult);
if (_6D.Success)
{
    return _6D;
}

var _6E = _Ⰳx61ⲻ7ARealized._6E.Create(this.previousNodeRealizationResult);
if (_6E.Success)
{
    return _6E;
}

var _6F = _Ⰳx61ⲻ7ARealized._6F.Create(this.previousNodeRealizationResult);
if (_6F.Success)
{
    return _6F;
}

var _70 = _Ⰳx61ⲻ7ARealized._70.Create(this.previousNodeRealizationResult);
if (_70.Success)
{
    return _70;
}

var _71 = _Ⰳx61ⲻ7ARealized._71.Create(this.previousNodeRealizationResult);
if (_71.Success)
{
    return _71;
}

var _72 = _Ⰳx61ⲻ7ARealized._72.Create(this.previousNodeRealizationResult);
if (_72.Success)
{
    return _72;
}

var _73 = _Ⰳx61ⲻ7ARealized._73.Create(this.previousNodeRealizationResult);
if (_73.Success)
{
    return _73;
}

var _74 = _Ⰳx61ⲻ7ARealized._74.Create(this.previousNodeRealizationResult);
if (_74.Success)
{
    return _74;
}

var _75 = _Ⰳx61ⲻ7ARealized._75.Create(this.previousNodeRealizationResult);
if (_75.Success)
{
    return _75;
}

var _76 = _Ⰳx61ⲻ7ARealized._76.Create(this.previousNodeRealizationResult);
if (_76.Success)
{
    return _76;
}

var _77 = _Ⰳx61ⲻ7ARealized._77.Create(this.previousNodeRealizationResult);
if (_77.Success)
{
    return _77;
}

var _78 = _Ⰳx61ⲻ7ARealized._78.Create(this.previousNodeRealizationResult);
if (_78.Success)
{
    return _78;
}

var _79 = _Ⰳx61ⲻ7ARealized._79.Create(this.previousNodeRealizationResult);
if (_79.Success)
{
    return _79;
}

var _7A = _Ⰳx61ⲻ7ARealized._7A.Create(this.previousNodeRealizationResult);
if (_7A.Success)
{
    return _7A;
}
return new RealizationResult<char, _Ⰳx61ⲻ7ARealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
        }
    }
    
}

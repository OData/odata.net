namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _Ⰳx00ⲻ1FDeferred : IAstNode<char, _Ⰳx00ⲻ1FRealized>
    {
        public _Ⰳx00ⲻ1FDeferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            //// TODO get constructor accessibility correct
            this.previousNodeRealizationResult = previousNodeRealizationResult;
            
            this.realizationResult = Future.Create(() => this.RealizeImpl());
        }
        public _Ⰳx00ⲻ1FDeferred(IFuture<IRealizationResult<char, _Ⰳx00ⲻ1FRealized>> realizationResult)
        {
            this.realizationResult = realizationResult;
        }
        
        private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
        private IFuture<IRealizationResult<char, _Ⰳx00ⲻ1FRealized>> realizationResult { get; }
        
        public IRealizationResult<char, _Ⰳx00ⲻ1FRealized> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _Ⰳx00ⲻ1FRealized> RealizeImpl()
        {
            if (!this.previousNodeRealizationResult.Value.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
}
var _00 = _Ⰳx00ⲻ1FRealized._00.Create(this.previousNodeRealizationResult);
if (_00.Success)
{
    return _00;
}

var _01 = _Ⰳx00ⲻ1FRealized._01.Create(this.previousNodeRealizationResult);
if (_01.Success)
{
    return _01;
}

var _02 = _Ⰳx00ⲻ1FRealized._02.Create(this.previousNodeRealizationResult);
if (_02.Success)
{
    return _02;
}

var _03 = _Ⰳx00ⲻ1FRealized._03.Create(this.previousNodeRealizationResult);
if (_03.Success)
{
    return _03;
}

var _04 = _Ⰳx00ⲻ1FRealized._04.Create(this.previousNodeRealizationResult);
if (_04.Success)
{
    return _04;
}

var _05 = _Ⰳx00ⲻ1FRealized._05.Create(this.previousNodeRealizationResult);
if (_05.Success)
{
    return _05;
}

var _06 = _Ⰳx00ⲻ1FRealized._06.Create(this.previousNodeRealizationResult);
if (_06.Success)
{
    return _06;
}

var _07 = _Ⰳx00ⲻ1FRealized._07.Create(this.previousNodeRealizationResult);
if (_07.Success)
{
    return _07;
}

var _08 = _Ⰳx00ⲻ1FRealized._08.Create(this.previousNodeRealizationResult);
if (_08.Success)
{
    return _08;
}

var _09 = _Ⰳx00ⲻ1FRealized._09.Create(this.previousNodeRealizationResult);
if (_09.Success)
{
    return _09;
}

var _0A = _Ⰳx00ⲻ1FRealized._0A.Create(this.previousNodeRealizationResult);
if (_0A.Success)
{
    return _0A;
}

var _0B = _Ⰳx00ⲻ1FRealized._0B.Create(this.previousNodeRealizationResult);
if (_0B.Success)
{
    return _0B;
}

var _0C = _Ⰳx00ⲻ1FRealized._0C.Create(this.previousNodeRealizationResult);
if (_0C.Success)
{
    return _0C;
}

var _0D = _Ⰳx00ⲻ1FRealized._0D.Create(this.previousNodeRealizationResult);
if (_0D.Success)
{
    return _0D;
}

var _0E = _Ⰳx00ⲻ1FRealized._0E.Create(this.previousNodeRealizationResult);
if (_0E.Success)
{
    return _0E;
}

var _0F = _Ⰳx00ⲻ1FRealized._0F.Create(this.previousNodeRealizationResult);
if (_0F.Success)
{
    return _0F;
}

var _10 = _Ⰳx00ⲻ1FRealized._10.Create(this.previousNodeRealizationResult);
if (_10.Success)
{
    return _10;
}

var _11 = _Ⰳx00ⲻ1FRealized._11.Create(this.previousNodeRealizationResult);
if (_11.Success)
{
    return _11;
}

var _12 = _Ⰳx00ⲻ1FRealized._12.Create(this.previousNodeRealizationResult);
if (_12.Success)
{
    return _12;
}

var _13 = _Ⰳx00ⲻ1FRealized._13.Create(this.previousNodeRealizationResult);
if (_13.Success)
{
    return _13;
}

var _14 = _Ⰳx00ⲻ1FRealized._14.Create(this.previousNodeRealizationResult);
if (_14.Success)
{
    return _14;
}

var _15 = _Ⰳx00ⲻ1FRealized._15.Create(this.previousNodeRealizationResult);
if (_15.Success)
{
    return _15;
}

var _16 = _Ⰳx00ⲻ1FRealized._16.Create(this.previousNodeRealizationResult);
if (_16.Success)
{
    return _16;
}

var _17 = _Ⰳx00ⲻ1FRealized._17.Create(this.previousNodeRealizationResult);
if (_17.Success)
{
    return _17;
}

var _18 = _Ⰳx00ⲻ1FRealized._18.Create(this.previousNodeRealizationResult);
if (_18.Success)
{
    return _18;
}

var _19 = _Ⰳx00ⲻ1FRealized._19.Create(this.previousNodeRealizationResult);
if (_19.Success)
{
    return _19;
}

var _1A = _Ⰳx00ⲻ1FRealized._1A.Create(this.previousNodeRealizationResult);
if (_1A.Success)
{
    return _1A;
}

var _1B = _Ⰳx00ⲻ1FRealized._1B.Create(this.previousNodeRealizationResult);
if (_1B.Success)
{
    return _1B;
}

var _1C = _Ⰳx00ⲻ1FRealized._1C.Create(this.previousNodeRealizationResult);
if (_1C.Success)
{
    return _1C;
}

var _1D = _Ⰳx00ⲻ1FRealized._1D.Create(this.previousNodeRealizationResult);
if (_1D.Success)
{
    return _1D;
}

var _1E = _Ⰳx00ⲻ1FRealized._1E.Create(this.previousNodeRealizationResult);
if (_1E.Success)
{
    return _1E;
}

var _1F = _Ⰳx00ⲻ1FRealized._1F.Create(this.previousNodeRealizationResult);
if (_1F.Success)
{
    return _1F;
}
return new RealizationResult<char, _Ⰳx00ⲻ1FRealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
        }
    }
    
}

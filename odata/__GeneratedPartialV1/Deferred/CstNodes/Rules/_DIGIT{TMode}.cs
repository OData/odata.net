namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _DIGIT<TMode> : IAstNode<char, _DIGIT<ParseMode.Realized>>, IFromRealizedable<_DIGIT<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _DIGIT(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx30ⲻ39<TMode>> _Ⰳx30ⲻ39_1)
        {
            this.__Ⰳx30ⲻ39_1 = _Ⰳx30ⲻ39_1;
            this.realizationResult = new Future<IRealizationResult<char, _DIGIT<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _DIGIT(__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx30ⲻ39<TMode> _Ⰳx30ⲻ39_1, IFuture<IRealizationResult<char, _DIGIT<ParseMode.Realized>>> realizationResult)
        {
            this.__Ⰳx30ⲻ39_1 = Future.Create(() => _Ⰳx30ⲻ39_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx30ⲻ39<TMode>> __Ⰳx30ⲻ39_1 { get; }
        private IFuture<IRealizationResult<char, _DIGIT<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx30ⲻ39<TMode> _Ⰳx30ⲻ39_1 { get{
        return this.__Ⰳx30ⲻ39_1.Value;
        }
        }
        
        internal static _DIGIT<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var _Ⰳx30ⲻ39_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx30ⲻ39.Create(previousNodeRealizationResult));
return new _DIGIT<ParseMode.Deferred>(_Ⰳx30ⲻ39_1);
        }
        
        public _DIGIT<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _DIGIT<ParseMode.Deferred>(
        this.__Ⰳx30ⲻ39_1.Select(_ => _.Convert()));
}
else
{
    return new _DIGIT<ParseMode.Deferred>(
        this.__Ⰳx30ⲻ39_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _DIGIT<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _DIGIT<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._Ⰳx30ⲻ39_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _DIGIT<ParseMode.Realized>>(
        true,
        new _DIGIT<ParseMode.Realized>(
            this._Ⰳx30ⲻ39_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _DIGIT<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

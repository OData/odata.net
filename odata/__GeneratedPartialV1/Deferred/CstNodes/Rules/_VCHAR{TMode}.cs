namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _VCHAR<TMode> : IAstNode<char, _VCHAR<ParseMode.Realized>>, IFromRealizedable<_VCHAR<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _VCHAR(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx21ⲻ7E<TMode>> _Ⰳx21ⲻ7E_1)
        {
            this.__Ⰳx21ⲻ7E_1 = _Ⰳx21ⲻ7E_1;
            this.realizationResult = new Future<IRealizationResult<char, _VCHAR<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _VCHAR(__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx21ⲻ7E<TMode> _Ⰳx21ⲻ7E_1, IFuture<IRealizationResult<char, _VCHAR<ParseMode.Realized>>> realizationResult)
        {
            this.__Ⰳx21ⲻ7E_1 = Future.Create(() => _Ⰳx21ⲻ7E_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx21ⲻ7E<TMode>> __Ⰳx21ⲻ7E_1 { get; }
        private IFuture<IRealizationResult<char, _VCHAR<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx21ⲻ7E<TMode> _Ⰳx21ⲻ7E_1 { get; }
        
        public _VCHAR<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _VCHAR<ParseMode.Deferred>(
        this.__Ⰳx21ⲻ7E_1.Select(_ => _.Convert()));
}
else
{
    return new _VCHAR<ParseMode.Deferred>(
        this.__Ⰳx21ⲻ7E_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _VCHAR<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _VCHAR<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._Ⰳx21ⲻ7E_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _VCHAR<ParseMode.Realized>>(
        true,
        new _VCHAR<ParseMode.Realized>(
            this._Ⰳx21ⲻ7E_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _VCHAR<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

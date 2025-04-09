namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _OCTET<TMode> : IAstNode<char, _OCTET<ParseMode.Realized>>, IFromRealizedable<_OCTET<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _OCTET(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx00ⲻFF<TMode>> _Ⰳx00ⲻFF_1)
        {
            this.__Ⰳx00ⲻFF_1 = _Ⰳx00ⲻFF_1;
            this.realizationResult = new Future<IRealizationResult<char, _OCTET<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _OCTET(__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx00ⲻFF<TMode> _Ⰳx00ⲻFF_1, IFuture<IRealizationResult<char, _OCTET<ParseMode.Realized>>> realizationResult)
        {
            this.__Ⰳx00ⲻFF_1 = Future.Create(() => _Ⰳx00ⲻFF_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx00ⲻFF<TMode>> __Ⰳx00ⲻFF_1 { get; }
        private IFuture<IRealizationResult<char, _OCTET<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx00ⲻFF<TMode> _Ⰳx00ⲻFF_1 { get; }
        
        public _OCTET<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _OCTET<ParseMode.Deferred>(
        this.__Ⰳx00ⲻFF_1.Select(_ => _.Convert()));
}
else
{
    return new _OCTET<ParseMode.Deferred>(
        this.__Ⰳx00ⲻFF_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _OCTET<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _OCTET<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._Ⰳx00ⲻFF_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _OCTET<ParseMode.Realized>>(
        true,
        new _OCTET<ParseMode.Realized>(
            this._Ⰳx00ⲻFF_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _OCTET<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

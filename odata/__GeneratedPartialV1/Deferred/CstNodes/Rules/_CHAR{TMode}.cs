namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _CHAR<TMode> : IAstNode<char, _CHAR<ParseMode.Realized>>, IFromRealizedable<_CHAR<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _CHAR(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx01ⲻ7F<TMode>> _Ⰳx01ⲻ7F_1)
        {
            this.__Ⰳx01ⲻ7F_1 = _Ⰳx01ⲻ7F_1;
            this.realizationResult = new Future<IRealizationResult<char, _CHAR<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _CHAR(__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx01ⲻ7F<TMode> _Ⰳx01ⲻ7F_1, IFuture<IRealizationResult<char, _CHAR<ParseMode.Realized>>> realizationResult)
        {
            this.__Ⰳx01ⲻ7F_1 = Future.Create(() => _Ⰳx01ⲻ7F_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx01ⲻ7F<TMode>> __Ⰳx01ⲻ7F_1 { get; }
        private IFuture<IRealizationResult<char, _CHAR<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx01ⲻ7F<TMode> _Ⰳx01ⲻ7F_1 { get; }
        
        internal static _CHAR<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var _Ⰳx01ⲻ7F_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx01ⲻ7F.Create(previousNodeRealizationResult));
return new _CHAR<ParseMode.Deferred>(_Ⰳx01ⲻ7F_1);
        }
        
        public _CHAR<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _CHAR<ParseMode.Deferred>(
        this.__Ⰳx01ⲻ7F_1.Select(_ => _.Convert()));
}
else
{
    return new _CHAR<ParseMode.Deferred>(
        this.__Ⰳx01ⲻ7F_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _CHAR<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _CHAR<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._Ⰳx01ⲻ7F_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _CHAR<ParseMode.Realized>>(
        true,
        new _CHAR<ParseMode.Realized>(
            this._Ⰳx01ⲻ7F_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _CHAR<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _HTAB<TMode> : IAstNode<char, _HTAB<ParseMode.Realized>>, IFromRealizedable<_HTAB<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _HTAB(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx09<TMode>> _Ⰳx09_1)
        {
            this.__Ⰳx09_1 = _Ⰳx09_1;
            this.realizationResult = new Future<IRealizationResult<char, _HTAB<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _HTAB(__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx09<TMode> _Ⰳx09_1, IFuture<IRealizationResult<char, _HTAB<ParseMode.Realized>>> realizationResult)
        {
            this.__Ⰳx09_1 = Future.Create(() => _Ⰳx09_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx09<TMode>> __Ⰳx09_1 { get; }
        private IFuture<IRealizationResult<char, _HTAB<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx09<TMode> _Ⰳx09_1 { get; }
        
        internal static _HTAB<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var _Ⰳx09_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx09.Create(previousNodeRealizationResult));
return new _HTAB<ParseMode.Deferred>(_Ⰳx09_1);
        }
        
        public _HTAB<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _HTAB<ParseMode.Deferred>(
        this.__Ⰳx09_1.Select(_ => _.Convert()));
}
else
{
    return new _HTAB<ParseMode.Deferred>(
        this.__Ⰳx09_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _HTAB<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _HTAB<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._Ⰳx09_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _HTAB<ParseMode.Realized>>(
        true,
        new _HTAB<ParseMode.Realized>(
            this._Ⰳx09_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _HTAB<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _SP<TMode> : IAstNode<char, _SP<ParseMode.Realized>>, IFromRealizedable<_SP<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _SP(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx20<TMode>> _Ⰳx20_1)
        {
            this.__Ⰳx20_1 = _Ⰳx20_1;
            this.realizationResult = new Future<IRealizationResult<char, _SP<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _SP(__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx20<TMode> _Ⰳx20_1, IFuture<IRealizationResult<char, _SP<ParseMode.Realized>>> realizationResult)
        {
            this.__Ⰳx20_1 = Future.Create(() => _Ⰳx20_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx20<TMode>> __Ⰳx20_1 { get; }
        private IFuture<IRealizationResult<char, _SP<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx20<TMode> _Ⰳx20_1 { get; }
        
        public _SP<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _SP<ParseMode.Deferred>(
        this.__Ⰳx20_1.Select(_ => _.Convert()));
}
else
{
    return new _SP<ParseMode.Deferred>(
        this.__Ⰳx20_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _SP<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _SP<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._Ⰳx20_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _SP<ParseMode.Realized>>(
        true,
        new _SP<ParseMode.Realized>(
            this._Ⰳx20_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _SP<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

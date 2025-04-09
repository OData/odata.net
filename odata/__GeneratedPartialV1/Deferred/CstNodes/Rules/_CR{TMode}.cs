namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _CR<TMode> : IAstNode<char, _CR<ParseMode.Realized>>, IFromRealizedable<_CR<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _CR(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx0D<TMode>> _Ⰳx0D_1)
        {
            this.__Ⰳx0D_1 = _Ⰳx0D_1;
            this.realizationResult = new Future<IRealizationResult<char, _CR<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _CR(__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx0D<TMode> _Ⰳx0D_1, IFuture<IRealizationResult<char, _CR<ParseMode.Realized>>> realizationResult)
        {
            this.__Ⰳx0D_1 = Future.Create(() => _Ⰳx0D_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx0D<TMode>> __Ⰳx0D_1 { get; }
        private IFuture<IRealizationResult<char, _CR<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx0D<TMode> _Ⰳx0D_1 { get; }
        
        public _CR<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _CR<ParseMode.Deferred>(
        this.__Ⰳx0D_1.Select(_ => _.Convert()));
}
else
{
    return new _CR<ParseMode.Deferred>(
        this.__Ⰳx0D_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _CR<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _CR<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._Ⰳx0D_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _CR<ParseMode.Realized>>(
        true,
        new _CR<ParseMode.Realized>(
            this._Ⰳx0D_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _CR<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

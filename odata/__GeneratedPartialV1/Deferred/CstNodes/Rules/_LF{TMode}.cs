namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _LF<TMode> : IAstNode<char, _LF<ParseMode.Realized>>, IFromRealizedable<_LF<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _LF(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx0A<TMode>> _Ⰳx0A_1)
        {
            this.__Ⰳx0A_1 = _Ⰳx0A_1;
            this.realizationResult = new Future<IRealizationResult<char, _LF<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _LF(__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx0A<TMode> _Ⰳx0A_1, IFuture<IRealizationResult<char, _LF<ParseMode.Realized>>> realizationResult)
        {
            this.__Ⰳx0A_1 = Future.Create(() => _Ⰳx0A_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx0A<TMode>> __Ⰳx0A_1 { get; }
        private IFuture<IRealizationResult<char, _LF<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx0A<TMode> _Ⰳx0A_1 { get; }
        
        public _LF<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _LF<ParseMode.Deferred>(
        this.__Ⰳx0A_1.Select(_ => _.Convert()));
}
else
{
    return new _LF<ParseMode.Deferred>(
        this.__Ⰳx0A_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _LF<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _LF<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._Ⰳx0A_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _LF<ParseMode.Realized>>(
        true,
        new _LF<ParseMode.Realized>(
            this._Ⰳx0A_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _LF<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

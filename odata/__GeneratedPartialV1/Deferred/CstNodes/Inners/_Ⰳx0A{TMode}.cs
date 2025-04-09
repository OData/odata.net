namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _Ⰳx0A<TMode> : IAstNode<char, _Ⰳx0A<ParseMode.Realized>>, IFromRealizedable<_Ⰳx0A<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _Ⰳx0A(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1)
        {
            this.__0_1 = _0_1;
            this.__A_1 = _A_1;
            this.realizationResult = new Future<IRealizationResult<char, _Ⰳx0A<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _Ⰳx0A(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1, IFuture<IRealizationResult<char, _Ⰳx0A<ParseMode.Realized>>> realizationResult)
        {
            this.__0_1 = Future.Create(() => _0_1);
            this.__A_1 = Future.Create(() => _A_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
        private IFuture<IRealizationResult<char, _Ⰳx0A<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
        
        public _Ⰳx0A<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _Ⰳx0A<ParseMode.Deferred>(
        this.__0_1.Select(_ => _.Convert()),
this.__A_1.Select(_ => _.Convert()));
}
else
{
    return new _Ⰳx0A<ParseMode.Deferred>(
        this.__0_1.Value.Convert(),
this.__A_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _Ⰳx0A<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _Ⰳx0A<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._A_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _Ⰳx0A<ParseMode.Realized>>(
        true,
        new _Ⰳx0A<ParseMode.Realized>(
            this._0_1.Realize().RealizedValue,
this._A_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _Ⰳx0A<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

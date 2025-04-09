namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _ʺx46ʺ<TMode> : IAstNode<char, _ʺx46ʺ<ParseMode.Realized>>, IFromRealizedable<_ʺx46ʺ<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _ʺx46ʺ(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._x46<TMode>> _x46_1)
        {
            this.__x46_1 = _x46_1;
            this.realizationResult = new Future<IRealizationResult<char, _ʺx46ʺ<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _ʺx46ʺ(__GeneratedPartialV1.Deferred.CstNodes.Inners._x46<TMode> _x46_1, IFuture<IRealizationResult<char, _ʺx46ʺ<ParseMode.Realized>>> realizationResult)
        {
            this.__x46_1 = Future.Create(() => _x46_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._x46<TMode>> __x46_1 { get; }
        private IFuture<IRealizationResult<char, _ʺx46ʺ<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._x46<TMode> _x46_1 { get; }
        
        public _ʺx46ʺ<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _ʺx46ʺ<ParseMode.Deferred>(
        this.__x46_1.Select(_ => _.Convert()));
}
else
{
    return new _ʺx46ʺ<ParseMode.Deferred>(
        this.__x46_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _ʺx46ʺ<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _ʺx46ʺ<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._x46_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _ʺx46ʺ<ParseMode.Realized>>(
        true,
        new _ʺx46ʺ<ParseMode.Realized>(
            this._x46_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _ʺx46ʺ<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

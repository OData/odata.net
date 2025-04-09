namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _ʺx41ʺ<TMode> : IAstNode<char, _ʺx41ʺ<ParseMode.Realized>>, IFromRealizedable<_ʺx41ʺ<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _ʺx41ʺ(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._x41<TMode>> _x41_1)
        {
            this.__x41_1 = _x41_1;
            this.realizationResult = new Future<IRealizationResult<char, _ʺx41ʺ<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _ʺx41ʺ(__GeneratedPartialV1.Deferred.CstNodes.Inners._x41<TMode> _x41_1, IFuture<IRealizationResult<char, _ʺx41ʺ<ParseMode.Realized>>> realizationResult)
        {
            this.__x41_1 = Future.Create(() => _x41_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._x41<TMode>> __x41_1 { get; }
        private IFuture<IRealizationResult<char, _ʺx41ʺ<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._x41<TMode> _x41_1 { get; }
        
        public _ʺx41ʺ<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _ʺx41ʺ<ParseMode.Deferred>(
        this.__x41_1.Select(_ => _.Convert()));
}
else
{
    return new _ʺx41ʺ<ParseMode.Deferred>(
        this.__x41_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _ʺx41ʺ<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _ʺx41ʺ<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._x41_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _ʺx41ʺ<ParseMode.Realized>>(
        true,
        new _ʺx41ʺ<ParseMode.Realized>(
            this._x41_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _ʺx41ʺ<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

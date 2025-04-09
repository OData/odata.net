namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _ʺx43ʺ<TMode> : IAstNode<char, _ʺx43ʺ<ParseMode.Realized>>, IFromRealizedable<_ʺx43ʺ<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _ʺx43ʺ(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._x43<TMode>> _x43_1)
        {
            this.__x43_1 = _x43_1;
            this.realizationResult = new Future<IRealizationResult<char, _ʺx43ʺ<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _ʺx43ʺ(__GeneratedPartialV1.Deferred.CstNodes.Inners._x43<TMode> _x43_1, IFuture<IRealizationResult<char, _ʺx43ʺ<ParseMode.Realized>>> realizationResult)
        {
            this.__x43_1 = Future.Create(() => _x43_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._x43<TMode>> __x43_1 { get; }
        private IFuture<IRealizationResult<char, _ʺx43ʺ<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._x43<TMode> _x43_1 { get; }
        
        public _ʺx43ʺ<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _ʺx43ʺ<ParseMode.Deferred>(
        this.__x43_1.Select(_ => _.Convert()));
}
else
{
    return new _ʺx43ʺ<ParseMode.Deferred>(
        this.__x43_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _ʺx43ʺ<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _ʺx43ʺ<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._x43_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _ʺx43ʺ<ParseMode.Realized>>(
        true,
        new _ʺx43ʺ<ParseMode.Realized>(
            this._x43_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _ʺx43ʺ<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

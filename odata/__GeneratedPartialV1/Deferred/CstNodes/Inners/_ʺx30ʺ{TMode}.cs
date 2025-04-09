namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _ʺx30ʺ<TMode> : IAstNode<char, _ʺx30ʺ<ParseMode.Realized>>, IFromRealizedable<_ʺx30ʺ<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _ʺx30ʺ(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._x30<TMode>> _x30_1)
        {
            this.__x30_1 = _x30_1;
            this.realizationResult = new Future<IRealizationResult<char, _ʺx30ʺ<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _ʺx30ʺ(__GeneratedPartialV1.Deferred.CstNodes.Inners._x30<TMode> _x30_1, IFuture<IRealizationResult<char, _ʺx30ʺ<ParseMode.Realized>>> realizationResult)
        {
            this.__x30_1 = Future.Create(() => _x30_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._x30<TMode>> __x30_1 { get; }
        private IFuture<IRealizationResult<char, _ʺx30ʺ<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._x30<TMode> _x30_1 { get; }
        
        public _ʺx30ʺ<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _ʺx30ʺ<ParseMode.Deferred>(
        this.__x30_1.Select(_ => _.Convert()));
}
else
{
    return new _ʺx30ʺ<ParseMode.Deferred>(
        this.__x30_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _ʺx30ʺ<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _ʺx30ʺ<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._x30_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _ʺx30ʺ<ParseMode.Realized>>(
        true,
        new _ʺx30ʺ<ParseMode.Realized>(
            this._x30_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _ʺx30ʺ<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _ʺx44ʺ<TMode> : IAstNode<char, _ʺx44ʺ<ParseMode.Realized>>, IFromRealizedable<_ʺx44ʺ<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _ʺx44ʺ(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._x44<TMode>> _x44_1)
        {
            this.__x44_1 = _x44_1;
            this.realizationResult = new Future<IRealizationResult<char, _ʺx44ʺ<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _ʺx44ʺ(__GeneratedPartialV1.Deferred.CstNodes.Inners._x44<TMode> _x44_1, IFuture<IRealizationResult<char, _ʺx44ʺ<ParseMode.Realized>>> realizationResult)
        {
            this.__x44_1 = Future.Create(() => _x44_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._x44<TMode>> __x44_1 { get; }
        private IFuture<IRealizationResult<char, _ʺx44ʺ<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._x44<TMode> _x44_1 { get; }
        
        public _ʺx44ʺ<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _ʺx44ʺ<ParseMode.Deferred>(
        this.__x44_1.Select(_ => _.Convert()));
}
else
{
    return new _ʺx44ʺ<ParseMode.Deferred>(
        this.__x44_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _ʺx44ʺ<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _ʺx44ʺ<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._x44_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _ʺx44ʺ<ParseMode.Realized>>(
        true,
        new _ʺx44ʺ<ParseMode.Realized>(
            this._x44_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _ʺx44ʺ<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _ʺx45ʺ<TMode> : IAstNode<char, _ʺx45ʺ<ParseMode.Realized>>, IFromRealizedable<_ʺx45ʺ<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _ʺx45ʺ(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._x45<TMode>> _x45_1)
        {
            this.__x45_1 = _x45_1;
            this.realizationResult = new Future<IRealizationResult<char, _ʺx45ʺ<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _ʺx45ʺ(__GeneratedPartialV1.Deferred.CstNodes.Inners._x45<TMode> _x45_1, IFuture<IRealizationResult<char, _ʺx45ʺ<ParseMode.Realized>>> realizationResult)
        {
            this.__x45_1 = Future.Create(() => _x45_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._x45<TMode>> __x45_1 { get; }
        private IFuture<IRealizationResult<char, _ʺx45ʺ<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._x45<TMode> _x45_1 { get; }
        
        public _ʺx45ʺ<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _ʺx45ʺ<ParseMode.Deferred>(
        this.__x45_1.Select(_ => _.Convert()));
}
else
{
    return new _ʺx45ʺ<ParseMode.Deferred>(
        this.__x45_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _ʺx45ʺ<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _ʺx45ʺ<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._x45_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _ʺx45ʺ<ParseMode.Realized>>(
        true,
        new _ʺx45ʺ<ParseMode.Realized>(
            this._x45_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _ʺx45ʺ<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

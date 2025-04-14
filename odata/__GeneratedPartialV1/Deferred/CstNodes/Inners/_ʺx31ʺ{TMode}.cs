namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _ʺx31ʺ<TMode> : IAstNode<char, _ʺx31ʺ<ParseMode.Realized>>, IFromRealizedable<_ʺx31ʺ<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _ʺx31ʺ(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._x31<TMode>> _x31_1)
        {
            this.__x31_1 = _x31_1;
            this.realizationResult = new Future<IRealizationResult<char, _ʺx31ʺ<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _ʺx31ʺ(__GeneratedPartialV1.Deferred.CstNodes.Inners._x31<TMode> _x31_1, IFuture<IRealizationResult<char, _ʺx31ʺ<ParseMode.Realized>>> realizationResult)
        {
            this.__x31_1 = Future.Create(() => _x31_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._x31<TMode>> __x31_1 { get; }
        private IFuture<IRealizationResult<char, _ʺx31ʺ<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._x31<TMode> _x31_1 { get; }
        
        internal static _ʺx31ʺ<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var _x31_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Inners._x31.Create(previousNodeRealizationResult));
return new _ʺx31ʺ<ParseMode.Deferred>(_x31_1);
        }
        
        public _ʺx31ʺ<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _ʺx31ʺ<ParseMode.Deferred>(
        this.__x31_1.Select(_ => _.Convert()));
}
else
{
    return new _ʺx31ʺ<ParseMode.Deferred>(
        this.__x31_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _ʺx31ʺ<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _ʺx31ʺ<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._x31_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _ʺx31ʺ<ParseMode.Realized>>(
        true,
        new _ʺx31ʺ<ParseMode.Realized>(
            this._x31_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _ʺx31ʺ<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

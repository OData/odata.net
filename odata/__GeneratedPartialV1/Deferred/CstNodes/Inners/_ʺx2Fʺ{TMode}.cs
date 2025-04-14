namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _ʺx2Fʺ<TMode> : IAstNode<char, _ʺx2Fʺ<ParseMode.Realized>>, IFromRealizedable<_ʺx2Fʺ<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _ʺx2Fʺ(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._x2F<TMode>> _x2F_1)
        {
            this.__x2F_1 = _x2F_1;
            this.realizationResult = new Future<IRealizationResult<char, _ʺx2Fʺ<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _ʺx2Fʺ(__GeneratedPartialV1.Deferred.CstNodes.Inners._x2F<TMode> _x2F_1, IFuture<IRealizationResult<char, _ʺx2Fʺ<ParseMode.Realized>>> realizationResult)
        {
            this.__x2F_1 = Future.Create(() => _x2F_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._x2F<TMode>> __x2F_1 { get; }
        private IFuture<IRealizationResult<char, _ʺx2Fʺ<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._x2F<TMode> _x2F_1 { get{
        return this.__x2F_1.Value;
        }
        }
        
        internal static _ʺx2Fʺ<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var _x2F_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Inners._x2F.Create(previousNodeRealizationResult));
return new _ʺx2Fʺ<ParseMode.Deferred>(_x2F_1);
        }
        
        public _ʺx2Fʺ<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _ʺx2Fʺ<ParseMode.Deferred>(
        this.__x2F_1.Select(_ => _.Convert()));
}
else
{
    return new _ʺx2Fʺ<ParseMode.Deferred>(
        this.__x2F_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _ʺx2Fʺ<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _ʺx2Fʺ<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._x2F_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _ʺx2Fʺ<ParseMode.Realized>>(
        true,
        new _ʺx2Fʺ<ParseMode.Realized>(
            this._x2F_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _ʺx2Fʺ<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

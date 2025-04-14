namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _ʺx3Fʺ<TMode> : IAstNode<char, _ʺx3Fʺ<ParseMode.Realized>>, IFromRealizedable<_ʺx3Fʺ<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _ʺx3Fʺ(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._x3F<TMode>> _x3F_1)
        {
            this.__x3F_1 = _x3F_1;
            this.realizationResult = new Future<IRealizationResult<char, _ʺx3Fʺ<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _ʺx3Fʺ(__GeneratedPartialV1.Deferred.CstNodes.Inners._x3F<TMode> _x3F_1, IFuture<IRealizationResult<char, _ʺx3Fʺ<ParseMode.Realized>>> realizationResult)
        {
            this.__x3F_1 = Future.Create(() => _x3F_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._x3F<TMode>> __x3F_1 { get; }
        private IFuture<IRealizationResult<char, _ʺx3Fʺ<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._x3F<TMode> _x3F_1 { get{
        return this.__x3F_1.Value;
        }
        }
        
        internal static _ʺx3Fʺ<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var _x3F_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Inners._x3F.Create(previousNodeRealizationResult));
return new _ʺx3Fʺ<ParseMode.Deferred>(_x3F_1);
        }
        
        public _ʺx3Fʺ<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _ʺx3Fʺ<ParseMode.Deferred>(
        this.__x3F_1.Select(_ => _.Convert()));
}
else
{
    return new _ʺx3Fʺ<ParseMode.Deferred>(
        this.__x3F_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _ʺx3Fʺ<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _ʺx3Fʺ<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._x3F_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _ʺx3Fʺ<ParseMode.Realized>>(
        true,
        new _ʺx3Fʺ<ParseMode.Realized>(
            this._x3F_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _ʺx3Fʺ<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

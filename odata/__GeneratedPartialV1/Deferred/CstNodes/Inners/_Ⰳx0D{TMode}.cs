namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _Ⰳx0D<TMode> : IAstNode<char, _Ⰳx0D<ParseMode.Realized>>, IFromRealizedable<_Ⰳx0D<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _Ⰳx0D(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1)
        {
            this.__0_1 = _0_1;
            this.__D_1 = _D_1;
            this.realizationResult = new Future<IRealizationResult<char, _Ⰳx0D<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _Ⰳx0D(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1, IFuture<IRealizationResult<char, _Ⰳx0D<ParseMode.Realized>>> realizationResult)
        {
            this.__0_1 = Future.Create(() => _0_1);
            this.__D_1 = Future.Create(() => _D_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
        private IFuture<IRealizationResult<char, _Ⰳx0D<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get{
        return this.__0_1.Value;
        }
        }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get{
        return this.__D_1.Value;
        }
        }
        
        internal static _Ⰳx0D<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var _0_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(previousNodeRealizationResult));
var _D_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _0_1.Value.Realize())));
return new _Ⰳx0D<ParseMode.Deferred>(_0_1, _D_1);
        }
        
        public _Ⰳx0D<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _Ⰳx0D<ParseMode.Deferred>(
        this.__0_1.Select(_ => _.Convert()),
this.__D_1.Select(_ => _.Convert()));
}
else
{
    return new _Ⰳx0D<ParseMode.Deferred>(
        this.__0_1.Value.Convert(),
this.__D_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _Ⰳx0D<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _Ⰳx0D<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._D_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _Ⰳx0D<ParseMode.Realized>>(
        true,
        new _Ⰳx0D<ParseMode.Realized>(
            this._0_1.Realize().RealizedValue,
this._D_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _Ⰳx0D<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

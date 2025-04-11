namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _Ⰳx20<TMode> : IAstNode<char, _Ⰳx20<ParseMode.Realized>>, IFromRealizedable<_Ⰳx20<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _Ⰳx20(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1)
        {
            this.__2_1 = _2_1;
            this.__0_1 = _0_1;
            this.realizationResult = new Future<IRealizationResult<char, _Ⰳx20<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _Ⰳx20(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, IFuture<IRealizationResult<char, _Ⰳx20<ParseMode.Realized>>> realizationResult)
        {
            this.__2_1 = Future.Create(() => _2_1);
            this.__0_1 = Future.Create(() => _0_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
        private IFuture<IRealizationResult<char, _Ⰳx20<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
        
        internal static _Ⰳx20<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var _2_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult));
var _0_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _2_1.Value.Realize())));
return new _Ⰳx20<ParseMode.Deferred>(_2_1, _0_1);
        }
        
        public _Ⰳx20<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _Ⰳx20<ParseMode.Deferred>(
        this.__2_1.Select(_ => _.Convert()),
this.__0_1.Select(_ => _.Convert()));
}
else
{
    return new _Ⰳx20<ParseMode.Deferred>(
        this.__2_1.Value.Convert(),
this.__0_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _Ⰳx20<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _Ⰳx20<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._0_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _Ⰳx20<ParseMode.Realized>>(
        true,
        new _Ⰳx20<ParseMode.Realized>(
            this._2_1.Realize().RealizedValue,
this._0_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _Ⰳx20<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

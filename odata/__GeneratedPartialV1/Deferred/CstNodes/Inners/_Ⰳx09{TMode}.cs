namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _Ⰳx09<TMode> : IAstNode<char, _Ⰳx09<ParseMode.Realized>>, IFromRealizedable<_Ⰳx09<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _Ⰳx09(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1)
        {
            this.__0_1 = _0_1;
            this.__9_1 = _9_1;
            this.realizationResult = new Future<IRealizationResult<char, _Ⰳx09<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _Ⰳx09(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1, IFuture<IRealizationResult<char, _Ⰳx09<ParseMode.Realized>>> realizationResult)
        {
            this.__0_1 = Future.Create(() => _0_1);
            this.__9_1 = Future.Create(() => _9_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
        private IFuture<IRealizationResult<char, _Ⰳx09<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get{
        return this.__0_1.Value;
        }
        }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get{
        return this.__9_1.Value;
        }
        }
        
        internal static _Ⰳx09<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var _0_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(previousNodeRealizationResult));
var _9_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _0_1.Value.Realize())));
return new _Ⰳx09<ParseMode.Deferred>(_0_1, _9_1);
        }
        
        public _Ⰳx09<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _Ⰳx09<ParseMode.Deferred>(
        this.__0_1.Select(_ => _.Convert()),
this.__9_1.Select(_ => _.Convert()));
}
else
{
    return new _Ⰳx09<ParseMode.Deferred>(
        this.__0_1.Value.Convert(),
this.__9_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _Ⰳx09<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _Ⰳx09<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._9_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _Ⰳx09<ParseMode.Realized>>(
        true,
        new _Ⰳx09<ParseMode.Realized>(
            this._0_1.Realize().RealizedValue,
this._9_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _Ⰳx09<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

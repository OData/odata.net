namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _Ⰳx22<TMode> : IAstNode<char, _Ⰳx22<ParseMode.Realized>>, IFromRealizedable<_Ⰳx22<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _Ⰳx22(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_2)
        {
            this.__2_1 = _2_1;
            this.__2_2 = _2_2;
            this.realizationResult = new Future<IRealizationResult<char, _Ⰳx22<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _Ⰳx22(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_2, IFuture<IRealizationResult<char, _Ⰳx22<ParseMode.Realized>>> realizationResult)
        {
            this.__2_1 = Future.Create(() => _2_1);
            this.__2_2 = Future.Create(() => _2_2);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_2 { get; }
        private IFuture<IRealizationResult<char, _Ⰳx22<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get{
        return this.__2_1.Value;
        }
        }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_2 { get{
        return this.__2_2.Value;
        }
        }
        
        internal static _Ⰳx22<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var _2_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult));
var _2_2 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _2_1.Value.Realize())));
return new _Ⰳx22<ParseMode.Deferred>(_2_1, _2_2);
        }
        
        public _Ⰳx22<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _Ⰳx22<ParseMode.Deferred>(
        this.__2_1.Select(_ => _.Convert()),
this.__2_2.Select(_ => _.Convert()));
}
else
{
    return new _Ⰳx22<ParseMode.Deferred>(
        this.__2_1.Value.Convert(),
this.__2_2.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _Ⰳx22<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _Ⰳx22<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._2_2.Realize();
if (output.Success)
{
    return new RealizationResult<char, _Ⰳx22<ParseMode.Realized>>(
        true,
        new _Ⰳx22<ParseMode.Realized>(
            this._2_1.Realize().RealizedValue,
this._2_2.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _Ⰳx22<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

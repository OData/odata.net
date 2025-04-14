namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _Ⰳx7F<TMode> : IAstNode<char, _Ⰳx7F<ParseMode.Realized>>, IFromRealizedable<_Ⰳx7F<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _Ⰳx7F(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1)
        {
            this.__7_1 = _7_1;
            this.__F_1 = _F_1;
            this.realizationResult = new Future<IRealizationResult<char, _Ⰳx7F<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _Ⰳx7F(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1, IFuture<IRealizationResult<char, _Ⰳx7F<ParseMode.Realized>>> realizationResult)
        {
            this.__7_1 = Future.Create(() => _7_1);
            this.__F_1 = Future.Create(() => _F_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
        private IFuture<IRealizationResult<char, _Ⰳx7F<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get{
        return this.__7_1.Value;
        }
        }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get{
        return this.__F_1.Value;
        }
        }
        
        internal static _Ⰳx7F<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var _7_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult));
var _F_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(Future.Create(() => _7_1.Value.Realize())));
return new _Ⰳx7F<ParseMode.Deferred>(_7_1, _F_1);
        }
        
        public _Ⰳx7F<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _Ⰳx7F<ParseMode.Deferred>(
        this.__7_1.Select(_ => _.Convert()),
this.__F_1.Select(_ => _.Convert()));
}
else
{
    return new _Ⰳx7F<ParseMode.Deferred>(
        this.__7_1.Value.Convert(),
this.__F_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _Ⰳx7F<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _Ⰳx7F<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._F_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _Ⰳx7F<ParseMode.Realized>>(
        true,
        new _Ⰳx7F<ParseMode.Realized>(
            this._7_1.Realize().RealizedValue,
this._F_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _Ⰳx7F<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

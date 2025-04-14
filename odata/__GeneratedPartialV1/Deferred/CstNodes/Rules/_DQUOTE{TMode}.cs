namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _DQUOTE<TMode> : IAstNode<char, _DQUOTE<ParseMode.Realized>>, IFromRealizedable<_DQUOTE<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _DQUOTE(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx22<TMode>> _Ⰳx22_1)
        {
            this.__Ⰳx22_1 = _Ⰳx22_1;
            this.realizationResult = new Future<IRealizationResult<char, _DQUOTE<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _DQUOTE(__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx22<TMode> _Ⰳx22_1, IFuture<IRealizationResult<char, _DQUOTE<ParseMode.Realized>>> realizationResult)
        {
            this.__Ⰳx22_1 = Future.Create(() => _Ⰳx22_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx22<TMode>> __Ⰳx22_1 { get; }
        private IFuture<IRealizationResult<char, _DQUOTE<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx22<TMode> _Ⰳx22_1 { get{
        return this.__Ⰳx22_1.Value;
        }
        }
        
        internal static _DQUOTE<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var _Ⰳx22_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx22.Create(previousNodeRealizationResult));
return new _DQUOTE<ParseMode.Deferred>(_Ⰳx22_1);
        }
        
        public _DQUOTE<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _DQUOTE<ParseMode.Deferred>(
        this.__Ⰳx22_1.Select(_ => _.Convert()));
}
else
{
    return new _DQUOTE<ParseMode.Deferred>(
        this.__Ⰳx22_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _DQUOTE<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _DQUOTE<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._Ⰳx22_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _DQUOTE<ParseMode.Realized>>(
        true,
        new _DQUOTE<ParseMode.Realized>(
            this._Ⰳx22_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _DQUOTE<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

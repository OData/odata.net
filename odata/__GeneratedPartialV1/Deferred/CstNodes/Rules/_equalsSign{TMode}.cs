namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _equalsSign<TMode> : IAstNode<char, _equalsSign<ParseMode.Realized>>, IFromRealizedable<_equalsSign<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _equalsSign(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx3Dʺ<TMode>> _ʺx3Dʺ_1)
        {
            this.__ʺx3Dʺ_1 = _ʺx3Dʺ_1;
            this.realizationResult = new Future<IRealizationResult<char, _equalsSign<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _equalsSign(__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx3Dʺ<TMode> _ʺx3Dʺ_1, IFuture<IRealizationResult<char, _equalsSign<ParseMode.Realized>>> realizationResult)
        {
            this.__ʺx3Dʺ_1 = Future.Create(() => _ʺx3Dʺ_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx3Dʺ<TMode>> __ʺx3Dʺ_1 { get; }
        private IFuture<IRealizationResult<char, _equalsSign<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx3Dʺ<TMode> _ʺx3Dʺ_1 { get{
        return this.__ʺx3Dʺ_1.Value;
        }
        }
        
        internal static _equalsSign<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var _ʺx3Dʺ_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx3Dʺ.Create(previousNodeRealizationResult));
return new _equalsSign<ParseMode.Deferred>(_ʺx3Dʺ_1);
        }
        
        public _equalsSign<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _equalsSign<ParseMode.Deferred>(
        this.__ʺx3Dʺ_1.Select(_ => _.Convert()));
}
else
{
    return new _equalsSign<ParseMode.Deferred>(
        this.__ʺx3Dʺ_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _equalsSign<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _equalsSign<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._ʺx3Dʺ_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _equalsSign<ParseMode.Realized>>(
        true,
        new _equalsSign<ParseMode.Realized>(
            this._ʺx3Dʺ_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _equalsSign<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

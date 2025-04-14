namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _slash<TMode> : IAstNode<char, _slash<ParseMode.Realized>>, IFromRealizedable<_slash<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _slash(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx2Fʺ<TMode>> _ʺx2Fʺ_1)
        {
            this.__ʺx2Fʺ_1 = _ʺx2Fʺ_1;
            this.realizationResult = new Future<IRealizationResult<char, _slash<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _slash(__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx2Fʺ<TMode> _ʺx2Fʺ_1, IFuture<IRealizationResult<char, _slash<ParseMode.Realized>>> realizationResult)
        {
            this.__ʺx2Fʺ_1 = Future.Create(() => _ʺx2Fʺ_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx2Fʺ<TMode>> __ʺx2Fʺ_1 { get; }
        private IFuture<IRealizationResult<char, _slash<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx2Fʺ<TMode> _ʺx2Fʺ_1 { get{
        return this.__ʺx2Fʺ_1.Value;
        }
        }
        
        internal static _slash<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var _ʺx2Fʺ_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx2Fʺ.Create(previousNodeRealizationResult));
return new _slash<ParseMode.Deferred>(_ʺx2Fʺ_1);
        }
        
        public _slash<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _slash<ParseMode.Deferred>(
        this.__ʺx2Fʺ_1.Select(_ => _.Convert()));
}
else
{
    return new _slash<ParseMode.Deferred>(
        this.__ʺx2Fʺ_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _slash<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _slash<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._ʺx2Fʺ_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _slash<ParseMode.Realized>>(
        true,
        new _slash<ParseMode.Realized>(
            this._ʺx2Fʺ_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _slash<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

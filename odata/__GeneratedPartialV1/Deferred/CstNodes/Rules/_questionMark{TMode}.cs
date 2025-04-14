namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _questionMark<TMode> : IAstNode<char, _questionMark<ParseMode.Realized>>, IFromRealizedable<_questionMark<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _questionMark(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx3Fʺ<TMode>> _ʺx3Fʺ_1)
        {
            this.__ʺx3Fʺ_1 = _ʺx3Fʺ_1;
            this.realizationResult = new Future<IRealizationResult<char, _questionMark<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _questionMark(__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx3Fʺ<TMode> _ʺx3Fʺ_1, IFuture<IRealizationResult<char, _questionMark<ParseMode.Realized>>> realizationResult)
        {
            this.__ʺx3Fʺ_1 = Future.Create(() => _ʺx3Fʺ_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx3Fʺ<TMode>> __ʺx3Fʺ_1 { get; }
        private IFuture<IRealizationResult<char, _questionMark<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx3Fʺ<TMode> _ʺx3Fʺ_1 { get{
        return this.__ʺx3Fʺ_1.Value;
        }
        }
        
        internal static _questionMark<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var _ʺx3Fʺ_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx3Fʺ.Create(previousNodeRealizationResult));
return new _questionMark<ParseMode.Deferred>(_ʺx3Fʺ_1);
        }
        
        public _questionMark<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _questionMark<ParseMode.Deferred>(
        this.__ʺx3Fʺ_1.Select(_ => _.Convert()));
}
else
{
    return new _questionMark<ParseMode.Deferred>(
        this.__ʺx3Fʺ_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _questionMark<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _questionMark<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._ʺx3Fʺ_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _questionMark<ParseMode.Realized>>(
        true,
        new _questionMark<ParseMode.Realized>(
            this._ʺx3Fʺ_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _questionMark<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

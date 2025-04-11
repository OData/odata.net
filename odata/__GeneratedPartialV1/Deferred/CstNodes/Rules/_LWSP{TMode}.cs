namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _LWSP<TMode> : IAstNode<char, _LWSP<ParseMode.Realized>>, IFromRealizedable<_LWSP<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _LWSP(IFuture<CombinatorParsingV3.Many<Inners._ⲤWSPⳆCRLF_WSPↃ<ParseMode.Deferred>, Inners._ⲤWSPⳆCRLF_WSPↃ<ParseMode.Realized>, TMode>> _ⲤWSPⳆCRLF_WSPↃ_1)
        {
            this.__ⲤWSPⳆCRLF_WSPↃ_1 = _ⲤWSPⳆCRLF_WSPↃ_1;
            this.realizationResult = new Future<IRealizationResult<char, _LWSP<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _LWSP(CombinatorParsingV3.Many<Inners._ⲤWSPⳆCRLF_WSPↃ<ParseMode.Deferred>, Inners._ⲤWSPⳆCRLF_WSPↃ<ParseMode.Realized>, TMode> _ⲤWSPⳆCRLF_WSPↃ_1, IFuture<IRealizationResult<char, _LWSP<ParseMode.Realized>>> realizationResult)
        {
            this.__ⲤWSPⳆCRLF_WSPↃ_1 = Future.Create(() => _ⲤWSPⳆCRLF_WSPↃ_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<CombinatorParsingV3.Many<Inners._ⲤWSPⳆCRLF_WSPↃ<ParseMode.Deferred>, Inners._ⲤWSPⳆCRLF_WSPↃ<ParseMode.Realized>, TMode>> __ⲤWSPⳆCRLF_WSPↃ_1 { get; }
        private IFuture<IRealizationResult<char, _LWSP<ParseMode.Realized>>> realizationResult { get; }
        public CombinatorParsingV3.Many<Inners._ⲤWSPⳆCRLF_WSPↃ<ParseMode.Deferred>, Inners._ⲤWSPⳆCRLF_WSPↃ<ParseMode.Realized>, TMode> _ⲤWSPⳆCRLF_WSPↃ_1 { get; }
        
        internal static _LWSP<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var _ⲤWSPⳆCRLF_WSPↃ_1 = Future.Create(() => CombinatorParsingV3.Many.Create<Inners._ⲤWSPⳆCRLF_WSPↃ<ParseMode.Deferred>, Inners._ⲤWSPⳆCRLF_WSPↃ<ParseMode.Realized>>(input => Inners._ⲤWSPⳆCRLF_WSPↃ.Create(input), previousNodeRealizationResult));
return new _LWSP<ParseMode.Deferred>(_ⲤWSPⳆCRLF_WSPↃ_1);
        }
        
        public _LWSP<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _LWSP<ParseMode.Deferred>(
        this.__ⲤWSPⳆCRLF_WSPↃ_1.Select(_ => _.Convert()));
}
else
{
    return new _LWSP<ParseMode.Deferred>(
        this.__ⲤWSPⳆCRLF_WSPↃ_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _LWSP<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _LWSP<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._ⲤWSPⳆCRLF_WSPↃ_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _LWSP<ParseMode.Realized>>(
        true,
        new _LWSP<ParseMode.Realized>(
            this._ⲤWSPⳆCRLF_WSPↃ_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _LWSP<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

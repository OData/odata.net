namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _ⲤWSPⳆCRLF_WSPↃ<TMode> : IAstNode<char, _ⲤWSPⳆCRLF_WSPↃ<ParseMode.Realized>>, IFromRealizedable<_ⲤWSPⳆCRLF_WSPↃ<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _ⲤWSPⳆCRLF_WSPↃ(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._WSPⳆCRLF_WSP<TMode>> _WSPⳆCRLF_WSP_1)
        {
            this.__WSPⳆCRLF_WSP_1 = _WSPⳆCRLF_WSP_1;
            this.realizationResult = new Future<IRealizationResult<char, _ⲤWSPⳆCRLF_WSPↃ<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _ⲤWSPⳆCRLF_WSPↃ(__GeneratedPartialV1.Deferred.CstNodes.Inners._WSPⳆCRLF_WSP<TMode> _WSPⳆCRLF_WSP_1, IFuture<IRealizationResult<char, _ⲤWSPⳆCRLF_WSPↃ<ParseMode.Realized>>> realizationResult)
        {
            this.__WSPⳆCRLF_WSP_1 = Future.Create(() => _WSPⳆCRLF_WSP_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._WSPⳆCRLF_WSP<TMode>> __WSPⳆCRLF_WSP_1 { get; }
        private IFuture<IRealizationResult<char, _ⲤWSPⳆCRLF_WSPↃ<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._WSPⳆCRLF_WSP<TMode> _WSPⳆCRLF_WSP_1 { get{
        return this.__WSPⳆCRLF_WSP_1.Value;
        }
        }
        
        internal static _ⲤWSPⳆCRLF_WSPↃ<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var _WSPⳆCRLF_WSP_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Inners._WSPⳆCRLF_WSP.Create(previousNodeRealizationResult));
return new _ⲤWSPⳆCRLF_WSPↃ<ParseMode.Deferred>(_WSPⳆCRLF_WSP_1);
        }
        
        public _ⲤWSPⳆCRLF_WSPↃ<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _ⲤWSPⳆCRLF_WSPↃ<ParseMode.Deferred>(
        this.__WSPⳆCRLF_WSP_1.Select(_ => _.Convert()));
}
else
{
    return new _ⲤWSPⳆCRLF_WSPↃ<ParseMode.Deferred>(
        this.__WSPⳆCRLF_WSP_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _ⲤWSPⳆCRLF_WSPↃ<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _ⲤWSPⳆCRLF_WSPↃ<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._WSPⳆCRLF_WSP_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _ⲤWSPⳆCRLF_WSPↃ<ParseMode.Realized>>(
        true,
        new _ⲤWSPⳆCRLF_WSPↃ<ParseMode.Realized>(
            this._WSPⳆCRLF_WSP_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _ⲤWSPⳆCRLF_WSPↃ<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

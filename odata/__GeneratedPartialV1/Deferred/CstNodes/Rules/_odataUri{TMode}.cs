namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _odataUri<TMode> : IAstNode<char, _odataUri<ParseMode.Realized>>, IFromRealizedable<_odataUri<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _odataUri(IFuture<CombinatorParsingV3.AtLeastOne<__GeneratedPartialV1.Deferred.CstNodes.Rules._segment<ParseMode.Deferred>, __GeneratedPartialV1.Deferred.CstNodes.Rules._segment<ParseMode.Realized>, TMode>> _segment_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Rules._questionMark<TMode>> _questionMark_1, IFuture<CombinatorParsingV3.Many<__GeneratedPartialV1.Deferred.CstNodes.Rules._queryOption<ParseMode.Deferred>, __GeneratedPartialV1.Deferred.CstNodes.Rules._queryOption<ParseMode.Realized>, TMode>> _queryOption_1)
        {
            this.__segment_1 = _segment_1;
            this.__questionMark_1 = _questionMark_1;
            this.__queryOption_1 = _queryOption_1;
            this.realizationResult = new Future<IRealizationResult<char, _odataUri<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _odataUri(CombinatorParsingV3.AtLeastOne<__GeneratedPartialV1.Deferred.CstNodes.Rules._segment<ParseMode.Deferred>, __GeneratedPartialV1.Deferred.CstNodes.Rules._segment<ParseMode.Realized>, TMode> _segment_1, __GeneratedPartialV1.Deferred.CstNodes.Rules._questionMark<TMode> _questionMark_1, CombinatorParsingV3.Many<__GeneratedPartialV1.Deferred.CstNodes.Rules._queryOption<ParseMode.Deferred>, __GeneratedPartialV1.Deferred.CstNodes.Rules._queryOption<ParseMode.Realized>, TMode> _queryOption_1, IFuture<IRealizationResult<char, _odataUri<ParseMode.Realized>>> realizationResult)
        {
            this.__segment_1 = Future.Create(() => _segment_1);
            this.__questionMark_1 = Future.Create(() => _questionMark_1);
            this.__queryOption_1 = Future.Create(() => _queryOption_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<CombinatorParsingV3.AtLeastOne<__GeneratedPartialV1.Deferred.CstNodes.Rules._segment<ParseMode.Deferred>, __GeneratedPartialV1.Deferred.CstNodes.Rules._segment<ParseMode.Realized>, TMode>> __segment_1 { get; }
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Rules._questionMark<TMode>> __questionMark_1 { get; }
        private IFuture<CombinatorParsingV3.Many<__GeneratedPartialV1.Deferred.CstNodes.Rules._queryOption<ParseMode.Deferred>, __GeneratedPartialV1.Deferred.CstNodes.Rules._queryOption<ParseMode.Realized>, TMode>> __queryOption_1 { get; }
        private IFuture<IRealizationResult<char, _odataUri<ParseMode.Realized>>> realizationResult { get; }
        public CombinatorParsingV3.AtLeastOne<__GeneratedPartialV1.Deferred.CstNodes.Rules._segment<ParseMode.Deferred>, __GeneratedPartialV1.Deferred.CstNodes.Rules._segment<ParseMode.Realized>, TMode> _segment_1 { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Rules._questionMark<TMode> _questionMark_1 { get; }
        public CombinatorParsingV3.Many<__GeneratedPartialV1.Deferred.CstNodes.Rules._queryOption<ParseMode.Deferred>, __GeneratedPartialV1.Deferred.CstNodes.Rules._queryOption<ParseMode.Realized>, TMode> _queryOption_1 { get; }
        
        public _odataUri<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _odataUri<ParseMode.Deferred>(
        this.__segment_1.Select(_ => _.Convert()),
this.__questionMark_1.Select(_ => _.Convert()),
this.__queryOption_1.Select(_ => _.Convert()));
}
else
{
    return new _odataUri<ParseMode.Deferred>(
        this.__segment_1.Value.Convert(),
this.__questionMark_1.Value.Convert(),
this.__queryOption_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _odataUri<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _odataUri<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._queryOption_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _odataUri<ParseMode.Realized>>(
        true,
        new _odataUri<ParseMode.Realized>(
            this._segment_1.Realize().RealizedValue,
this._questionMark_1.Realize().RealizedValue,
this._queryOption_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _odataUri<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _optionValue<TMode> : IAstNode<char, _optionValue<ParseMode.Realized>>, IFromRealizedable<_optionValue<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _optionValue(IFuture<CombinatorParsingV3.Many<__GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Deferred>, __GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Realized>, TMode>> _alphaNumeric_1)
        {
            this.__alphaNumeric_1 = _alphaNumeric_1;
            this.realizationResult = new Future<IRealizationResult<char, _optionValue<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _optionValue(CombinatorParsingV3.Many<__GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Deferred>, __GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Realized>, TMode> _alphaNumeric_1, IFuture<IRealizationResult<char, _optionValue<ParseMode.Realized>>> realizationResult)
        {
            this.__alphaNumeric_1 = Future.Create(() => _alphaNumeric_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<CombinatorParsingV3.Many<__GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Deferred>, __GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Realized>, TMode>> __alphaNumeric_1 { get; }
        private IFuture<IRealizationResult<char, _optionValue<ParseMode.Realized>>> realizationResult { get; }
        public CombinatorParsingV3.Many<__GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Deferred>, __GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Realized>, TMode> _alphaNumeric_1 { get; }
        
        public _optionValue<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _optionValue<ParseMode.Deferred>(
        this.__alphaNumeric_1.Select(_ => _.Convert()));
}
else
{
    return new _optionValue<ParseMode.Deferred>(
        this.__alphaNumeric_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _optionValue<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _optionValue<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._alphaNumeric_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _optionValue<ParseMode.Realized>>(
        true,
        new _optionValue<ParseMode.Realized>(
            this._alphaNumeric_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _optionValue<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

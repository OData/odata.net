namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _optionName<TMode> : IAstNode<char, _optionName<ParseMode.Realized>>, IFromRealizedable<_optionName<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _optionName(IFuture<CombinatorParsingV3.AtLeastOne<__GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Deferred>, __GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Realized>, TMode>> _alphaNumeric_1)
        {
            this.__alphaNumeric_1 = _alphaNumeric_1;
            this.realizationResult = new Future<IRealizationResult<char, _optionName<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _optionName(CombinatorParsingV3.AtLeastOne<__GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Deferred>, __GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Realized>, TMode> _alphaNumeric_1, IFuture<IRealizationResult<char, _optionName<ParseMode.Realized>>> realizationResult)
        {
            this.__alphaNumeric_1 = Future.Create(() => _alphaNumeric_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<CombinatorParsingV3.AtLeastOne<__GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Deferred>, __GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Realized>, TMode>> __alphaNumeric_1 { get; }
        private IFuture<IRealizationResult<char, _optionName<ParseMode.Realized>>> realizationResult { get; }
        public CombinatorParsingV3.AtLeastOne<__GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Deferred>, __GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Realized>, TMode> _alphaNumeric_1 { get{
        return this.__alphaNumeric_1.Value;
        }
        }
        
        internal static _optionName<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var _alphaNumeric_1 = Future.Create(() => CombinatorParsingV3.AtLeastOne.Create<__GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Deferred>, __GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Realized>>(previousNodeRealizationResult, input => __GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric.Create(input)));
return new _optionName<ParseMode.Deferred>(_alphaNumeric_1);
        }
        
        public _optionName<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _optionName<ParseMode.Deferred>(
        this.__alphaNumeric_1.Select(_ => _.Convert()));
}
else
{
    return new _optionName<ParseMode.Deferred>(
        this.__alphaNumeric_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _optionName<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _optionName<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._alphaNumeric_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _optionName<ParseMode.Realized>>(
        true,
        new _optionName<ParseMode.Realized>(
            this._alphaNumeric_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _optionName<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

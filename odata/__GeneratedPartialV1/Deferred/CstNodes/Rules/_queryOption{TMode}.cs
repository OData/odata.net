namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _queryOption<TMode> : IAstNode<char, _queryOption<ParseMode.Realized>>, IFromRealizedable<_queryOption<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _queryOption(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Rules._optionName<TMode>> _optionName_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Rules._equalsSign<TMode>> _equalsSign_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Rules._optionValue<TMode>> _optionValue_1)
        {
            this.__optionName_1 = _optionName_1;
            this.__equalsSign_1 = _equalsSign_1;
            this.__optionValue_1 = _optionValue_1;
            this.realizationResult = new Future<IRealizationResult<char, _queryOption<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _queryOption(__GeneratedPartialV1.Deferred.CstNodes.Rules._optionName<TMode> _optionName_1, __GeneratedPartialV1.Deferred.CstNodes.Rules._equalsSign<TMode> _equalsSign_1, __GeneratedPartialV1.Deferred.CstNodes.Rules._optionValue<TMode> _optionValue_1, IFuture<IRealizationResult<char, _queryOption<ParseMode.Realized>>> realizationResult)
        {
            this.__optionName_1 = Future.Create(() => _optionName_1);
            this.__equalsSign_1 = Future.Create(() => _equalsSign_1);
            this.__optionValue_1 = Future.Create(() => _optionValue_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Rules._optionName<TMode>> __optionName_1 { get; }
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Rules._equalsSign<TMode>> __equalsSign_1 { get; }
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Rules._optionValue<TMode>> __optionValue_1 { get; }
        private IFuture<IRealizationResult<char, _queryOption<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Rules._optionName<TMode> _optionName_1 { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Rules._equalsSign<TMode> _equalsSign_1 { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Rules._optionValue<TMode> _optionValue_1 { get; }
        
        public _queryOption<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _queryOption<ParseMode.Deferred>(
        this.__optionName_1.Select(_ => _.Convert()),
this.__equalsSign_1.Select(_ => _.Convert()),
this.__optionValue_1.Select(_ => _.Convert()));
}
else
{
    return new _queryOption<ParseMode.Deferred>(
        this.__optionName_1.Value.Convert(),
this.__equalsSign_1.Value.Convert(),
this.__optionValue_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _queryOption<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _queryOption<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._optionValue_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _queryOption<ParseMode.Realized>>(
        true,
        new _queryOption<ParseMode.Realized>(
            this._optionName_1.Realize().RealizedValue,
this._equalsSign_1.Realize().RealizedValue,
this._optionValue_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _queryOption<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

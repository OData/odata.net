namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _segment<TMode> : IAstNode<char, _segment<ParseMode.Realized>>, IFromRealizedable<_segment<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _segment(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Rules._slash<TMode>> _slash_1, IFuture<CombinatorParsingV3.AtLeastOne<__GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Deferred>, __GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Realized>, TMode>> _alphaNumeric_1)
        {
            this.__slash_1 = _slash_1;
            this.__alphaNumeric_1 = _alphaNumeric_1;
            this.realizationResult = new Future<IRealizationResult<char, _segment<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _segment(__GeneratedPartialV1.Deferred.CstNodes.Rules._slash<TMode> _slash_1, CombinatorParsingV3.AtLeastOne<__GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Deferred>, __GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Realized>, TMode> _alphaNumeric_1, IFuture<IRealizationResult<char, _segment<ParseMode.Realized>>> realizationResult)
        {
            this.__slash_1 = Future.Create(() => _slash_1);
            this.__alphaNumeric_1 = Future.Create(() => _alphaNumeric_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Rules._slash<TMode>> __slash_1 { get; }
        private IFuture<CombinatorParsingV3.AtLeastOne<__GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Deferred>, __GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Realized>, TMode>> __alphaNumeric_1 { get; }
        private IFuture<IRealizationResult<char, _segment<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Rules._slash<TMode> _slash_1 { get; }
        public CombinatorParsingV3.AtLeastOne<__GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Deferred>, __GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Realized>, TMode> _alphaNumeric_1 { get; }
        
        internal static _segment<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var _slash_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Rules._slash.Create(previousNodeRealizationResult));
var _alphaNumeric_1 = Future.Create(() => CombinatorParsingV3.AtLeastOne.Create<__GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Deferred>, __GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Realized>>(Future.Create(() => _slash_1.Value.Realize()), input => __GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric.Create(input)));
return new _segment<ParseMode.Deferred>(_slash_1, _alphaNumeric_1);
        }
        
        public _segment<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _segment<ParseMode.Deferred>(
        this.__slash_1.Select(_ => _.Convert()),
this.__alphaNumeric_1.Select(_ => _.Convert()));
}
else
{
    return new _segment<ParseMode.Deferred>(
        this.__slash_1.Value.Convert(),
this.__alphaNumeric_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _segment<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _segment<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._alphaNumeric_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _segment<ParseMode.Realized>>(
        true,
        new _segment<ParseMode.Realized>(
            this._slash_1.Realize().RealizedValue,
this._alphaNumeric_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _segment<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

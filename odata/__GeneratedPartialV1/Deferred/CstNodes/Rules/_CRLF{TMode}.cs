namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _CRLF<TMode> : IAstNode<char, _CRLF<ParseMode.Realized>>, IFromRealizedable<_CRLF<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _CRLF(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Rules._CR<TMode>> _CR_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Rules._LF<TMode>> _LF_1)
        {
            this.__CR_1 = _CR_1;
            this.__LF_1 = _LF_1;
            this.realizationResult = new Future<IRealizationResult<char, _CRLF<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _CRLF(__GeneratedPartialV1.Deferred.CstNodes.Rules._CR<TMode> _CR_1, __GeneratedPartialV1.Deferred.CstNodes.Rules._LF<TMode> _LF_1, IFuture<IRealizationResult<char, _CRLF<ParseMode.Realized>>> realizationResult)
        {
            this.__CR_1 = Future.Create(() => _CR_1);
            this.__LF_1 = Future.Create(() => _LF_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Rules._CR<TMode>> __CR_1 { get; }
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Rules._LF<TMode>> __LF_1 { get; }
        private IFuture<IRealizationResult<char, _CRLF<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Rules._CR<TMode> _CR_1 { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Rules._LF<TMode> _LF_1 { get; }
        
        internal static _CRLF<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var _CR_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Rules._CR.Create(previousNodeRealizationResult));
var _LF_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Rules._LF.Create(Future.Create(() => _CR_1.Value.Realize())));
return new _CRLF<ParseMode.Deferred>(_CR_1, _LF_1);
        }
        
        public _CRLF<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _CRLF<ParseMode.Deferred>(
        this.__CR_1.Select(_ => _.Convert()),
this.__LF_1.Select(_ => _.Convert()));
}
else
{
    return new _CRLF<ParseMode.Deferred>(
        this.__CR_1.Value.Convert(),
this.__LF_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _CRLF<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _CRLF<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._LF_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _CRLF<ParseMode.Realized>>(
        true,
        new _CRLF<ParseMode.Realized>(
            this._CR_1.Realize().RealizedValue,
this._LF_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _CRLF<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

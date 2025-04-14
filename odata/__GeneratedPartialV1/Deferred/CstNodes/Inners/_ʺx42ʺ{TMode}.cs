namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _ʺx42ʺ<TMode> : IAstNode<char, _ʺx42ʺ<ParseMode.Realized>>, IFromRealizedable<_ʺx42ʺ<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _ʺx42ʺ(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._x42<TMode>> _x42_1)
        {
            this.__x42_1 = _x42_1;
            this.realizationResult = new Future<IRealizationResult<char, _ʺx42ʺ<ParseMode.Realized>>>(this.RealizeImpl);
        }
        private _ʺx42ʺ(__GeneratedPartialV1.Deferred.CstNodes.Inners._x42<TMode> _x42_1, IFuture<IRealizationResult<char, _ʺx42ʺ<ParseMode.Realized>>> realizationResult)
        {
            this.__x42_1 = Future.Create(() => _x42_1);
            this.realizationResult = realizationResult;
        }
        
        private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._x42<TMode>> __x42_1 { get; }
        private IFuture<IRealizationResult<char, _ʺx42ʺ<ParseMode.Realized>>> realizationResult { get; }
        public __GeneratedPartialV1.Deferred.CstNodes.Inners._x42<TMode> _x42_1 { get{
        return this.__x42_1.Value;
        }
        }
        
        internal static _ʺx42ʺ<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            var _x42_1 = Future.Create(() => __GeneratedPartialV1.Deferred.CstNodes.Inners._x42.Create(previousNodeRealizationResult));
return new _ʺx42ʺ<ParseMode.Deferred>(_x42_1);
        }
        
        public _ʺx42ʺ<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _ʺx42ʺ<ParseMode.Deferred>(
        this.__x42_1.Select(_ => _.Convert()));
}
else
{
    return new _ʺx42ʺ<ParseMode.Deferred>(
        this.__x42_1.Value.Convert(),
        this.realizationResult);
}
        }
        
        public IRealizationResult<char, _ʺx42ʺ<ParseMode.Realized>> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _ʺx42ʺ<ParseMode.Realized>> RealizeImpl()
        {
            var output = this._x42_1.Realize();
if (output.Success)
{
    return new RealizationResult<char, _ʺx42ʺ<ParseMode.Realized>>(
        true,
        new _ʺx42ʺ<ParseMode.Realized>(
            this._x42_1.Realize().RealizedValue,
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, _ʺx42ʺ<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
        }
    }
    
}

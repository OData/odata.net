namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _C<TMode> : IAstNode<char, _C<ParseMode.Realized>>, IFromRealizedable<_C<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _C(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            if (typeof(TMode) != typeof(ParseMode.Deferred))
            {
                throw new ArgumentException("TODO");
            }
            
            this.previousNodeRealizationResult = previousNodeRealizationResult;
            
            this.realizationResult = new Future<IRealizationResult<char, _C<ParseMode.Realized>>>(() => this.RealizeImpl());
        }
        private _C(IFuture<IRealizationResult<char, _C<ParseMode.Realized>>> realizationResult)
        {
            if (typeof(TMode) != typeof(ParseMode.Deferred))
            {
                throw new ArgumentException("TODO");
            }
            
            this.realizationResult = realizationResult;
        }
        
        private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
        private IFuture<IRealizationResult<char, _C<ParseMode.Realized>>> realizationResult { get; }
        
        internal static _C<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return new _C<ParseMode.Deferred>(previousNodeRealizationResult);
        }
        
        public _C<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _C<ParseMode.Deferred>(this.previousNodeRealizationResult);
}
else
{
    return new _C<ParseMode.Deferred>(this.realizationResult);
}
        }
        
        public IRealizationResult<char, _C<ParseMode.Realized>> Realize()
        {
            return realizationResult.Value;
        }
        
        private IRealizationResult<char, _C<ParseMode.Realized>> RealizeImpl()
        {
            var output = this.previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _C<ParseMode.Realized>>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    return new RealizationResult<char, _C<ParseMode.Realized>>(false, default, output.RemainingTokens);
}

if (input.Current == '/') //// TODO
{
    return new RealizationResult<char, _C<ParseMode.Realized>>(
        true,
        new _C<ParseMode.Realized>(this.realizationResult),
        input.Next());
}
else
{
    return new RealizationResult<char, _C<ParseMode.Realized>>(false, default, input);
}
        }
    }
    
}

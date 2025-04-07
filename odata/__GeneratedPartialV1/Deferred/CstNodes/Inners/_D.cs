namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _D<TMode> : IAstNode<char, _D<ParseMode.Realized>>, IFromRealizedable<_D<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _D(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            if (typeof(TMode) != typeof(ParseMode.Deferred))
            {
                throw new ArgumentException("TODO");
            }
            
            this.previousNodeRealizationResult = previousNodeRealizationResult;
            
            this.realizationResult = new Future<IRealizationResult<char, _D<ParseMode.Realized>>>(() => this.RealizeImpl());
        }
        private _D(IFuture<IRealizationResult<char, _D<ParseMode.Realized>>> realizationResult)
        {
            if (typeof(TMode) != typeof(ParseMode.Deferred))
            {
                throw new ArgumentException("TODO");
            }
            
            this.realizationResult = realizationResult;
        }
        
        private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
        private IFuture<IRealizationResult<char, _D<ParseMode.Realized>>> realizationResult { get; }
        
        internal static _D<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return new _D<ParseMode.Deferred>(previousNodeRealizationResult);
        }
        
        public _D<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _D<ParseMode.Deferred>(this.previousNodeRealizationResult);
}
else
{
    return new _D<ParseMode.Deferred>(this.realizationResult);
}
        }
        
        public IRealizationResult<char, _D<ParseMode.Realized>> Realize()
        {
            return realizationResult.Value;
        }
        
        private IRealizationResult<char, _D<ParseMode.Realized>> RealizeImpl()
        {
            var output = this.previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _D<ParseMode.Realized>>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    return new RealizationResult<char, _D<ParseMode.Realized>>(false, default, output.RemainingTokens);
}

if (input.Current == '/') //// TODO
{
    return new RealizationResult<char, _D<ParseMode.Realized>>(
        true,
        new _D<ParseMode.Realized>(this.realizationResult),
        input.Next());
}
else
{
    return new RealizationResult<char, _D<ParseMode.Realized>>(false, default, input);
}
        }
    }
    
}

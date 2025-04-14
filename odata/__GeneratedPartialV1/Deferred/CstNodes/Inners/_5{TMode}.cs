namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _5<TMode> : IAstNode<char, _5<ParseMode.Realized>>, IFromRealizedable<_5<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _5(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            if (typeof(TMode) != typeof(ParseMode.Deferred))
            {
                throw new ArgumentException("TODO");
            }
            
            this.previousNodeRealizationResult = previousNodeRealizationResult;
            
            this.realizationResult = new Future<IRealizationResult<char, _5<ParseMode.Realized>>>(() => this.RealizeImpl());
        }
        private _5(IFuture<IRealizationResult<char, _5<ParseMode.Realized>>> realizationResult)
        {
            if (typeof(TMode) != typeof(ParseMode.Realized))
            {
                throw new ArgumentException("TODO");
            }
            
            this.realizationResult = realizationResult;
        }
        
        private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
        private IFuture<IRealizationResult<char, _5<ParseMode.Realized>>> realizationResult { get; }
        
        internal static _5<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return new _5<ParseMode.Deferred>(previousNodeRealizationResult);
        }
        
        public _5<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _5<ParseMode.Deferred>(this.previousNodeRealizationResult);
}
else
{
    return new _5<ParseMode.Deferred>(this.realizationResult);
}
        }
        
        public IRealizationResult<char, _5<ParseMode.Realized>> Realize()
        {
            return realizationResult.Value;
        }
        
        private IRealizationResult<char, _5<ParseMode.Realized>> RealizeImpl()
        {
            var output = this.previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _5<ParseMode.Realized>>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    return new RealizationResult<char, _5<ParseMode.Realized>>(false, default, output.RemainingTokens);
}

if (input.Current == '/') //// TODO this whole section is wrong to do
{
    return new RealizationResult<char, _5<ParseMode.Realized>>(
        true,
        new _5<ParseMode.Realized>(this.realizationResult),
        input.Next());
}
else
{
    return new RealizationResult<char, _5<ParseMode.Realized>>(false, default, input);
}
        }
    }
    
}

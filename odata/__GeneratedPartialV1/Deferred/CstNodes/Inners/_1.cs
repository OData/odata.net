namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _1<TMode> : IAstNode<char, _1<ParseMode.Realized>>, IFromRealizedable<_1<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _1(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            if (typeof(TMode) != typeof(ParseMode.Deferred))
            {
                throw new ArgumentException("TODO");
            }
            
            this.previousNodeRealizationResult = previousNodeRealizationResult;
            
            this.realizationResult = new Future<IRealizationResult<char, _1<ParseMode.Realized>>>(() => this.RealizeImpl());
        }
        private _1(IFuture<IRealizationResult<char, _1<ParseMode.Realized>>> realizationResult)
        {
            if (typeof(TMode) != typeof(ParseMode.Deferred))
            {
                throw new ArgumentException("TODO");
            }
            
            this.realizationResult = realizationResult;
        }
        
        private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
        private IFuture<IRealizationResult<char, _1<ParseMode.Realized>>> realizationResult { get; }
        
        internal static _1<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return new _1<ParseMode.Deferred>(previousNodeRealizationResult);
        }
        
        public _1<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _1<ParseMode.Deferred>(this.previousNodeRealizationResult);
}
else
{
    return new _1<ParseMode.Deferred>(this.realizationResult);
}
        }
        
        public IRealizationResult<char, _1<ParseMode.Realized>> Realize()
        {
            return realizationResult.Value;
        }
        
        private IRealizationResult<char, _1<ParseMode.Realized>> RealizeImpl()
        {
            var output = this.previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _1<ParseMode.Realized>>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    return new RealizationResult<char, _1<ParseMode.Realized>>(false, default, output.RemainingTokens);
}

if (input.Current == '/') //// TODO
{
    return new RealizationResult<char, _1<ParseMode.Realized>>(
        true,
        new _1<ParseMode.Realized>(this.realizationResult),
        input.Next());
}
else
{
    return new RealizationResult<char, _1<ParseMode.Realized>>(false, default, input);
}
        }
    }
    
}

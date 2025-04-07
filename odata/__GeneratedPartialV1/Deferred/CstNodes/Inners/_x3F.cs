namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _x3F<TMode> : IAstNode<char, _x3F<ParseMode.Realized>>, IFromRealizedable<_x3F<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _x3F(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            if (typeof(TMode) != typeof(ParseMode.Deferred))
            {
                throw new ArgumentException("TODO");
            }
            
            this.previousNodeRealizationResult = previousNodeRealizationResult;
            
            this.realizationResult = new Future<IRealizationResult<char, _x3F<ParseMode.Realized>>>(() => this.RealizeImpl());
        }
        private _x3F(IFuture<IRealizationResult<char, _x3F<ParseMode.Realized>>> realizationResult)
        {
            if (typeof(TMode) != typeof(ParseMode.Deferred))
            {
                throw new ArgumentException("TODO");
            }
            
            this.realizationResult = realizationResult;
        }
        
        private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
        private IFuture<IRealizationResult<char, _x3F<ParseMode.Realized>>> realizationResult { get; }
        
        internal static _x3F<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return new _x3F<ParseMode.Deferred>(previousNodeRealizationResult);
        }
        
        public _x3F<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _x3F<ParseMode.Deferred>(this.previousNodeRealizationResult);
}
else
{
    return new _x3F<ParseMode.Deferred>(this.realizationResult);
}
        }
        
        public IRealizationResult<char, _x3F<ParseMode.Realized>> Realize()
        {
            return realizationResult.Value;
        }
        
        private IRealizationResult<char, _x3F<ParseMode.Realized>> RealizeImpl()
        {
            var output = this.previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _x3F<ParseMode.Realized>>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    return new RealizationResult<char, _x3F<ParseMode.Realized>>(false, default, output.RemainingTokens);
}

if (input.Current == '/') //// TODO
{
    return new RealizationResult<char, _x3F<ParseMode.Realized>>(
        true,
        new _x3F<ParseMode.Realized>(this.realizationResult),
        input.Next());
}
else
{
    return new RealizationResult<char, _x3F<ParseMode.Realized>>(false, default, input);
}
        }
    }
    
}

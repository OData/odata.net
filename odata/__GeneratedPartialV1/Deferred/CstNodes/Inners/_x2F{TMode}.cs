namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _x2F<TMode> : IAstNode<char, _x2F<ParseMode.Realized>>, IFromRealizedable<_x2F<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _x2F(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            if (typeof(TMode) != typeof(ParseMode.Deferred))
            {
                throw new ArgumentException("TODO");
            }
            
            this.previousNodeRealizationResult = previousNodeRealizationResult;
            
            this.realizationResult = new Future<IRealizationResult<char, _x2F<ParseMode.Realized>>>(() => this.RealizeImpl());
        }
        private _x2F(IFuture<IRealizationResult<char, _x2F<ParseMode.Realized>>> realizationResult)
        {
            if (typeof(TMode) != typeof(ParseMode.Realized))
            {
                throw new ArgumentException("TODO");
            }
            
            this.realizationResult = realizationResult;
        }
        
        private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
        private IFuture<IRealizationResult<char, _x2F<ParseMode.Realized>>> realizationResult { get; }
        
        internal static _x2F<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return new _x2F<ParseMode.Deferred>(previousNodeRealizationResult);
        }
        
        public _x2F<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _x2F<ParseMode.Deferred>(this.previousNodeRealizationResult);
}
else
{
    return new _x2F<ParseMode.Deferred>(this.realizationResult);
}
        }
        
        public IRealizationResult<char, _x2F<ParseMode.Realized>> Realize()
        {
            return realizationResult.Value;
        }
        
        private IRealizationResult<char, _x2F<ParseMode.Realized>> RealizeImpl()
        {
            var output = this.previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _x2F<ParseMode.Realized>>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    return new RealizationResult<char, _x2F<ParseMode.Realized>>(false, default, output.RemainingTokens);
}

if (input.Current == '/')
{
    return new RealizationResult<char, _x2F<ParseMode.Realized>>(
        true,
        new _x2F<ParseMode.Realized>(this.realizationResult),
        input.Next());
}
else
{
    return new RealizationResult<char, _x2F<ParseMode.Realized>>(false, default, input);
}
        }
    }
    
}

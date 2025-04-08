namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _x46<TMode> : IAstNode<char, _x46<ParseMode.Realized>>, IFromRealizedable<_x46<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _x46(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            if (typeof(TMode) != typeof(ParseMode.Deferred))
            {
                throw new ArgumentException("TODO");
            }
            
            this.previousNodeRealizationResult = previousNodeRealizationResult;
            
            this.realizationResult = new Future<IRealizationResult<char, _x46<ParseMode.Realized>>>(() => this.RealizeImpl());
        }
        private _x46(IFuture<IRealizationResult<char, _x46<ParseMode.Realized>>> realizationResult)
        {
            if (typeof(TMode) != typeof(ParseMode.Deferred))
            {
                throw new ArgumentException("TODO");
            }
            
            this.realizationResult = realizationResult;
        }
        
        private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
        private IFuture<IRealizationResult<char, _x46<ParseMode.Realized>>> realizationResult { get; }
        
        internal static _x46<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return new _x46<ParseMode.Deferred>(previousNodeRealizationResult);
        }
        
        public _x46<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _x46<ParseMode.Deferred>(this.previousNodeRealizationResult);
}
else
{
    return new _x46<ParseMode.Deferred>(this.realizationResult);
}
        }
        
        public IRealizationResult<char, _x46<ParseMode.Realized>> Realize()
        {
            return realizationResult.Value;
        }
        
        private IRealizationResult<char, _x46<ParseMode.Realized>> RealizeImpl()
        {
            var output = this.previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _x46<ParseMode.Realized>>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    return new RealizationResult<char, _x46<ParseMode.Realized>>(false, default, output.RemainingTokens);
}

if (input.Current == 'F')
{
    return new RealizationResult<char, _x46<ParseMode.Realized>>(
        true,
        new _x46<ParseMode.Realized>(this.realizationResult),
        input.Next());
}
else
{
    return new RealizationResult<char, _x46<ParseMode.Realized>>(false, default, input);
}
        }
    }
    
}

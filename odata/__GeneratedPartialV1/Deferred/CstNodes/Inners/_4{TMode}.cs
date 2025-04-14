namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _4<TMode> : IAstNode<char, _4<ParseMode.Realized>>, IFromRealizedable<_4<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _4(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            if (typeof(TMode) != typeof(ParseMode.Deferred))
            {
                throw new ArgumentException("TODO");
            }
            
            this.previousNodeRealizationResult = previousNodeRealizationResult;
            
            this.realizationResult = new Future<IRealizationResult<char, _4<ParseMode.Realized>>>(() => this.RealizeImpl());
        }
        private _4(IFuture<IRealizationResult<char, _4<ParseMode.Realized>>> realizationResult)
        {
            if (typeof(TMode) != typeof(ParseMode.Realized))
            {
                throw new ArgumentException("TODO");
            }
            
            this.realizationResult = realizationResult;
        }
        
        private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
        private IFuture<IRealizationResult<char, _4<ParseMode.Realized>>> realizationResult { get; }
        
        internal static _4<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return new _4<ParseMode.Deferred>(previousNodeRealizationResult);
        }
        
        public _4<ParseMode.Deferred> Convert()
        {
            if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new _4<ParseMode.Deferred>(this.previousNodeRealizationResult);
}
else
{
    return new _4<ParseMode.Deferred>(this.realizationResult);
}
        }
        
        public IRealizationResult<char, _4<ParseMode.Realized>> Realize()
        {
            return realizationResult.Value;
        }
        
        private IRealizationResult<char, _4<ParseMode.Realized>> RealizeImpl()
        {
            var output = this.previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _4<ParseMode.Realized>>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    return new RealizationResult<char, _4<ParseMode.Realized>>(false, default, output.RemainingTokens);
}

if (input.Current == '/') //// TODO this whole section is wrong to do
{
    return new RealizationResult<char, _4<ParseMode.Realized>>(
        true,
        new _4<ParseMode.Realized>(this.realizationResult),
        input.Next());
}
else
{
    return new RealizationResult<char, _4<ParseMode.Realized>>(false, default, input);
}
        }
    }
    
}

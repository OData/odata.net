namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _alphaNumericRealized : IFromRealizedable<_alphaNumericDeferred>
    {
        private _alphaNumericRealized()
        {
        }
        
        public abstract _alphaNumericDeferred Convert();
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_alphaNumericRealized node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx41ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx43ʺ node, TContext context);
        }
        
        public sealed class _ʺx41ʺ : _alphaNumericRealized
        {
            private _ʺx41ʺ(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _alphaNumericRealized._ʺx41ʺ>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _alphaNumericRealized._ʺx41ʺ> RealizationResult { get; }
            
            public static IRealizationResult<char, _alphaNumericRealized._ʺx41ʺ> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _alphaNumericRealized._ʺx41ʺ>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _alphaNumericRealized._ʺx41ʺ>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _alphaNumericRealized._ʺx41ʺ(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _alphaNumericRealized._ʺx41ʺ>(false, default, input);
}
            }
            
            public override _alphaNumericDeferred Convert()
            {
                return new _alphaNumericDeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx43ʺ : _alphaNumericRealized
        {
            private _ʺx43ʺ(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _alphaNumericRealized._ʺx43ʺ>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _alphaNumericRealized._ʺx43ʺ> RealizationResult { get; }
            
            public static IRealizationResult<char, _alphaNumericRealized._ʺx43ʺ> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _alphaNumericRealized._ʺx43ʺ>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _alphaNumericRealized._ʺx43ʺ>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _alphaNumericRealized._ʺx43ʺ(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _alphaNumericRealized._ʺx43ʺ>(false, default, input);
}
            }
            
            public override _alphaNumericDeferred Convert()
            {
                return new _alphaNumericDeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

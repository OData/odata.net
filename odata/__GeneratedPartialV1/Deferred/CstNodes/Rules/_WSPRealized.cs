namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _WSPRealized
    {
        private _WSPRealized()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_WSPRealized node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_SP node, TContext context);
            protected internal abstract TResult Accept(_HTAB node, TContext context);
        }
        
        public sealed class _SP : _WSPRealized
        {
            private _SP(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _WSPRealized._SP>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _WSPRealized._SP> RealizationResult { get; }
            
            public static IRealizationResult<char, _WSPRealized._SP> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _WSPRealized._SP>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _WSPRealized._SP>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _WSPRealized._SP(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _WSPRealized._SP>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _HTAB : _WSPRealized
        {
            private _HTAB(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _WSPRealized._HTAB>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _WSPRealized._HTAB> RealizationResult { get; }
            
            public static IRealizationResult<char, _WSPRealized._HTAB> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _WSPRealized._HTAB>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _WSPRealized._HTAB>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _WSPRealized._HTAB(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _WSPRealized._HTAB>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

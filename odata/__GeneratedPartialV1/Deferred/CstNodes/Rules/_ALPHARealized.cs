namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _ALPHARealized : IFromRealizedable<_ALPHADeferred>
    {
        private _ALPHARealized()
        {
        }
        
        public abstract _ALPHADeferred Convert();
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ALPHARealized node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_Ⰳx41ⲻ5A node, TContext context);
            protected internal abstract TResult Accept(_Ⰳx61ⲻ7A node, TContext context);
        }
        
        public sealed class _Ⰳx41ⲻ5A : _ALPHARealized
        {
            private _Ⰳx41ⲻ5A(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _ALPHARealized._Ⰳx41ⲻ5A>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _ALPHARealized._Ⰳx41ⲻ5A> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx41ⲻ5A<ParseMode.Realized> _Ⰳx41ⲻ5A_1 { get; }
            
            public static IRealizationResult<char, _ALPHARealized._Ⰳx41ⲻ5A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _ALPHARealized._Ⰳx41ⲻ5A>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _ALPHARealized._Ⰳx41ⲻ5A>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _ALPHARealized._Ⰳx41ⲻ5A(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _ALPHARealized._Ⰳx41ⲻ5A>(false, default, input);
}
            }
            
            public override _ALPHADeferred Convert()
            {
                return new _ALPHADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _Ⰳx61ⲻ7A : _ALPHARealized
        {
            private _Ⰳx61ⲻ7A(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _ALPHARealized._Ⰳx61ⲻ7A>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _ALPHARealized._Ⰳx61ⲻ7A> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx61ⲻ7A<ParseMode.Realized> _Ⰳx61ⲻ7A_1 { get; }
            
            public static IRealizationResult<char, _ALPHARealized._Ⰳx61ⲻ7A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _ALPHARealized._Ⰳx61ⲻ7A>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _ALPHARealized._Ⰳx61ⲻ7A>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _ALPHARealized._Ⰳx61ⲻ7A(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _ALPHARealized._Ⰳx61ⲻ7A>(false, default, input);
}
            }
            
            public override _ALPHADeferred Convert()
            {
                return new _ALPHADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _alphaNumericRealized
    {
        private _alphaNumericRealized()
        {
        }
        
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
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _CTLRealized
    {
        private _CTLRealized()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_CTLRealized node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_Ⰳx00ⲻ1F node, TContext context);
            protected internal abstract TResult Accept(_Ⰳx7F node, TContext context);
        }
        
        public sealed class _Ⰳx00ⲻ1F : _CTLRealized
        {
            private _Ⰳx00ⲻ1F(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _CTLRealized._Ⰳx00ⲻ1F>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _CTLRealized._Ⰳx00ⲻ1F> RealizationResult { get; }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _Ⰳx7F : _CTLRealized
        {
            private _Ⰳx7F(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _CTLRealized._Ⰳx7F>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _CTLRealized._Ⰳx7F> RealizationResult { get; }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

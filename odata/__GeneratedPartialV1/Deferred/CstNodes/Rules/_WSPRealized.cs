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
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

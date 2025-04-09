namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _BITRealized
    {
        private _BITRealized()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_BITRealized node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx30ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx31ʺ node, TContext context);
        }
        
        public sealed class _ʺx30ʺ : _BITRealized
        {
            private _ʺx30ʺ(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _BITRealized._ʺx30ʺ>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _BITRealized._ʺx30ʺ> RealizationResult { get; }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx31ʺ : _BITRealized
        {
            private _ʺx31ʺ(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _BITRealized._ʺx31ʺ>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _BITRealized._ʺx31ʺ> RealizationResult { get; }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

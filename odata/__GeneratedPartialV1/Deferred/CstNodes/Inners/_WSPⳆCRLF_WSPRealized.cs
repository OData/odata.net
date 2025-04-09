namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _WSPⳆCRLF_WSPRealized
    {
        private _WSPⳆCRLF_WSPRealized()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_WSPⳆCRLF_WSPRealized node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_WSP node, TContext context);
            protected internal abstract TResult Accept(_CRLF_WSP node, TContext context);
        }
        
        public sealed class _WSP : _WSPⳆCRLF_WSPRealized
        {
            private _WSP(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _WSPⳆCRLF_WSPRealized._WSP>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _WSPⳆCRLF_WSPRealized._WSP> RealizationResult { get; }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _CRLF_WSP : _WSPⳆCRLF_WSPRealized
        {
            private _CRLF_WSP(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _WSPⳆCRLF_WSPRealized._CRLF_WSP>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _WSPⳆCRLF_WSPRealized._CRLF_WSP> RealizationResult { get; }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

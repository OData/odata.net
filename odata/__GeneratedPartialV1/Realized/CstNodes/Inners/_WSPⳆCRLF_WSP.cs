namespace __GeneratedPartialV1.CstNodes.Inners
{
    public abstract class _WSPⳆCRLF_WSP
    {
        private _WSPⳆCRLF_WSP()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_WSPⳆCRLF_WSP node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_WSPⳆCRLF_WSP._WSP node, TContext context);
            protected internal abstract TResult Accept(_WSPⳆCRLF_WSP._CRLF_WSP node, TContext context);
        }
        
        public sealed class _WSP : _WSPⳆCRLF_WSP
        {
            public _WSP(__GeneratedPartialV1.CstNodes.Rules._WSP _WSP_1)
            {
                this._WSP_1 = _WSP_1;
            }
            
            public __GeneratedPartialV1.CstNodes.Rules._WSP _WSP_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _CRLF_WSP : _WSPⳆCRLF_WSP
        {
            public _CRLF_WSP(__GeneratedPartialV1.CstNodes.Rules._CRLF _CRLF_1, __GeneratedPartialV1.CstNodes.Rules._WSP _WSP_1)
            {
                this._CRLF_1 = _CRLF_1;
                this._WSP_1 = _WSP_1;
            }
            
            public __GeneratedPartialV1.CstNodes.Rules._CRLF _CRLF_1 { get; }
            public __GeneratedPartialV1.CstNodes.Rules._WSP _WSP_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

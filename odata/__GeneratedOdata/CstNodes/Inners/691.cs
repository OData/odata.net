namespace __GeneratedOdata.CstNodes.Inners
{
    public abstract class _escapeⳆquotationⲻmark
    {
        private _escapeⳆquotationⲻmark()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_escapeⳆquotationⲻmark node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_escapeⳆquotationⲻmark._escape node, TContext context);
            protected internal abstract TResult Accept(_escapeⳆquotationⲻmark._quotationⲻmark node, TContext context);
        }
        
        public sealed class _escape : _escapeⳆquotationⲻmark
        {
            public _escape(__GeneratedOdata.CstNodes.Rules._escape _escape_1)
            {
                this._escape_1 = _escape_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._escape _escape_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _quotationⲻmark : _escapeⳆquotationⲻmark
        {
            public _quotationⲻmark(__GeneratedOdata.CstNodes.Rules._quotationⲻmark _quotationⲻmark_1)
            {
                this._quotationⲻmark_1 = _quotationⲻmark_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._quotationⲻmark _quotationⲻmark_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

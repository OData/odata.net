namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _qcharⲻnoⲻAMPⲻDQUOTE
    {
        private _qcharⲻnoⲻAMPⲻDQUOTE()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_qcharⲻnoⲻAMPⲻDQUOTE node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_qcharⲻnoⲻAMPⲻDQUOTE._qcharⲻunescaped node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻnoⲻAMPⲻDQUOTE._escape_ⲤescapeⳆquotationⲻmarkↃ node, TContext context);
        }
        
        public sealed class _qcharⲻunescaped : _qcharⲻnoⲻAMPⲻDQUOTE
        {
            public _qcharⲻunescaped(__GeneratedOdata.CstNodes.Rules._qcharⲻunescaped _qcharⲻunescaped_1)
            {
                this._qcharⲻunescaped_1 = _qcharⲻunescaped_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._qcharⲻunescaped _qcharⲻunescaped_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _escape_ⲤescapeⳆquotationⲻmarkↃ : _qcharⲻnoⲻAMPⲻDQUOTE
        {
            public _escape_ⲤescapeⳆquotationⲻmarkↃ(__GeneratedOdata.CstNodes.Rules._escape _escape_1, __GeneratedOdata.CstNodes.Inners._ⲤescapeⳆquotationⲻmarkↃ _ⲤescapeⳆquotationⲻmarkↃ_1)
            {
                this._escape_1 = _escape_1;
                this._ⲤescapeⳆquotationⲻmarkↃ_1 = _ⲤescapeⳆquotationⲻmarkↃ_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._escape _escape_1 { get; }
            public __GeneratedOdata.CstNodes.Inners._ⲤescapeⳆquotationⲻmarkↃ _ⲤescapeⳆquotationⲻmarkↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
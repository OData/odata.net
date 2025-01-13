namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _HEXDIG
    {
        private _HEXDIG()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_HEXDIG node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_HEXDIG._DIGIT node, TContext context);
            protected internal abstract TResult Accept(_HEXDIG._AⲻtoⲻF node, TContext context);
        }
        
        public sealed class _DIGIT : _HEXDIG
        {
            public _DIGIT(__GeneratedOdata.CstNodes.Rules._DIGIT _DIGIT_1)
            {
                this._DIGIT_1 = _DIGIT_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._DIGIT _DIGIT_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _AⲻtoⲻF : _HEXDIG
        {
            public _AⲻtoⲻF(__GeneratedOdata.CstNodes.Rules._AⲻtoⲻF _AⲻtoⲻF_1)
            {
                this._AⲻtoⲻF_1 = _AⲻtoⲻF_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._AⲻtoⲻF _AⲻtoⲻF_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

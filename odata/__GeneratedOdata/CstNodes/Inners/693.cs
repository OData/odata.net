namespace __GeneratedOdata.CstNodes.Inners
{
    public abstract class _VCHARⳆobsⲻtext
    {
        private _VCHARⳆobsⲻtext()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_VCHARⳆobsⲻtext node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_VCHARⳆobsⲻtext._VCHAR node, TContext context);
            protected internal abstract TResult Accept(_VCHARⳆobsⲻtext._obsⲻtext node, TContext context);
        }
        
        public sealed class _VCHAR : _VCHARⳆobsⲻtext
        {
            public _VCHAR(__GeneratedOdata.CstNodes.Rules._VCHAR _VCHAR_1)
            {
                this._VCHAR_1 = _VCHAR_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._VCHAR _VCHAR_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _obsⲻtext : _VCHARⳆobsⲻtext
        {
            public _obsⲻtext(__GeneratedOdata.CstNodes.Rules._obsⲻtext _obsⲻtext_1)
            {
                this._obsⲻtext_1 = _obsⲻtext_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._obsⲻtext _obsⲻtext_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

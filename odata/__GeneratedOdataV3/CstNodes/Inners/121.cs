namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _complexPropertyⳆcomplexColProperty
    {
        private _complexPropertyⳆcomplexColProperty()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_complexPropertyⳆcomplexColProperty node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_complexPropertyⳆcomplexColProperty._complexProperty node, TContext context);
            protected internal abstract TResult Accept(_complexPropertyⳆcomplexColProperty._complexColProperty node, TContext context);
        }
        
        public sealed class _complexProperty : _complexPropertyⳆcomplexColProperty
        {
            public _complexProperty(__GeneratedOdataV3.CstNodes.Rules._complexProperty _complexProperty_1)
            {
                this._complexProperty_1 = _complexProperty_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._complexProperty _complexProperty_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _complexColProperty : _complexPropertyⳆcomplexColProperty
        {
            public _complexColProperty(__GeneratedOdataV3.CstNodes.Rules._complexColProperty _complexColProperty_1)
            {
                this._complexColProperty_1 = _complexColProperty_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._complexColProperty _complexColProperty_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

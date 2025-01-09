namespace __Generated.CstNodes.Inners
{
    public abstract class _binⲻvalⳆdecⲻvalⳆhexⲻval
    {
        private _binⲻvalⳆdecⲻvalⳆhexⲻval()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_binⲻvalⳆdecⲻvalⳆhexⲻval node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_binⲻvalⳆdecⲻvalⳆhexⲻval._binⲻval node, TContext context);
            protected internal abstract TResult Accept(_binⲻvalⳆdecⲻvalⳆhexⲻval._decⲻval node, TContext context);
            protected internal abstract TResult Accept(_binⲻvalⳆdecⲻvalⳆhexⲻval._hexⲻval node, TContext context);
        }
        
        public sealed class _binⲻval : _binⲻvalⳆdecⲻvalⳆhexⲻval
        {
            public _binⲻval(__Generated.CstNodes.Rules._binⲻval _binⲻval_1)
            {
                this._binⲻval_1 = _binⲻval_1;
            }
            
            public __Generated.CstNodes.Rules._binⲻval _binⲻval_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _decⲻval : _binⲻvalⳆdecⲻvalⳆhexⲻval
        {
            public _decⲻval(__Generated.CstNodes.Rules._decⲻval _decⲻval_1)
            {
                this._decⲻval_1 = _decⲻval_1;
            }
            
            public __Generated.CstNodes.Rules._decⲻval _decⲻval_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _hexⲻval : _binⲻvalⳆdecⲻvalⳆhexⲻval
        {
            public _hexⲻval(__Generated.CstNodes.Rules._hexⲻval _hexⲻval_1)
            {
                this._hexⲻval_1 = _hexⲻval_1;
            }
            
            public __Generated.CstNodes.Rules._hexⲻval _hexⲻval_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

namespace __Generated.CstNodes.Rules
{
    public abstract class _element
    {
        private _element()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_element node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_element._rulename node, TContext context);
            protected internal abstract TResult Accept(_element._group node, TContext context);
            protected internal abstract TResult Accept(_element._option node, TContext context);
            protected internal abstract TResult Accept(_element._charⲻval node, TContext context);
            protected internal abstract TResult Accept(_element._numⲻval node, TContext context);
            protected internal abstract TResult Accept(_element._proseⲻval node, TContext context);
        }
        
        public sealed class _rulename : _element
        {
            public _rulename(__Generated.CstNodes.Rules._rulename _rulename_1)
            {
                this._rulename_1 = _rulename_1;
            }
            
            public __Generated.CstNodes.Rules._rulename _rulename_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _group : _element
        {
            public _group(__Generated.CstNodes.Rules._group _group_1)
            {
                this._group_1 = _group_1;
            }
            
            public __Generated.CstNodes.Rules._group _group_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _option : _element
        {
            public _option(__Generated.CstNodes.Rules._option _option_1)
            {
                this._option_1 = _option_1;
            }
            
            public __Generated.CstNodes.Rules._option _option_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _charⲻval : _element
        {
            public _charⲻval(__Generated.CstNodes.Rules._charⲻval _charⲻval_1)
            {
                this._charⲻval_1 = _charⲻval_1;
            }
            
            public __Generated.CstNodes.Rules._charⲻval _charⲻval_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _numⲻval : _element
        {
            public _numⲻval(__Generated.CstNodes.Rules._numⲻval _numⲻval_1)
            {
                this._numⲻval_1 = _numⲻval_1;
            }
            
            public __Generated.CstNodes.Rules._numⲻval _numⲻval_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _proseⲻval : _element
        {
            public _proseⲻval(__Generated.CstNodes.Rules._proseⲻval _proseⲻval_1)
            {
                this._proseⲻval_1 = _proseⲻval_1;
            }
            
            public __Generated.CstNodes.Rules._proseⲻval _proseⲻval_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

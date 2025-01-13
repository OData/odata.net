namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _selectOption
    {
        private _selectOption()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_selectOption node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_selectOption._selectOptionPC node, TContext context);
            protected internal abstract TResult Accept(_selectOption._compute node, TContext context);
            protected internal abstract TResult Accept(_selectOption._select node, TContext context);
            protected internal abstract TResult Accept(_selectOption._expand node, TContext context);
            protected internal abstract TResult Accept(_selectOption._aliasAndValue node, TContext context);
        }
        
        public sealed class _selectOptionPC : _selectOption
        {
            public _selectOptionPC(__GeneratedOdata.CstNodes.Rules._selectOptionPC _selectOptionPC_1)
            {
                this._selectOptionPC_1 = _selectOptionPC_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._selectOptionPC _selectOptionPC_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _compute : _selectOption
        {
            public _compute(__GeneratedOdata.CstNodes.Rules._compute _compute_1)
            {
                this._compute_1 = _compute_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._compute _compute_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _select : _selectOption
        {
            public _select(__GeneratedOdata.CstNodes.Rules._select _select_1)
            {
                this._select_1 = _select_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._select _select_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _expand : _selectOption
        {
            public _expand(__GeneratedOdata.CstNodes.Rules._expand _expand_1)
            {
                this._expand_1 = _expand_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._expand _expand_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _aliasAndValue : _selectOption
        {
            public _aliasAndValue(__GeneratedOdata.CstNodes.Rules._aliasAndValue _aliasAndValue_1)
            {
                this._aliasAndValue_1 = _aliasAndValue_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._aliasAndValue _aliasAndValue_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

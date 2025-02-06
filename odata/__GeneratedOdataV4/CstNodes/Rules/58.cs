namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _entityCastOption
    {
        private _entityCastOption()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_entityCastOption node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_entityCastOption._entityIdOption node, TContext context);
            protected internal abstract TResult Accept(_entityCastOption._expand node, TContext context);
            protected internal abstract TResult Accept(_entityCastOption._select node, TContext context);
        }
        
        public sealed class _entityIdOption : _entityCastOption
        {
            public _entityIdOption(__GeneratedOdataV4.CstNodes.Rules._entityIdOption _entityIdOption_1)
            {
                this._entityIdOption_1 = _entityIdOption_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._entityIdOption _entityIdOption_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _expand : _entityCastOption
        {
            public _expand(__GeneratedOdataV4.CstNodes.Rules._expand _expand_1)
            {
                this._expand_1 = _expand_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._expand _expand_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _select : _entityCastOption
        {
            public _select(__GeneratedOdataV4.CstNodes.Rules._select _select_1)
            {
                this._select_1 = _select_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._select _select_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

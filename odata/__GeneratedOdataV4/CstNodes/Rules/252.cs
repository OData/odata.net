namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _navigationProperty
    {
        private _navigationProperty()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_navigationProperty node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_navigationProperty._entityNavigationProperty node, TContext context);
            protected internal abstract TResult Accept(_navigationProperty._entityColNavigationProperty node, TContext context);
        }
        
        public sealed class _entityNavigationProperty : _navigationProperty
        {
            public _entityNavigationProperty(__GeneratedOdataV4.CstNodes.Rules._entityNavigationProperty _entityNavigationProperty_1)
            {
                this._entityNavigationProperty_1 = _entityNavigationProperty_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._entityNavigationProperty _entityNavigationProperty_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _entityColNavigationProperty : _navigationProperty
        {
            public _entityColNavigationProperty(__GeneratedOdataV4.CstNodes.Rules._entityColNavigationProperty _entityColNavigationProperty_1)
            {
                this._entityColNavigationProperty_1 = _entityColNavigationProperty_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._entityColNavigationProperty _entityColNavigationProperty_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

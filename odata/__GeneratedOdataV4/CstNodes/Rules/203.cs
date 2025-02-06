namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _navigationPropertyInUri
    {
        private _navigationPropertyInUri()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_navigationPropertyInUri node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_navigationPropertyInUri._singleNavPropInJSON node, TContext context);
            protected internal abstract TResult Accept(_navigationPropertyInUri._collectionNavPropInJSON node, TContext context);
        }
        
        public sealed class _singleNavPropInJSON : _navigationPropertyInUri
        {
            public _singleNavPropInJSON(__GeneratedOdataV4.CstNodes.Rules._singleNavPropInJSON _singleNavPropInJSON_1)
            {
                this._singleNavPropInJSON_1 = _singleNavPropInJSON_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._singleNavPropInJSON _singleNavPropInJSON_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _collectionNavPropInJSON : _navigationPropertyInUri
        {
            public _collectionNavPropInJSON(__GeneratedOdataV4.CstNodes.Rules._collectionNavPropInJSON _collectionNavPropInJSON_1)
            {
                this._collectionNavPropInJSON_1 = _collectionNavPropInJSON_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._collectionNavPropInJSON _collectionNavPropInJSON_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

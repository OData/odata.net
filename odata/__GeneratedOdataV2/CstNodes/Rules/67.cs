namespace __GeneratedOdataV2.CstNodes.Rules
{
    public abstract class _expandCountOption
    {
        private _expandCountOption()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_expandCountOption node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_expandCountOption._filter node, TContext context);
            protected internal abstract TResult Accept(_expandCountOption._search node, TContext context);
        }
        
        public sealed class _filter : _expandCountOption
        {
            public _filter(__GeneratedOdataV2.CstNodes.Rules._filter _filter_1)
            {
                this._filter_1 = _filter_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._filter _filter_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _search : _expandCountOption
        {
            public _search(__GeneratedOdataV2.CstNodes.Rules._search _search_1)
            {
                this._search_1 = _search_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._search _search_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

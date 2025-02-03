namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _searchOrExprⳆsearchAndExpr
    {
        private _searchOrExprⳆsearchAndExpr()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_searchOrExprⳆsearchAndExpr node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_searchOrExprⳆsearchAndExpr._searchOrExpr node, TContext context);
            protected internal abstract TResult Accept(_searchOrExprⳆsearchAndExpr._searchAndExpr node, TContext context);
        }
        
        public sealed class _searchOrExpr : _searchOrExprⳆsearchAndExpr
        {
            public _searchOrExpr(__GeneratedOdataV3.CstNodes.Rules._searchOrExpr _searchOrExpr_1)
            {
                this._searchOrExpr_1 = _searchOrExpr_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._searchOrExpr _searchOrExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _searchAndExpr : _searchOrExprⳆsearchAndExpr
        {
            public _searchAndExpr(__GeneratedOdataV3.CstNodes.Rules._searchAndExpr _searchAndExpr_1)
            {
                this._searchAndExpr_1 = _searchAndExpr_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._searchAndExpr _searchAndExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

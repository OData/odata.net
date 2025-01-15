namespace __GeneratedOdata.CstNodes.Inners
{
    public abstract class _OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm
    {
        private _OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm._OPEN_BWS_searchExpr_BWS_CLOSE node, TContext context);
            protected internal abstract TResult Accept(_OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm._searchTerm node, TContext context);
        }
        
        public sealed class _OPEN_BWS_searchExpr_BWS_CLOSE : _OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm
        {
            public _OPEN_BWS_searchExpr_BWS_CLOSE(__GeneratedOdata.CstNodes.Rules._OPEN _OPEN_1, __GeneratedOdata.CstNodes.Rules._BWS _BWS_1, __GeneratedOdata.CstNodes.Rules._searchExpr _searchExpr_1, __GeneratedOdata.CstNodes.Rules._BWS _BWS_2, __GeneratedOdata.CstNodes.Rules._CLOSE _CLOSE_1)
            {
                this._OPEN_1 = _OPEN_1;
                this._BWS_1 = _BWS_1;
                this._searchExpr_1 = _searchExpr_1;
                this._BWS_2 = _BWS_2;
                this._CLOSE_1 = _CLOSE_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._OPEN _OPEN_1 { get; }
            public __GeneratedOdata.CstNodes.Rules._BWS _BWS_1 { get; }
            public __GeneratedOdata.CstNodes.Rules._searchExpr _searchExpr_1 { get; }
            public __GeneratedOdata.CstNodes.Rules._BWS _BWS_2 { get; }
            public __GeneratedOdata.CstNodes.Rules._CLOSE _CLOSE_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _searchTerm : _OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm
        {
            public _searchTerm(__GeneratedOdata.CstNodes.Rules._searchTerm _searchTerm_1)
            {
                this._searchTerm_1 = _searchTerm_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._searchTerm _searchTerm_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

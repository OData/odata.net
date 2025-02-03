namespace __GeneratedOdataV2.CstNodes.Inners
{
    public abstract class _keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr
    {
        private _keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._keyPredicate_꘡singleNavigationExpr꘡ node, TContext context);
            protected internal abstract TResult Accept(_keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._collectionPathExpr node, TContext context);
        }
        
        public sealed class _keyPredicate_꘡singleNavigationExpr꘡ : _keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr
        {
            public _keyPredicate_꘡singleNavigationExpr꘡(__GeneratedOdataV2.CstNodes.Rules._keyPredicate _keyPredicate_1, __GeneratedOdataV2.CstNodes.Rules._singleNavigationExpr? _singleNavigationExpr_1)
            {
                this._keyPredicate_1 = _keyPredicate_1;
                this._singleNavigationExpr_1 = _singleNavigationExpr_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._keyPredicate _keyPredicate_1 { get; }
            public __GeneratedOdataV2.CstNodes.Rules._singleNavigationExpr? _singleNavigationExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _collectionPathExpr : _keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr
        {
            public _collectionPathExpr(__GeneratedOdataV2.CstNodes.Rules._collectionPathExpr _collectionPathExpr_1)
            {
                this._collectionPathExpr_1 = _collectionPathExpr_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._collectionPathExpr _collectionPathExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

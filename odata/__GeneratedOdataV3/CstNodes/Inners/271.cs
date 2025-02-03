namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr
    {
        private _collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._collectionPathExpr node, TContext context);
            protected internal abstract TResult Accept(_collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._singleNavigationExpr node, TContext context);
            protected internal abstract TResult Accept(_collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._complexPathExpr node, TContext context);
            protected internal abstract TResult Accept(_collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._primitivePathExpr node, TContext context);
        }
        
        public sealed class _collectionPathExpr : _collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr
        {
            public _collectionPathExpr(__GeneratedOdataV3.CstNodes.Rules._collectionPathExpr _collectionPathExpr_1)
            {
                this._collectionPathExpr_1 = _collectionPathExpr_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._collectionPathExpr _collectionPathExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _singleNavigationExpr : _collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr
        {
            public _singleNavigationExpr(__GeneratedOdataV3.CstNodes.Rules._singleNavigationExpr _singleNavigationExpr_1)
            {
                this._singleNavigationExpr_1 = _singleNavigationExpr_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._singleNavigationExpr _singleNavigationExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _complexPathExpr : _collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr
        {
            public _complexPathExpr(__GeneratedOdataV3.CstNodes.Rules._complexPathExpr _complexPathExpr_1)
            {
                this._complexPathExpr_1 = _complexPathExpr_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._complexPathExpr _complexPathExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _primitivePathExpr : _collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr
        {
            public _primitivePathExpr(__GeneratedOdataV3.CstNodes.Rules._primitivePathExpr _primitivePathExpr_1)
            {
                this._primitivePathExpr_1 = _primitivePathExpr_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._primitivePathExpr _primitivePathExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

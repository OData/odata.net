namespace __GeneratedOdata.CstNodes.Inners
{
    public abstract class _annotationExprⳆboundFunctionExpr
    {
        private _annotationExprⳆboundFunctionExpr()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_annotationExprⳆboundFunctionExpr node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_annotationExprⳆboundFunctionExpr._annotationExpr node, TContext context);
            protected internal abstract TResult Accept(_annotationExprⳆboundFunctionExpr._boundFunctionExpr node, TContext context);
        }
        
        public sealed class _annotationExpr : _annotationExprⳆboundFunctionExpr
        {
            public _annotationExpr(__GeneratedOdata.CstNodes.Rules._annotationExpr _annotationExpr_1)
            {
                this._annotationExpr_1 = _annotationExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._annotationExpr _annotationExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _boundFunctionExpr : _annotationExprⳆboundFunctionExpr
        {
            public _boundFunctionExpr(__GeneratedOdata.CstNodes.Rules._boundFunctionExpr _boundFunctionExpr_1)
            {
                this._boundFunctionExpr_1 = _boundFunctionExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._boundFunctionExpr _boundFunctionExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
namespace __GeneratedOdata.CstNodes.Inners
{
    public abstract class _propertyPathExprⳆboundFunctionExprⳆannotationExpr
    {
        private _propertyPathExprⳆboundFunctionExprⳆannotationExpr()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_propertyPathExprⳆboundFunctionExprⳆannotationExpr node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_propertyPathExprⳆboundFunctionExprⳆannotationExpr._propertyPathExpr node, TContext context);
            protected internal abstract TResult Accept(_propertyPathExprⳆboundFunctionExprⳆannotationExpr._boundFunctionExpr node, TContext context);
            protected internal abstract TResult Accept(_propertyPathExprⳆboundFunctionExprⳆannotationExpr._annotationExpr node, TContext context);
        }
        
        public sealed class _propertyPathExpr : _propertyPathExprⳆboundFunctionExprⳆannotationExpr
        {
            public _propertyPathExpr(__GeneratedOdata.CstNodes.Rules._propertyPathExpr _propertyPathExpr_1)
            {
                this._propertyPathExpr_1 = _propertyPathExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._propertyPathExpr _propertyPathExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _boundFunctionExpr : _propertyPathExprⳆboundFunctionExprⳆannotationExpr
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
        
        public sealed class _annotationExpr : _propertyPathExprⳆboundFunctionExprⳆannotationExpr
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
    }
    
}

namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _andExprⳆorExpr
    {
        private _andExprⳆorExpr()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_andExprⳆorExpr node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_andExprⳆorExpr._andExpr node, TContext context);
            protected internal abstract TResult Accept(_andExprⳆorExpr._orExpr node, TContext context);
        }
        
        public sealed class _andExpr : _andExprⳆorExpr
        {
            public _andExpr(__GeneratedOdataV3.CstNodes.Rules._andExpr _andExpr_1)
            {
                this._andExpr_1 = _andExpr_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._andExpr _andExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _orExpr : _andExprⳆorExpr
        {
            public _orExpr(__GeneratedOdataV3.CstNodes.Rules._orExpr _orExpr_1)
            {
                this._orExpr_1 = _orExpr_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._orExpr _orExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

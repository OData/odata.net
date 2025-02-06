namespace __GeneratedOdataV4.CstNodes.Inners
{
    public abstract class _ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr
    {
        private _ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr._ʺx2Fʺ_propertyPathExpr node, TContext context);
            protected internal abstract TResult Accept(_ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr._ʺx2Fʺ_boundFunctionExpr node, TContext context);
            protected internal abstract TResult Accept(_ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr._ʺx2Fʺ_annotationExpr node, TContext context);
        }
        
        public sealed class _ʺx2Fʺ_propertyPathExpr : _ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr
        {
            public _ʺx2Fʺ_propertyPathExpr(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1, __GeneratedOdataV4.CstNodes.Rules._propertyPathExpr _propertyPathExpr_1)
            {
                this._ʺx2Fʺ_1 = _ʺx2Fʺ_1;
                this._propertyPathExpr_1 = _propertyPathExpr_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1 { get; }
            public __GeneratedOdataV4.CstNodes.Rules._propertyPathExpr _propertyPathExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Fʺ_boundFunctionExpr : _ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr
        {
            public _ʺx2Fʺ_boundFunctionExpr(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1, __GeneratedOdataV4.CstNodes.Rules._boundFunctionExpr _boundFunctionExpr_1)
            {
                this._ʺx2Fʺ_1 = _ʺx2Fʺ_1;
                this._boundFunctionExpr_1 = _boundFunctionExpr_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1 { get; }
            public __GeneratedOdataV4.CstNodes.Rules._boundFunctionExpr _boundFunctionExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Fʺ_annotationExpr : _ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr
        {
            public _ʺx2Fʺ_annotationExpr(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1, __GeneratedOdataV4.CstNodes.Rules._annotationExpr _annotationExpr_1)
            {
                this._ʺx2Fʺ_1 = _ʺx2Fʺ_1;
                this._annotationExpr_1 = _annotationExpr_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1 { get; }
            public __GeneratedOdataV4.CstNodes.Rules._annotationExpr _annotationExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

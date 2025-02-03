namespace __GeneratedOdataV3.CstNodes.Rules
{
    public abstract class _inscopeVariableExpr
    {
        private _inscopeVariableExpr()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_inscopeVariableExpr node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_inscopeVariableExpr._implicitVariableExpr node, TContext context);
            protected internal abstract TResult Accept(_inscopeVariableExpr._parameterAlias node, TContext context);
            protected internal abstract TResult Accept(_inscopeVariableExpr._lambdaVariableExpr node, TContext context);
        }
        
        public sealed class _implicitVariableExpr : _inscopeVariableExpr
        {
            public _implicitVariableExpr(__GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr _implicitVariableExpr_1)
            {
                this._implicitVariableExpr_1 = _implicitVariableExpr_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr _implicitVariableExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _parameterAlias : _inscopeVariableExpr
        {
            public _parameterAlias(__GeneratedOdataV3.CstNodes.Rules._parameterAlias _parameterAlias_1)
            {
                this._parameterAlias_1 = _parameterAlias_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._parameterAlias _parameterAlias_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _lambdaVariableExpr : _inscopeVariableExpr
        {
            public _lambdaVariableExpr(__GeneratedOdataV3.CstNodes.Rules._lambdaVariableExpr _lambdaVariableExpr_1)
            {
                this._lambdaVariableExpr_1 = _lambdaVariableExpr_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._lambdaVariableExpr _lambdaVariableExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

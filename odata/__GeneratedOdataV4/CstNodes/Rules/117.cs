namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _firstMemberExpr
    {
        private _firstMemberExpr()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_firstMemberExpr node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_firstMemberExpr._memberExpr node, TContext context);
            protected internal abstract TResult Accept(_firstMemberExpr._inscopeVariableExpr_꘡ʺx2Fʺ_memberExpr꘡ node, TContext context);
        }
        
        public sealed class _memberExpr : _firstMemberExpr
        {
            public _memberExpr(__GeneratedOdataV4.CstNodes.Rules._memberExpr _memberExpr_1)
            {
                this._memberExpr_1 = _memberExpr_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._memberExpr _memberExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _inscopeVariableExpr_꘡ʺx2Fʺ_memberExpr꘡ : _firstMemberExpr
        {
            public _inscopeVariableExpr_꘡ʺx2Fʺ_memberExpr꘡(__GeneratedOdataV4.CstNodes.Rules._inscopeVariableExpr _inscopeVariableExpr_1, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_memberExpr? _ʺx2Fʺ_memberExpr_1)
            {
                this._inscopeVariableExpr_1 = _inscopeVariableExpr_1;
                this._ʺx2Fʺ_memberExpr_1 = _ʺx2Fʺ_memberExpr_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._inscopeVariableExpr _inscopeVariableExpr_1 { get; }
            public __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_memberExpr? _ʺx2Fʺ_memberExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

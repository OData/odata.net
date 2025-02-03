namespace __GeneratedOdataV2.CstNodes.Rules
{
    public abstract class _implicitVariableExpr
    {
        private _implicitVariableExpr()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_implicitVariableExpr node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_implicitVariableExpr._ʺx24x69x74ʺ node, TContext context);
            protected internal abstract TResult Accept(_implicitVariableExpr._ʺx24x74x68x69x73ʺ node, TContext context);
        }
        
        public sealed class _ʺx24x69x74ʺ : _implicitVariableExpr
        {
            public _ʺx24x69x74ʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx24x69x74ʺ _ʺx24x69x74ʺ_1)
            {
                this._ʺx24x69x74ʺ_1 = _ʺx24x69x74ʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx24x69x74ʺ _ʺx24x69x74ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx24x74x68x69x73ʺ : _implicitVariableExpr
        {
            public _ʺx24x74x68x69x73ʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx24x74x68x69x73ʺ _ʺx24x74x68x69x73ʺ_1)
            {
                this._ʺx24x74x68x69x73ʺ_1 = _ʺx24x74x68x69x73ʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx24x74x68x69x73ʺ _ʺx24x74x68x69x73ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

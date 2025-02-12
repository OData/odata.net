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
            private _ʺx24x69x74ʺ()
            {
                this._ʺx24x69x74ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx24x69x74ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx24x69x74ʺ _ʺx24x69x74ʺ_1 { get; }
            public static _ʺx24x69x74ʺ Instance { get; } = new _ʺx24x69x74ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx24x74x68x69x73ʺ : _implicitVariableExpr
        {
            private _ʺx24x74x68x69x73ʺ()
            {
                this._ʺx24x74x68x69x73ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx24x74x68x69x73ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx24x74x68x69x73ʺ _ʺx24x74x68x69x73ʺ_1 { get; }
            public static _ʺx24x74x68x69x73ʺ Instance { get; } = new _ʺx24x74x68x69x73ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

namespace __GeneratedOdataV2.CstNodes.Rules
{
    public abstract class _booleanValue
    {
        private _booleanValue()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_booleanValue node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_booleanValue._ʺx74x72x75x65ʺ node, TContext context);
            protected internal abstract TResult Accept(_booleanValue._ʺx66x61x6Cx73x65ʺ node, TContext context);
        }
        
        public sealed class _ʺx74x72x75x65ʺ : _booleanValue
        {
            private _ʺx74x72x75x65ʺ()
            {
                this._ʺx74x72x75x65ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx74x72x75x65ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx74x72x75x65ʺ _ʺx74x72x75x65ʺ_1 { get; }
            public static _ʺx74x72x75x65ʺ Instance { get; } = new _ʺx74x72x75x65ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx66x61x6Cx73x65ʺ : _booleanValue
        {
            private _ʺx66x61x6Cx73x65ʺ()
            {
                this._ʺx66x61x6Cx73x65ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx66x61x6Cx73x65ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx66x61x6Cx73x65ʺ _ʺx66x61x6Cx73x65ʺ_1 { get; }
            public static _ʺx66x61x6Cx73x65ʺ Instance { get; } = new _ʺx66x61x6Cx73x65ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

namespace __GeneratedOdataV3.CstNodes.Rules
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
            public _ʺx74x72x75x65ʺ(__GeneratedOdataV3.CstNodes.Inners._ʺx74x72x75x65ʺ _ʺx74x72x75x65ʺ_1)
            {
                this._ʺx74x72x75x65ʺ_1 = _ʺx74x72x75x65ʺ_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx74x72x75x65ʺ _ʺx74x72x75x65ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx66x61x6Cx73x65ʺ : _booleanValue
        {
            public _ʺx66x61x6Cx73x65ʺ(__GeneratedOdataV3.CstNodes.Inners._ʺx66x61x6Cx73x65ʺ _ʺx66x61x6Cx73x65ʺ_1)
            {
                this._ʺx66x61x6Cx73x65ʺ_1 = _ʺx66x61x6Cx73x65ʺ_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx66x61x6Cx73x65ʺ _ʺx66x61x6Cx73x65ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

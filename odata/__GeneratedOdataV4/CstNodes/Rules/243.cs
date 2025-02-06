namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _abstractSpatialTypeName
    {
        private _abstractSpatialTypeName()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_abstractSpatialTypeName node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_abstractSpatialTypeName._ʺx47x65x6Fx67x72x61x70x68x79ʺ node, TContext context);
            protected internal abstract TResult Accept(_abstractSpatialTypeName._ʺx47x65x6Fx6Dx65x74x72x79ʺ node, TContext context);
        }
        
        public sealed class _ʺx47x65x6Fx67x72x61x70x68x79ʺ : _abstractSpatialTypeName
        {
            private _ʺx47x65x6Fx67x72x61x70x68x79ʺ()
            {
                this._ʺx47x65x6Fx67x72x61x70x68x79ʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx47x65x6Fx67x72x61x70x68x79ʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx47x65x6Fx67x72x61x70x68x79ʺ _ʺx47x65x6Fx67x72x61x70x68x79ʺ_1 { get; }
            public static _ʺx47x65x6Fx67x72x61x70x68x79ʺ Instance { get; } = new _ʺx47x65x6Fx67x72x61x70x68x79ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx47x65x6Fx6Dx65x74x72x79ʺ : _abstractSpatialTypeName
        {
            private _ʺx47x65x6Fx6Dx65x74x72x79ʺ()
            {
                this._ʺx47x65x6Fx6Dx65x74x72x79ʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx47x65x6Fx6Dx65x74x72x79ʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx47x65x6Fx6Dx65x74x72x79ʺ _ʺx47x65x6Fx6Dx65x74x72x79ʺ_1 { get; }
            public static _ʺx47x65x6Fx6Dx65x74x72x79ʺ Instance { get; } = new _ʺx47x65x6Fx6Dx65x74x72x79ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

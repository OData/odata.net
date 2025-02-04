namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ
    {
        private _ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ._ʺx24x6Fx72x64x65x72x62x79ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ._ʺx6Fx72x64x65x72x62x79ʺ node, TContext context);
        }
        
        public sealed class _ʺx24x6Fx72x64x65x72x62x79ʺ : _ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ
        {
            private _ʺx24x6Fx72x64x65x72x62x79ʺ()
            {
                this._ʺx24x6Fx72x64x65x72x62x79ʺ_1 = __GeneratedOdataV3.CstNodes.Inners._ʺx24x6Fx72x64x65x72x62x79ʺ.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx24x6Fx72x64x65x72x62x79ʺ _ʺx24x6Fx72x64x65x72x62x79ʺ_1 { get; }
            public static _ʺx24x6Fx72x64x65x72x62x79ʺ Instance { get; } = new _ʺx24x6Fx72x64x65x72x62x79ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx6Fx72x64x65x72x62x79ʺ : _ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ
        {
            private _ʺx6Fx72x64x65x72x62x79ʺ()
            {
                this._ʺx6Fx72x64x65x72x62x79ʺ_1 = __GeneratedOdataV3.CstNodes.Inners._ʺx6Fx72x64x65x72x62x79ʺ.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx6Fx72x64x65x72x62x79ʺ _ʺx6Fx72x64x65x72x62x79ʺ_1 { get; }
            public static _ʺx6Fx72x64x65x72x62x79ʺ Instance { get; } = new _ʺx6Fx72x64x65x72x62x79ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

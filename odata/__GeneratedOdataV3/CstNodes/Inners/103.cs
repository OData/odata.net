namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ
    {
        private _ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ._ʺx24x65x78x70x61x6Ex64ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ._ʺx65x78x70x61x6Ex64ʺ node, TContext context);
        }
        
        public sealed class _ʺx24x65x78x70x61x6Ex64ʺ : _ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ
        {
            private _ʺx24x65x78x70x61x6Ex64ʺ()
            {
                this._ʺx24x65x78x70x61x6Ex64ʺ_1 = __GeneratedOdataV3.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺ.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺ _ʺx24x65x78x70x61x6Ex64ʺ_1 { get; }
            public static _ʺx24x65x78x70x61x6Ex64ʺ Instance { get; } = new _ʺx24x65x78x70x61x6Ex64ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx65x78x70x61x6Ex64ʺ : _ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ
        {
            private _ʺx65x78x70x61x6Ex64ʺ()
            {
                this._ʺx65x78x70x61x6Ex64ʺ_1 = __GeneratedOdataV3.CstNodes.Inners._ʺx65x78x70x61x6Ex64ʺ.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx65x78x70x61x6Ex64ʺ _ʺx65x78x70x61x6Ex64ʺ_1 { get; }
            public static _ʺx65x78x70x61x6Ex64ʺ Instance { get; } = new _ʺx65x78x70x61x6Ex64ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

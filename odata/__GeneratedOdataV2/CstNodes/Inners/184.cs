namespace __GeneratedOdataV2.CstNodes.Inners
{
    public abstract class _ʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺ
    {
        private _ʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺ._ʺx24x73x65x61x72x63x68ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺ._ʺx73x65x61x72x63x68ʺ node, TContext context);
        }
        
        public sealed class _ʺx24x73x65x61x72x63x68ʺ : _ʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺ
        {
            private _ʺx24x73x65x61x72x63x68ʺ()
            {
                this._ʺx24x73x65x61x72x63x68ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx24x73x65x61x72x63x68ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx24x73x65x61x72x63x68ʺ _ʺx24x73x65x61x72x63x68ʺ_1 { get; }
            public static _ʺx24x73x65x61x72x63x68ʺ Instance { get; } = new _ʺx24x73x65x61x72x63x68ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx73x65x61x72x63x68ʺ : _ʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺ
        {
            private _ʺx73x65x61x72x63x68ʺ()
            {
                this._ʺx73x65x61x72x63x68ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx73x65x61x72x63x68ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx73x65x61x72x63x68ʺ _ʺx73x65x61x72x63x68ʺ_1 { get; }
            public static _ʺx73x65x61x72x63x68ʺ Instance { get; } = new _ʺx73x65x61x72x63x68ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

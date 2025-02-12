namespace __GeneratedOdataV2.CstNodes.Inners
{
    public abstract class _ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ
    {
        private _ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ._ʺx68x74x74x70x73ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ._ʺx68x74x74x70ʺ node, TContext context);
        }
        
        public sealed class _ʺx68x74x74x70x73ʺ : _ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ
        {
            private _ʺx68x74x74x70x73ʺ()
            {
                this._ʺx68x74x74x70x73ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx68x74x74x70x73ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx68x74x74x70x73ʺ _ʺx68x74x74x70x73ʺ_1 { get; }
            public static _ʺx68x74x74x70x73ʺ Instance { get; } = new _ʺx68x74x74x70x73ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx68x74x74x70ʺ : _ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ
        {
            private _ʺx68x74x74x70ʺ()
            {
                this._ʺx68x74x74x70ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx68x74x74x70ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx68x74x74x70ʺ _ʺx68x74x74x70ʺ_1 { get; }
            public static _ʺx68x74x74x70ʺ Instance { get; } = new _ʺx68x74x74x70ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

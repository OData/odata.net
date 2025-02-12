namespace __GeneratedOdataV2.CstNodes.Inners
{
    public abstract class _ʺx30ʺⳆʺx31ʺⳆʺx32ʺ
    {
        private _ʺx30ʺⳆʺx31ʺⳆʺx32ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx30ʺⳆʺx31ʺⳆʺx32ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx30ʺⳆʺx31ʺⳆʺx32ʺ._ʺx30ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx30ʺⳆʺx31ʺⳆʺx32ʺ._ʺx31ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx30ʺⳆʺx31ʺⳆʺx32ʺ._ʺx32ʺ node, TContext context);
        }
        
        public sealed class _ʺx30ʺ : _ʺx30ʺⳆʺx31ʺⳆʺx32ʺ
        {
            private _ʺx30ʺ()
            {
                this._ʺx30ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx30ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx30ʺ _ʺx30ʺ_1 { get; }
            public static _ʺx30ʺ Instance { get; } = new _ʺx30ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx31ʺ : _ʺx30ʺⳆʺx31ʺⳆʺx32ʺ
        {
            private _ʺx31ʺ()
            {
                this._ʺx31ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx31ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx31ʺ _ʺx31ʺ_1 { get; }
            public static _ʺx31ʺ Instance { get; } = new _ʺx31ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx32ʺ : _ʺx30ʺⳆʺx31ʺⳆʺx32ʺ
        {
            private _ʺx32ʺ()
            {
                this._ʺx32ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx32ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx32ʺ _ʺx32ʺ_1 { get; }
            public static _ʺx32ʺ Instance { get; } = new _ʺx32ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

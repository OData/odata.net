namespace __GeneratedOdataV2.CstNodes.Inners
{
    public abstract class _ʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺ
    {
        private _ʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺ._ʺx24x63x6Fx75x6Ex74ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺ._ʺx63x6Fx75x6Ex74ʺ node, TContext context);
        }
        
        public sealed class _ʺx24x63x6Fx75x6Ex74ʺ : _ʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺ
        {
            private _ʺx24x63x6Fx75x6Ex74ʺ()
            {
                this._ʺx24x63x6Fx75x6Ex74ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx24x63x6Fx75x6Ex74ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx24x63x6Fx75x6Ex74ʺ _ʺx24x63x6Fx75x6Ex74ʺ_1 { get; }
            public static _ʺx24x63x6Fx75x6Ex74ʺ Instance { get; } = new _ʺx24x63x6Fx75x6Ex74ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx63x6Fx75x6Ex74ʺ : _ʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺ
        {
            private _ʺx63x6Fx75x6Ex74ʺ()
            {
                this._ʺx63x6Fx75x6Ex74ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx63x6Fx75x6Ex74ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx63x6Fx75x6Ex74ʺ _ʺx63x6Fx75x6Ex74ʺ_1 { get; }
            public static _ʺx63x6Fx75x6Ex74ʺ Instance { get; } = new _ʺx63x6Fx75x6Ex74ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

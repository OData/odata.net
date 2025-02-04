namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _ʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺ
    {
        private _ʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺ._ʺx24x63x6Fx6Dx70x75x74x65ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺ._ʺx63x6Fx6Dx70x75x74x65ʺ node, TContext context);
        }
        
        public sealed class _ʺx24x63x6Fx6Dx70x75x74x65ʺ : _ʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺ
        {
            private _ʺx24x63x6Fx6Dx70x75x74x65ʺ()
            {
                this._ʺx24x63x6Fx6Dx70x75x74x65ʺ_1 = __GeneratedOdataV3.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺ.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺ _ʺx24x63x6Fx6Dx70x75x74x65ʺ_1 { get; }
            public static _ʺx24x63x6Fx6Dx70x75x74x65ʺ Instance { get; } = new _ʺx24x63x6Fx6Dx70x75x74x65ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx63x6Fx6Dx70x75x74x65ʺ : _ʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺ
        {
            private _ʺx63x6Fx6Dx70x75x74x65ʺ()
            {
                this._ʺx63x6Fx6Dx70x75x74x65ʺ_1 = __GeneratedOdataV3.CstNodes.Inners._ʺx63x6Fx6Dx70x75x74x65ʺ.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx63x6Fx6Dx70x75x74x65ʺ _ʺx63x6Fx6Dx70x75x74x65ʺ_1 { get; }
            public static _ʺx63x6Fx6Dx70x75x74x65ʺ Instance { get; } = new _ʺx63x6Fx6Dx70x75x74x65ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

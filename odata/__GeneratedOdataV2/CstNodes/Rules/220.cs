namespace __GeneratedOdataV2.CstNodes.Rules
{
    public abstract class _int
    {
        private _int()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_int node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_int._ʺx30ʺ node, TContext context);
            protected internal abstract TResult Accept(_int._ⲤoneToNine_ЖDIGITↃ node, TContext context);
        }
        
        public sealed class _ʺx30ʺ : _int
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
        
        public sealed class _ⲤoneToNine_ЖDIGITↃ : _int
        {
            public _ⲤoneToNine_ЖDIGITↃ(__GeneratedOdataV2.CstNodes.Inners._ⲤoneToNine_ЖDIGITↃ _ⲤoneToNine_ЖDIGITↃ_1)
            {
                this._ⲤoneToNine_ЖDIGITↃ_1 = _ⲤoneToNine_ЖDIGITↃ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ⲤoneToNine_ЖDIGITↃ _ⲤoneToNine_ЖDIGITↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

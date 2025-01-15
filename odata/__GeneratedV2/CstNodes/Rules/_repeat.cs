namespace __GeneratedV2.CstNodes.Rules
{
    public abstract class _repeat
    {
        private _repeat()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_repeat node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_repeat._Ⲥʺx2Aʺ_ЖDIGITↃ node, TContext context);
            protected internal abstract TResult Accept(_repeat._Ⲥ1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡Ↄ node, TContext context);
        }
        
        public sealed class _Ⲥʺx2Aʺ_ЖDIGITↃ : _repeat
        {
            public _Ⲥʺx2Aʺ_ЖDIGITↃ(__GeneratedV2.CstNodes.Inners._Ⲥʺx2Aʺ_ЖDIGITↃ _Ⲥʺx2Aʺ_ЖDIGITↃ_1)
            {
                this._Ⲥʺx2Aʺ_ЖDIGITↃ_1 = _Ⲥʺx2Aʺ_ЖDIGITↃ_1;
            }
            
            public __GeneratedV2.CstNodes.Inners._Ⲥʺx2Aʺ_ЖDIGITↃ _Ⲥʺx2Aʺ_ЖDIGITↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _Ⲥ1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡Ↄ : _repeat
        {
            public _Ⲥ1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡Ↄ(__GeneratedV2.CstNodes.Inners._Ⲥ1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡Ↄ _Ⲥ1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡Ↄ_1)
            {
                this._Ⲥ1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡Ↄ_1 = _Ⲥ1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡Ↄ_1;
            }
            
            public __GeneratedV2.CstNodes.Inners._Ⲥ1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡Ↄ _Ⲥ1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡Ↄ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

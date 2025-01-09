namespace __Generated.CstNodes.Inners
{
    public abstract class _1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ
    {
        private _1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ._1ЖⲤʺx2Eʺ_1ЖDIGITↃ node, TContext context);
            protected internal abstract TResult Accept(_1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ._Ⲥʺx2Dʺ_1ЖDIGITↃ node, TContext context);
        }
        
        public sealed class _1ЖⲤʺx2Eʺ_1ЖDIGITↃ : _1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ
        {
            public _1ЖⲤʺx2Eʺ_1ЖDIGITↃ(System.Collections.Generic.IEnumerable<__Generated.CstNodes.Inners._Ⲥʺx2Eʺ_1ЖDIGITↃ> _Ⲥʺx2Eʺ_1ЖDIGITↃ_1)
            {
                this._Ⲥʺx2Eʺ_1ЖDIGITↃ_1 = _Ⲥʺx2Eʺ_1ЖDIGITↃ_1;
            }
            
            public System.Collections.Generic.IEnumerable<__Generated.CstNodes.Inners._Ⲥʺx2Eʺ_1ЖDIGITↃ> _Ⲥʺx2Eʺ_1ЖDIGITↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _Ⲥʺx2Dʺ_1ЖDIGITↃ : _1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ
        {
            public _Ⲥʺx2Dʺ_1ЖDIGITↃ(__Generated.CstNodes.Inners._Ⲥʺx2Dʺ_1ЖDIGITↃ _Ⲥʺx2Dʺ_1ЖDIGITↃ_1)
            {
                this._Ⲥʺx2Dʺ_1ЖDIGITↃ_1 = _Ⲥʺx2Dʺ_1ЖDIGITↃ_1;
            }
            
            public __Generated.CstNodes.Inners._Ⲥʺx2Dʺ_1ЖDIGITↃ _Ⲥʺx2Dʺ_1ЖDIGITↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

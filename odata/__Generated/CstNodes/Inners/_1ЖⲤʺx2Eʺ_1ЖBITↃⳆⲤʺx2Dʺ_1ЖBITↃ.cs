namespace __Generated.CstNodes.Inners
{
    public abstract class _1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ
    {
        private _1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ._1ЖⲤʺx2Eʺ_1ЖBITↃ node, TContext context);
            protected internal abstract TResult Accept(_1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ._Ⲥʺx2Dʺ_1ЖBITↃ node, TContext context);
        }
        
        public sealed class _1ЖⲤʺx2Eʺ_1ЖBITↃ : _1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ
        {
            public _1ЖⲤʺx2Eʺ_1ЖBITↃ(__Generated.CstNodes.Inners.HelperRangedAtLeast1<Inners._Ⲥʺx2Eʺ_1ЖBITↃ> _Ⲥʺx2Eʺ_1ЖBITↃ_1)
            {
                this._Ⲥʺx2Eʺ_1ЖBITↃ_1 = _Ⲥʺx2Eʺ_1ЖBITↃ_1;
            }
            
            public __Generated.CstNodes.Inners.HelperRangedAtLeast1<Inners._Ⲥʺx2Eʺ_1ЖBITↃ> _Ⲥʺx2Eʺ_1ЖBITↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _Ⲥʺx2Dʺ_1ЖBITↃ : _1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ
        {
            public _Ⲥʺx2Dʺ_1ЖBITↃ(__Generated.CstNodes.Inners._Ⲥʺx2Dʺ_1ЖBITↃ _Ⲥʺx2Dʺ_1ЖBITↃ_1)
            {
                this._Ⲥʺx2Dʺ_1ЖBITↃ_1 = _Ⲥʺx2Dʺ_1ЖBITↃ_1;
            }
            
            public __Generated.CstNodes.Inners._Ⲥʺx2Dʺ_1ЖBITↃ _Ⲥʺx2Dʺ_1ЖBITↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

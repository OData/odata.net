namespace __Generated.CstNodes.Inners
{
    public abstract class _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ
    {
        private _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃ node, TContext context);
            protected internal abstract TResult Accept(_1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ._Ⲥʺx2Dʺ_1ЖHEXDIGↃ node, TContext context);
        }
        
        public sealed class _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃ : _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ
        {
            public _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃ(__Generated.CstNodes.Inners.HelperRangedAtLeast1<__Generated.CstNodes.Inners._Ⲥʺx2Eʺ_1ЖHEXDIGↃ> _Ⲥʺx2Eʺ_1ЖHEXDIGↃ_1)
            {
                this._Ⲥʺx2Eʺ_1ЖHEXDIGↃ_1 = _Ⲥʺx2Eʺ_1ЖHEXDIGↃ_1;
            }
            
            public __Generated.CstNodes.Inners.HelperRangedAtLeast1<__Generated.CstNodes.Inners._Ⲥʺx2Eʺ_1ЖHEXDIGↃ> _Ⲥʺx2Eʺ_1ЖHEXDIGↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _Ⲥʺx2Dʺ_1ЖHEXDIGↃ : _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ
        {
            public _Ⲥʺx2Dʺ_1ЖHEXDIGↃ(__Generated.CstNodes.Inners._Ⲥʺx2Dʺ_1ЖHEXDIGↃ _Ⲥʺx2Dʺ_1ЖHEXDIGↃ_1)
            {
                this._Ⲥʺx2Dʺ_1ЖHEXDIGↃ_1 = _Ⲥʺx2Dʺ_1ЖHEXDIGↃ_1;
            }
            
            public __Generated.CstNodes.Inners._Ⲥʺx2Dʺ_1ЖHEXDIGↃ _Ⲥʺx2Dʺ_1ЖHEXDIGↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

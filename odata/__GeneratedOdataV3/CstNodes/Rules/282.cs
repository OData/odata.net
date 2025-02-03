namespace __GeneratedOdataV3.CstNodes.Rules
{
    public abstract class _nanInfinity
    {
        private _nanInfinity()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_nanInfinity node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_nanInfinity._ʺx4Ex61x4Eʺ node, TContext context);
            protected internal abstract TResult Accept(_nanInfinity._ʺx2Dx49x4Ex46ʺ node, TContext context);
            protected internal abstract TResult Accept(_nanInfinity._ʺx49x4Ex46ʺ node, TContext context);
        }
        
        public sealed class _ʺx4Ex61x4Eʺ : _nanInfinity
        {
            public _ʺx4Ex61x4Eʺ(__GeneratedOdataV3.CstNodes.Inners._ʺx4Ex61x4Eʺ _ʺx4Ex61x4Eʺ_1)
            {
                this._ʺx4Ex61x4Eʺ_1 = _ʺx4Ex61x4Eʺ_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx4Ex61x4Eʺ _ʺx4Ex61x4Eʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Dx49x4Ex46ʺ : _nanInfinity
        {
            public _ʺx2Dx49x4Ex46ʺ(__GeneratedOdataV3.CstNodes.Inners._ʺx2Dx49x4Ex46ʺ _ʺx2Dx49x4Ex46ʺ_1)
            {
                this._ʺx2Dx49x4Ex46ʺ_1 = _ʺx2Dx49x4Ex46ʺ_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx2Dx49x4Ex46ʺ _ʺx2Dx49x4Ex46ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx49x4Ex46ʺ : _nanInfinity
        {
            public _ʺx49x4Ex46ʺ(__GeneratedOdataV3.CstNodes.Inners._ʺx49x4Ex46ʺ _ʺx49x4Ex46ʺ_1)
            {
                this._ʺx49x4Ex46ʺ_1 = _ʺx49x4Ex46ʺ_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx49x4Ex46ʺ _ʺx49x4Ex46ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

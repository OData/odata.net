namespace __GeneratedOdataV2.CstNodes.Rules
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
            private _ʺx4Ex61x4Eʺ()
            {
                this._ʺx4Ex61x4Eʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx4Ex61x4Eʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx4Ex61x4Eʺ _ʺx4Ex61x4Eʺ_1 { get; }
            public static _ʺx4Ex61x4Eʺ Instance { get; } = new _ʺx4Ex61x4Eʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Dx49x4Ex46ʺ : _nanInfinity
        {
            private _ʺx2Dx49x4Ex46ʺ()
            {
                this._ʺx2Dx49x4Ex46ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx2Dx49x4Ex46ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx2Dx49x4Ex46ʺ _ʺx2Dx49x4Ex46ʺ_1 { get; }
            public static _ʺx2Dx49x4Ex46ʺ Instance { get; } = new _ʺx2Dx49x4Ex46ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx49x4Ex46ʺ : _nanInfinity
        {
            private _ʺx49x4Ex46ʺ()
            {
                this._ʺx49x4Ex46ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx49x4Ex46ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx49x4Ex46ʺ _ʺx49x4Ex46ʺ_1 { get; }
            public static _ʺx49x4Ex46ʺ Instance { get; } = new _ʺx49x4Ex46ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

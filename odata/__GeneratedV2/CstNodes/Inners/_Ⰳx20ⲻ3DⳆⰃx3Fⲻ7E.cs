namespace __GeneratedV2.CstNodes.Inners
{
    public abstract class _Ⰳx20ⲻ3DⳆⰃx3Fⲻ7E
    {
        private _Ⰳx20ⲻ3DⳆⰃx3Fⲻ7E()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_Ⰳx20ⲻ3DⳆⰃx3Fⲻ7E node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_Ⰳx20ⲻ3DⳆⰃx3Fⲻ7E._Ⰳx20ⲻ3D node, TContext context);
            protected internal abstract TResult Accept(_Ⰳx20ⲻ3DⳆⰃx3Fⲻ7E._Ⰳx3Fⲻ7E node, TContext context);
        }
        
        public sealed class _Ⰳx20ⲻ3D : _Ⰳx20ⲻ3DⳆⰃx3Fⲻ7E
        {
            public _Ⰳx20ⲻ3D(__GeneratedV2.CstNodes.Inners._Ⰳx20ⲻ3D _Ⰳx20ⲻ3D_1)
            {
                this._Ⰳx20ⲻ3D_1 = _Ⰳx20ⲻ3D_1;
            }
            
            public __GeneratedV2.CstNodes.Inners._Ⰳx20ⲻ3D _Ⰳx20ⲻ3D_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _Ⰳx3Fⲻ7E : _Ⰳx20ⲻ3DⳆⰃx3Fⲻ7E
        {
            public _Ⰳx3Fⲻ7E(__GeneratedV2.CstNodes.Inners._Ⰳx3Fⲻ7E _Ⰳx3Fⲻ7E_1)
            {
                this._Ⰳx3Fⲻ7E_1 = _Ⰳx3Fⲻ7E_1;
            }
            
            public __GeneratedV2.CstNodes.Inners._Ⰳx3Fⲻ7E _Ⰳx3Fⲻ7E_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

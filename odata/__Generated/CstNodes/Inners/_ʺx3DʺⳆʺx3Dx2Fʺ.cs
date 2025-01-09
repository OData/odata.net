namespace __Generated.CstNodes.Inners
{
    public abstract class _ʺx3DʺⳆʺx3Dx2Fʺ
    {
        private _ʺx3DʺⳆʺx3Dx2Fʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx3DʺⳆʺx3Dx2Fʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx3DʺⳆʺx3Dx2Fʺ._ʺx3Dʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx3DʺⳆʺx3Dx2Fʺ._ʺx3Dx2Fʺ node, TContext context);
        }
        
        public sealed class _ʺx3Dʺ : _ʺx3DʺⳆʺx3Dx2Fʺ
        {
            public _ʺx3Dʺ(__Generated.CstNodes.Inners._ʺx3Dʺ _ʺx3Dʺ_1)
            {
                this._ʺx3Dʺ_1 = _ʺx3Dʺ_1;
            }
            
            public __Generated.CstNodes.Inners._ʺx3Dʺ _ʺx3Dʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx3Dx2Fʺ : _ʺx3DʺⳆʺx3Dx2Fʺ
        {
            public _ʺx3Dx2Fʺ(__Generated.CstNodes.Inners._ʺx3Dx2Fʺ _ʺx3Dx2Fʺ_1)
            {
                this._ʺx3Dx2Fʺ_1 = _ʺx3Dx2Fʺ_1;
            }
            
            public __Generated.CstNodes.Inners._ʺx3Dx2Fʺ _ʺx3Dx2Fʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

namespace __GeneratedOdata.CstNodes.Inners
{
    public abstract class _ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ
    {
        private _ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ._ʺx24x74x6Fx70ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ._ʺx74x6Fx70ʺ node, TContext context);
        }
        
        public sealed class _ʺx24x74x6Fx70ʺ : _ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ
        {
            public _ʺx24x74x6Fx70ʺ(__GeneratedOdata.CstNodes.Inners._ʺx24x74x6Fx70ʺ _ʺx24x74x6Fx70ʺ_1)
            {
                this._ʺx24x74x6Fx70ʺ_1 = _ʺx24x74x6Fx70ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx24x74x6Fx70ʺ _ʺx24x74x6Fx70ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx74x6Fx70ʺ : _ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ
        {
            public _ʺx74x6Fx70ʺ(__GeneratedOdata.CstNodes.Inners._ʺx74x6Fx70ʺ _ʺx74x6Fx70ʺ_1)
            {
                this._ʺx74x6Fx70ʺ_1 = _ʺx74x6Fx70ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx74x6Fx70ʺ _ʺx74x6Fx70ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

namespace __GeneratedOdata.CstNodes.Inners
{
    public abstract class _ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ
    {
        private _ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ._ʺx2Fx24x65x6Ex74x69x74x79ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ._ʺx2Fx24x64x65x6Cx74x61ʺ node, TContext context);
        }
        
        public sealed class _ʺx2Fx24x65x6Ex74x69x74x79ʺ : _ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ
        {
            public _ʺx2Fx24x65x6Ex74x69x74x79ʺ(__GeneratedOdata.CstNodes.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺ _ʺx2Fx24x65x6Ex74x69x74x79ʺ_1)
            {
                this._ʺx2Fx24x65x6Ex74x69x74x79ʺ_1 = _ʺx2Fx24x65x6Ex74x69x74x79ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺ _ʺx2Fx24x65x6Ex74x69x74x79ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Fx24x64x65x6Cx74x61ʺ : _ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ
        {
            public _ʺx2Fx24x64x65x6Cx74x61ʺ(__GeneratedOdata.CstNodes.Inners._ʺx2Fx24x64x65x6Cx74x61ʺ _ʺx2Fx24x64x65x6Cx74x61ʺ_1)
            {
                this._ʺx2Fx24x64x65x6Cx74x61ʺ_1 = _ʺx2Fx24x64x65x6Cx74x61ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx2Fx24x64x65x6Cx74x61ʺ _ʺx2Fx24x64x65x6Cx74x61ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

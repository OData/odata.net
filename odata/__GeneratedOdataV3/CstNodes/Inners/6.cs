namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ
    {
        private _ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ._ʺx68x74x74x70x73ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ._ʺx68x74x74x70ʺ node, TContext context);
        }
        
        public sealed class _ʺx68x74x74x70x73ʺ : _ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ
        {
            public _ʺx68x74x74x70x73ʺ(__GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70x73ʺ _ʺx68x74x74x70x73ʺ_1)
            {
                this._ʺx68x74x74x70x73ʺ_1 = _ʺx68x74x74x70x73ʺ_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70x73ʺ _ʺx68x74x74x70x73ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx68x74x74x70ʺ : _ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ
        {
            public _ʺx68x74x74x70ʺ(__GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70ʺ _ʺx68x74x74x70ʺ_1)
            {
                this._ʺx68x74x74x70ʺ_1 = _ʺx68x74x74x70ʺ_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70ʺ _ʺx68x74x74x70ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

namespace __GeneratedOdataV2.CstNodes.Inners
{
    public abstract class _ʺx24x69x64ʺⳆʺx69x64ʺ
    {
        private _ʺx24x69x64ʺⳆʺx69x64ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx24x69x64ʺⳆʺx69x64ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx24x69x64ʺⳆʺx69x64ʺ._ʺx24x69x64ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx24x69x64ʺⳆʺx69x64ʺ._ʺx69x64ʺ node, TContext context);
        }
        
        public sealed class _ʺx24x69x64ʺ : _ʺx24x69x64ʺⳆʺx69x64ʺ
        {
            public _ʺx24x69x64ʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx24x69x64ʺ _ʺx24x69x64ʺ_1)
            {
                this._ʺx24x69x64ʺ_1 = _ʺx24x69x64ʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx24x69x64ʺ _ʺx24x69x64ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx69x64ʺ : _ʺx24x69x64ʺⳆʺx69x64ʺ
        {
            public _ʺx69x64ʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx69x64ʺ _ʺx69x64ʺ_1)
            {
                this._ʺx69x64ʺ_1 = _ʺx69x64ʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx69x64ʺ _ʺx69x64ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ
    {
        private _ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx24x66x69x6Cx74x65x72ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx66x69x6Cx74x65x72ʺ node, TContext context);
        }
        
        public sealed class _ʺx24x66x69x6Cx74x65x72ʺ : _ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ
        {
            public _ʺx24x66x69x6Cx74x65x72ʺ(__GeneratedOdataV3.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺ _ʺx24x66x69x6Cx74x65x72ʺ_1)
            {
                this._ʺx24x66x69x6Cx74x65x72ʺ_1 = _ʺx24x66x69x6Cx74x65x72ʺ_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺ _ʺx24x66x69x6Cx74x65x72ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx66x69x6Cx74x65x72ʺ : _ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ
        {
            public _ʺx66x69x6Cx74x65x72ʺ(__GeneratedOdataV3.CstNodes.Inners._ʺx66x69x6Cx74x65x72ʺ _ʺx66x69x6Cx74x65x72ʺ_1)
            {
                this._ʺx66x69x6Cx74x65x72ʺ_1 = _ʺx66x69x6Cx74x65x72ʺ_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx66x69x6Cx74x65x72ʺ _ʺx66x69x6Cx74x65x72ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

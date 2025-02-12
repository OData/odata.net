namespace __GeneratedOdataV2.CstNodes.Inners
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
            private _ʺx24x66x69x6Cx74x65x72ʺ()
            {
                this._ʺx24x66x69x6Cx74x65x72ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺ _ʺx24x66x69x6Cx74x65x72ʺ_1 { get; }
            public static _ʺx24x66x69x6Cx74x65x72ʺ Instance { get; } = new _ʺx24x66x69x6Cx74x65x72ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx66x69x6Cx74x65x72ʺ : _ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ
        {
            private _ʺx66x69x6Cx74x65x72ʺ()
            {
                this._ʺx66x69x6Cx74x65x72ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx66x69x6Cx74x65x72ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx66x69x6Cx74x65x72ʺ _ʺx66x69x6Cx74x65x72ʺ_1 { get; }
            public static _ʺx66x69x6Cx74x65x72ʺ Instance { get; } = new _ʺx66x69x6Cx74x65x72ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

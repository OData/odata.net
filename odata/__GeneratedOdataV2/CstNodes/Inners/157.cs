namespace __GeneratedOdataV2.CstNodes.Inners
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
            private _ʺx24x74x6Fx70ʺ()
            {
                this._ʺx24x74x6Fx70ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx24x74x6Fx70ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx24x74x6Fx70ʺ _ʺx24x74x6Fx70ʺ_1 { get; }
            public static _ʺx24x74x6Fx70ʺ Instance { get; } = new _ʺx24x74x6Fx70ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx74x6Fx70ʺ : _ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ
        {
            private _ʺx74x6Fx70ʺ()
            {
                this._ʺx74x6Fx70ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx74x6Fx70ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx74x6Fx70ʺ _ʺx74x6Fx70ʺ_1 { get; }
            public static _ʺx74x6Fx70ʺ Instance { get; } = new _ʺx74x6Fx70ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

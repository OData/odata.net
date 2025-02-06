namespace __GeneratedOdataV4.CstNodes.Inners
{
    public abstract class _ʺx2FʺⳆʺx25x32x46ʺ
    {
        private _ʺx2FʺⳆʺx25x32x46ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx2FʺⳆʺx25x32x46ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx2FʺⳆʺx25x32x46ʺ._ʺx2Fʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx2FʺⳆʺx25x32x46ʺ._ʺx25x32x46ʺ node, TContext context);
        }
        
        public sealed class _ʺx2Fʺ : _ʺx2FʺⳆʺx25x32x46ʺ
        {
            private _ʺx2Fʺ()
            {
                this._ʺx2Fʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1 { get; }
            public static _ʺx2Fʺ Instance { get; } = new _ʺx2Fʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x32x46ʺ : _ʺx2FʺⳆʺx25x32x46ʺ
        {
            private _ʺx25x32x46ʺ()
            {
                this._ʺx25x32x46ʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx25x32x46ʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx25x32x46ʺ _ʺx25x32x46ʺ_1 { get; }
            public static _ʺx25x32x46ʺ Instance { get; } = new _ʺx25x32x46ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

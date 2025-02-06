namespace __GeneratedOdataV4.CstNodes.Inners
{
    public abstract class _ʺx7DʺⳆʺx25x37x44ʺ
    {
        private _ʺx7DʺⳆʺx25x37x44ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx7DʺⳆʺx25x37x44ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx7DʺⳆʺx25x37x44ʺ._ʺx7Dʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx7DʺⳆʺx25x37x44ʺ._ʺx25x37x44ʺ node, TContext context);
        }
        
        public sealed class _ʺx7Dʺ : _ʺx7DʺⳆʺx25x37x44ʺ
        {
            private _ʺx7Dʺ()
            {
                this._ʺx7Dʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx7Dʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx7Dʺ _ʺx7Dʺ_1 { get; }
            public static _ʺx7Dʺ Instance { get; } = new _ʺx7Dʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x37x44ʺ : _ʺx7DʺⳆʺx25x37x44ʺ
        {
            private _ʺx25x37x44ʺ()
            {
                this._ʺx25x37x44ʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx25x37x44ʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx25x37x44ʺ _ʺx25x37x44ʺ_1 { get; }
            public static _ʺx25x37x44ʺ Instance { get; } = new _ʺx25x37x44ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

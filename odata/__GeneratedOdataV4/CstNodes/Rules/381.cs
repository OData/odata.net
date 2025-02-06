namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _OPEN
    {
        private _OPEN()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_OPEN node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_OPEN._ʺx28ʺ node, TContext context);
            protected internal abstract TResult Accept(_OPEN._ʺx25x32x38ʺ node, TContext context);
        }
        
        public sealed class _ʺx28ʺ : _OPEN
        {
            private _ʺx28ʺ()
            {
                this._ʺx28ʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx28ʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx28ʺ _ʺx28ʺ_1 { get; }
            public static _ʺx28ʺ Instance { get; } = new _ʺx28ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x32x38ʺ : _OPEN
        {
            private _ʺx25x32x38ʺ()
            {
                this._ʺx25x32x38ʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx25x32x38ʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx25x32x38ʺ _ʺx25x32x38ʺ_1 { get; }
            public static _ʺx25x32x38ʺ Instance { get; } = new _ʺx25x32x38ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

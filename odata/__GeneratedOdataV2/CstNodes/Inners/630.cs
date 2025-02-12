namespace __GeneratedOdataV2.CstNodes.Inners
{
    public abstract class _SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ
    {
        private _SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._SP node, TContext context);
            protected internal abstract TResult Accept(_SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._HTAB node, TContext context);
            protected internal abstract TResult Accept(_SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._ʺx25x32x30ʺ node, TContext context);
            protected internal abstract TResult Accept(_SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._ʺx25x30x39ʺ node, TContext context);
        }
        
        public sealed class _SP : _SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ
        {
            private _SP()
            {
                this._SP_1 = __GeneratedOdataV2.CstNodes.Rules._SP.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._SP _SP_1 { get; }
            public static _SP Instance { get; } = new _SP();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _HTAB : _SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ
        {
            private _HTAB()
            {
                this._HTAB_1 = __GeneratedOdataV2.CstNodes.Rules._HTAB.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._HTAB _HTAB_1 { get; }
            public static _HTAB Instance { get; } = new _HTAB();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x32x30ʺ : _SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ
        {
            private _ʺx25x32x30ʺ()
            {
                this._ʺx25x32x30ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx25x32x30ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx25x32x30ʺ _ʺx25x32x30ʺ_1 { get; }
            public static _ʺx25x32x30ʺ Instance { get; } = new _ʺx25x32x30ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x30x39ʺ : _SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ
        {
            private _ʺx25x30x39ʺ()
            {
                this._ʺx25x30x39ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx25x30x39ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx25x30x39ʺ _ʺx25x30x39ʺ_1 { get; }
            public static _ʺx25x30x39ʺ Instance { get; } = new _ʺx25x30x39ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

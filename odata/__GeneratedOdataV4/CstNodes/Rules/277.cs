namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _base64char
    {
        private _base64char()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_base64char node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_base64char._ALPHA node, TContext context);
            protected internal abstract TResult Accept(_base64char._DIGIT node, TContext context);
            protected internal abstract TResult Accept(_base64char._ʺx2Dʺ node, TContext context);
            protected internal abstract TResult Accept(_base64char._ʺx5Fʺ node, TContext context);
        }
        
        public sealed class _ALPHA : _base64char
        {
            public _ALPHA(__GeneratedOdataV4.CstNodes.Rules._ALPHA _ALPHA_1)
            {
                this._ALPHA_1 = _ALPHA_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._ALPHA _ALPHA_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _DIGIT : _base64char
        {
            public _DIGIT(__GeneratedOdataV4.CstNodes.Rules._DIGIT _DIGIT_1)
            {
                this._DIGIT_1 = _DIGIT_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._DIGIT _DIGIT_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Dʺ : _base64char
        {
            private _ʺx2Dʺ()
            {
                this._ʺx2Dʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx2Dʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx2Dʺ _ʺx2Dʺ_1 { get; }
            public static _ʺx2Dʺ Instance { get; } = new _ʺx2Dʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx5Fʺ : _base64char
        {
            private _ʺx5Fʺ()
            {
                this._ʺx5Fʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx5Fʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx5Fʺ _ʺx5Fʺ_1 { get; }
            public static _ʺx5Fʺ Instance { get; } = new _ʺx5Fʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

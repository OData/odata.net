namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _identifierLeadingCharacter
    {
        private _identifierLeadingCharacter()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_identifierLeadingCharacter node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_identifierLeadingCharacter._ALPHA node, TContext context);
            protected internal abstract TResult Accept(_identifierLeadingCharacter._ʺx5Fʺ node, TContext context);
        }
        
        public sealed class _ALPHA : _identifierLeadingCharacter
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
        
        public sealed class _ʺx5Fʺ : _identifierLeadingCharacter
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

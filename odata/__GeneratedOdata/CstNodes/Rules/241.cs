namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _identifierCharacter
    {
        private _identifierCharacter()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_identifierCharacter node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_identifierCharacter._ALPHA node, TContext context);
            protected internal abstract TResult Accept(_identifierCharacter._ʺx5Fʺ node, TContext context);
            protected internal abstract TResult Accept(_identifierCharacter._DIGIT node, TContext context);
        }
        
        public sealed class _ALPHA : _identifierCharacter
        {
            public _ALPHA(__GeneratedOdata.CstNodes.Rules._ALPHA _ALPHA_1)
            {
                this._ALPHA_1 = _ALPHA_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._ALPHA _ALPHA_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx5Fʺ : _identifierCharacter
        {
            public _ʺx5Fʺ(__GeneratedOdata.CstNodes.Inners._ʺx5Fʺ _ʺx5Fʺ_1)
            {
                this._ʺx5Fʺ_1 = _ʺx5Fʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx5Fʺ _ʺx5Fʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _DIGIT : _identifierCharacter
        {
            public _DIGIT(__GeneratedOdata.CstNodes.Rules._DIGIT _DIGIT_1)
            {
                this._DIGIT_1 = _DIGIT_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._DIGIT _DIGIT_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

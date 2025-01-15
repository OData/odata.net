namespace __GeneratedV2.CstNodes.Rules
{
    public abstract class _HEXDIG
    {
        private _HEXDIG()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_HEXDIG node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_HEXDIG._DIGIT node, TContext context);
            protected internal abstract TResult Accept(_HEXDIG._ʺx41ʺ node, TContext context);
            protected internal abstract TResult Accept(_HEXDIG._ʺx42ʺ node, TContext context);
            protected internal abstract TResult Accept(_HEXDIG._ʺx43ʺ node, TContext context);
            protected internal abstract TResult Accept(_HEXDIG._ʺx44ʺ node, TContext context);
            protected internal abstract TResult Accept(_HEXDIG._ʺx45ʺ node, TContext context);
            protected internal abstract TResult Accept(_HEXDIG._ʺx46ʺ node, TContext context);
        }
        
        public sealed class _DIGIT : _HEXDIG
        {
            public _DIGIT(__GeneratedV2.CstNodes.Rules._DIGIT _DIGIT_1)
            {
                this._DIGIT_1 = _DIGIT_1;
            }
            
            public __GeneratedV2.CstNodes.Rules._DIGIT _DIGIT_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx41ʺ : _HEXDIG
        {
            public _ʺx41ʺ(__GeneratedV2.CstNodes.Inners._ʺx41ʺ _ʺx41ʺ_1)
            {
                this._ʺx41ʺ_1 = _ʺx41ʺ_1;
            }
            
            public __GeneratedV2.CstNodes.Inners._ʺx41ʺ _ʺx41ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx42ʺ : _HEXDIG
        {
            public _ʺx42ʺ(__GeneratedV2.CstNodes.Inners._ʺx42ʺ _ʺx42ʺ_1)
            {
                this._ʺx42ʺ_1 = _ʺx42ʺ_1;
            }
            
            public __GeneratedV2.CstNodes.Inners._ʺx42ʺ _ʺx42ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx43ʺ : _HEXDIG
        {
            public _ʺx43ʺ(__GeneratedV2.CstNodes.Inners._ʺx43ʺ _ʺx43ʺ_1)
            {
                this._ʺx43ʺ_1 = _ʺx43ʺ_1;
            }
            
            public __GeneratedV2.CstNodes.Inners._ʺx43ʺ _ʺx43ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx44ʺ : _HEXDIG
        {
            public _ʺx44ʺ(__GeneratedV2.CstNodes.Inners._ʺx44ʺ _ʺx44ʺ_1)
            {
                this._ʺx44ʺ_1 = _ʺx44ʺ_1;
            }
            
            public __GeneratedV2.CstNodes.Inners._ʺx44ʺ _ʺx44ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx45ʺ : _HEXDIG
        {
            public _ʺx45ʺ(__GeneratedV2.CstNodes.Inners._ʺx45ʺ _ʺx45ʺ_1)
            {
                this._ʺx45ʺ_1 = _ʺx45ʺ_1;
            }
            
            public __GeneratedV2.CstNodes.Inners._ʺx45ʺ _ʺx45ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx46ʺ : _HEXDIG
        {
            public _ʺx46ʺ(__GeneratedV2.CstNodes.Inners._ʺx46ʺ _ʺx46ʺ_1)
            {
                this._ʺx46ʺ_1 = _ʺx46ʺ_1;
            }
            
            public __GeneratedV2.CstNodes.Inners._ʺx46ʺ _ʺx46ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

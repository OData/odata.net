namespace __Generated.CstNodes.Inners
{
    public abstract class _WSPⳆVCHAR
    {
        private _WSPⳆVCHAR()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_WSPⳆVCHAR node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_WSPⳆVCHAR._WSP node, TContext context);
            protected internal abstract TResult Accept(_WSPⳆVCHAR._VCHAR node, TContext context);
        }
        
        public sealed class _WSP : _WSPⳆVCHAR
        {
            public _WSP(__Generated.CstNodes.Rules._WSP _WSP_1)
            {
                this._WSP_1 = _WSP_1;
            }
            
            public __Generated.CstNodes.Rules._WSP _WSP_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _VCHAR : _WSPⳆVCHAR
        {
            public _VCHAR(__Generated.CstNodes.Rules._VCHAR _VCHAR_1)
            {
                this._VCHAR_1 = _VCHAR_1;
            }
            
            public __Generated.CstNodes.Rules._VCHAR _VCHAR_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

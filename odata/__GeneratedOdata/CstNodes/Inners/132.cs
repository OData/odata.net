namespace __GeneratedOdata.CstNodes.Inners
{
    public abstract class _oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ
    {
        private _oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ._oneToNine_ЖDIGIT node, TContext context);
            protected internal abstract TResult Accept(_oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ._ʺx6Dx61x78ʺ node, TContext context);
        }
        
        public sealed class _oneToNine_ЖDIGIT : _oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ
        {
            public _oneToNine_ЖDIGIT(__GeneratedOdata.CstNodes.Rules._oneToNine _oneToNine_1, System.Collections.Generic.IEnumerable<__GeneratedOdata.CstNodes.Rules._DIGIT> _DIGIT_1)
            {
                this._oneToNine_1 = _oneToNine_1;
                this._DIGIT_1 = _DIGIT_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._oneToNine _oneToNine_1 { get; }
            public System.Collections.Generic.IEnumerable<__GeneratedOdata.CstNodes.Rules._DIGIT> _DIGIT_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx6Dx61x78ʺ : _oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ
        {
            public _ʺx6Dx61x78ʺ(__GeneratedOdata.CstNodes.Inners._ʺx6Dx61x78ʺ _ʺx6Dx61x78ʺ_1)
            {
                this._ʺx6Dx61x78ʺ_1 = _ʺx6Dx61x78ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx6Dx61x78ʺ _ʺx6Dx61x78ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
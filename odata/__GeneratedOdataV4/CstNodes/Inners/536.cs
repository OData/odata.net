namespace __GeneratedOdataV4.CstNodes.Inners
{
    public abstract class _ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT
    {
        private _ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT._ʺx30ʺ_3DIGIT node, TContext context);
            protected internal abstract TResult Accept(_ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT._oneToNine_3ЖDIGIT node, TContext context);
        }
        
        public sealed class _ʺx30ʺ_3DIGIT : _ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT
        {
            public _ʺx30ʺ_3DIGIT(__GeneratedOdataV4.CstNodes.Inners._ʺx30ʺ _ʺx30ʺ_1, __GeneratedOdataV4.CstNodes.Inners.HelperRangedExactly3<__GeneratedOdataV4.CstNodes.Rules._DIGIT> _DIGIT_1)
            {
                this._ʺx30ʺ_1 = _ʺx30ʺ_1;
                this._DIGIT_1 = _DIGIT_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx30ʺ _ʺx30ʺ_1 { get; }
            public __GeneratedOdataV4.CstNodes.Inners.HelperRangedExactly3<__GeneratedOdataV4.CstNodes.Rules._DIGIT> _DIGIT_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _oneToNine_3ЖDIGIT : _ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT
        {
            public _oneToNine_3ЖDIGIT(__GeneratedOdataV4.CstNodes.Rules._oneToNine _oneToNine_1, __GeneratedOdataV4.CstNodes.Inners.HelperRangedAtLeast3<__GeneratedOdataV4.CstNodes.Rules._DIGIT> _DIGIT_1)
            {
                this._oneToNine_1 = _oneToNine_1;
                this._DIGIT_1 = _DIGIT_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._oneToNine _oneToNine_1 { get; }
            public __GeneratedOdataV4.CstNodes.Inners.HelperRangedAtLeast3<__GeneratedOdataV4.CstNodes.Rules._DIGIT> _DIGIT_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _decⲻoctet
    {
        private _decⲻoctet()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_decⲻoctet node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_decⲻoctet._ʺx31ʺ_2DIGIT node, TContext context);
            protected internal abstract TResult Accept(_decⲻoctet._ʺx32ʺ_Ⰳx30ⲻ34_DIGIT node, TContext context);
            protected internal abstract TResult Accept(_decⲻoctet._ʺx32x35ʺ_Ⰳx30ⲻ35 node, TContext context);
            protected internal abstract TResult Accept(_decⲻoctet._Ⰳx31ⲻ39_DIGIT node, TContext context);
            protected internal abstract TResult Accept(_decⲻoctet._DIGIT node, TContext context);
        }
        
        public sealed class _ʺx31ʺ_2DIGIT : _decⲻoctet
        {
            public _ʺx31ʺ_2DIGIT(__GeneratedOdata.CstNodes.Inners._ʺx31ʺ _ʺx31ʺ_1, __GeneratedOdata.CstNodes.Inners.HelperRangedExactly2<__GeneratedOdata.CstNodes.Rules._DIGIT> _DIGIT_1)
            {
                this._ʺx31ʺ_1 = _ʺx31ʺ_1;
                this._DIGIT_1 = _DIGIT_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx31ʺ _ʺx31ʺ_1 { get; }
            public __GeneratedOdata.CstNodes.Inners.HelperRangedExactly2<__GeneratedOdata.CstNodes.Rules._DIGIT> _DIGIT_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx32ʺ_Ⰳx30ⲻ34_DIGIT : _decⲻoctet
        {
            public _ʺx32ʺ_Ⰳx30ⲻ34_DIGIT(__GeneratedOdata.CstNodes.Inners._ʺx32ʺ _ʺx32ʺ_1, __GeneratedOdata.CstNodes.Inners._Ⰳx30ⲻ34 _Ⰳx30ⲻ34_1, __GeneratedOdata.CstNodes.Rules._DIGIT _DIGIT_1)
            {
                this._ʺx32ʺ_1 = _ʺx32ʺ_1;
                this._Ⰳx30ⲻ34_1 = _Ⰳx30ⲻ34_1;
                this._DIGIT_1 = _DIGIT_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx32ʺ _ʺx32ʺ_1 { get; }
            public __GeneratedOdata.CstNodes.Inners._Ⰳx30ⲻ34 _Ⰳx30ⲻ34_1 { get; }
            public __GeneratedOdata.CstNodes.Rules._DIGIT _DIGIT_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx32x35ʺ_Ⰳx30ⲻ35 : _decⲻoctet
        {
            public _ʺx32x35ʺ_Ⰳx30ⲻ35(__GeneratedOdata.CstNodes.Inners._ʺx32x35ʺ _ʺx32x35ʺ_1, __GeneratedOdata.CstNodes.Inners._Ⰳx30ⲻ35 _Ⰳx30ⲻ35_1)
            {
                this._ʺx32x35ʺ_1 = _ʺx32x35ʺ_1;
                this._Ⰳx30ⲻ35_1 = _Ⰳx30ⲻ35_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx32x35ʺ _ʺx32x35ʺ_1 { get; }
            public __GeneratedOdata.CstNodes.Inners._Ⰳx30ⲻ35 _Ⰳx30ⲻ35_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _Ⰳx31ⲻ39_DIGIT : _decⲻoctet
        {
            public _Ⰳx31ⲻ39_DIGIT(__GeneratedOdata.CstNodes.Inners._Ⰳx31ⲻ39 _Ⰳx31ⲻ39_1, __GeneratedOdata.CstNodes.Rules._DIGIT _DIGIT_1)
            {
                this._Ⰳx31ⲻ39_1 = _Ⰳx31ⲻ39_1;
                this._DIGIT_1 = _DIGIT_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._Ⰳx31ⲻ39 _Ⰳx31ⲻ39_1 { get; }
            public __GeneratedOdata.CstNodes.Rules._DIGIT _DIGIT_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _DIGIT : _decⲻoctet
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

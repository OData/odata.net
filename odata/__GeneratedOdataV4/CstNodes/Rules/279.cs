namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _decimalValue
    {
        private _decimalValue()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_decimalValue node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_decimalValue._꘡SIGN꘡_1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_꘡ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT꘡ node, TContext context);
            protected internal abstract TResult Accept(_decimalValue._nanInfinity node, TContext context);
        }
        
        public sealed class _꘡SIGN꘡_1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_꘡ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT꘡ : _decimalValue
        {
            public _꘡SIGN꘡_1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_꘡ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT꘡(__GeneratedOdataV4.CstNodes.Rules._SIGN? _SIGN_1, __GeneratedOdataV4.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV4.CstNodes.Rules._DIGIT> _DIGIT_1, __GeneratedOdataV4.CstNodes.Inners._ʺx2Eʺ_1ЖDIGIT? _ʺx2Eʺ_1ЖDIGIT_1, __GeneratedOdataV4.CstNodes.Inners._ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT? _ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT_1)
            {
                this._SIGN_1 = _SIGN_1;
                this._DIGIT_1 = _DIGIT_1;
                this._ʺx2Eʺ_1ЖDIGIT_1 = _ʺx2Eʺ_1ЖDIGIT_1;
                this._ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT_1 = _ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._SIGN? _SIGN_1 { get; }
            public __GeneratedOdataV4.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV4.CstNodes.Rules._DIGIT> _DIGIT_1 { get; }
            public __GeneratedOdataV4.CstNodes.Inners._ʺx2Eʺ_1ЖDIGIT? _ʺx2Eʺ_1ЖDIGIT_1 { get; }
            public __GeneratedOdataV4.CstNodes.Inners._ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT? _ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _nanInfinity : _decimalValue
        {
            public _nanInfinity(__GeneratedOdataV4.CstNodes.Rules._nanInfinity _nanInfinity_1)
            {
                this._nanInfinity_1 = _nanInfinity_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._nanInfinity _nanInfinity_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

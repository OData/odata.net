namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _charInJSON
    {
        private _charInJSON()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_charInJSON node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_charInJSON._qcharⲻunescaped node, TContext context);
            protected internal abstract TResult Accept(_charInJSON._qcharⲻJSONⲻspecial node, TContext context);
            protected internal abstract TResult Accept(_charInJSON._escape_ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ node, TContext context);
        }
        
        public sealed class _qcharⲻunescaped : _charInJSON
        {
            public _qcharⲻunescaped(__GeneratedOdataV4.CstNodes.Rules._qcharⲻunescaped _qcharⲻunescaped_1)
            {
                this._qcharⲻunescaped_1 = _qcharⲻunescaped_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._qcharⲻunescaped _qcharⲻunescaped_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _qcharⲻJSONⲻspecial : _charInJSON
        {
            public _qcharⲻJSONⲻspecial(__GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial _qcharⲻJSONⲻspecial_1)
            {
                this._qcharⲻJSONⲻspecial_1 = _qcharⲻJSONⲻspecial_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial _qcharⲻJSONⲻspecial_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _escape_ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ : _charInJSON
        {
            public _escape_ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ(__GeneratedOdataV4.CstNodes.Rules._escape _escape_1, __GeneratedOdataV4.CstNodes.Inners._ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ _ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ_1)
            {
                this._escape_1 = _escape_1;
                this._ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ_1 = _ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._escape _escape_1 { get; }
            public __GeneratedOdataV4.CstNodes.Inners._ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ _ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

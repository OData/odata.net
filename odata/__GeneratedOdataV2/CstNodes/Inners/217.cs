namespace __GeneratedOdataV2.CstNodes.Inners
{
    public abstract class _OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty
    {
        private _OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSE node, TContext context);
            protected internal abstract TResult Accept(_OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty._ʺx2Fʺ_selectProperty node, TContext context);
        }
        
        public sealed class _OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSE : _OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty
        {
            public _OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSE(__GeneratedOdataV2.CstNodes.Rules._OPEN _OPEN_1, __GeneratedOdataV2.CstNodes.Rules._selectOption _selectOption_1, System.Collections.Generic.IEnumerable<Inners._ⲤSEMI_selectOptionↃ> _ⲤSEMI_selectOptionↃ_1, __GeneratedOdataV2.CstNodes.Rules._CLOSE _CLOSE_1)
            {
                this._OPEN_1 = _OPEN_1;
                this._selectOption_1 = _selectOption_1;
                this._ⲤSEMI_selectOptionↃ_1 = _ⲤSEMI_selectOptionↃ_1;
                this._CLOSE_1 = _CLOSE_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._OPEN _OPEN_1 { get; }
            public __GeneratedOdataV2.CstNodes.Rules._selectOption _selectOption_1 { get; }
            public System.Collections.Generic.IEnumerable<Inners._ⲤSEMI_selectOptionↃ> _ⲤSEMI_selectOptionↃ_1 { get; }
            public __GeneratedOdataV2.CstNodes.Rules._CLOSE _CLOSE_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Fʺ_selectProperty : _OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty
        {
            public _ʺx2Fʺ_selectProperty(__GeneratedOdataV2.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1, __GeneratedOdataV2.CstNodes.Rules._selectProperty _selectProperty_1)
            {
                this._ʺx2Fʺ_1 = _ʺx2Fʺ_1;
                this._selectProperty_1 = _selectProperty_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1 { get; }
            public __GeneratedOdataV2.CstNodes.Rules._selectProperty _selectProperty_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

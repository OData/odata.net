namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _selectItem
    {
        private _selectItem()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_selectItem node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_selectItem._STAR node, TContext context);
            protected internal abstract TResult Accept(_selectItem._allOperationsInSchema node, TContext context);
            protected internal abstract TResult Accept(_selectItem._꘡ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ꘡_ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ node, TContext context);
        }
        
        public sealed class _STAR : _selectItem
        {
            public _STAR(__GeneratedOdata.CstNodes.Rules._STAR _STAR_1)
            {
                this._STAR_1 = _STAR_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._STAR _STAR_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _allOperationsInSchema : _selectItem
        {
            public _allOperationsInSchema(__GeneratedOdata.CstNodes.Rules._allOperationsInSchema _allOperationsInSchema_1)
            {
                this._allOperationsInSchema_1 = _allOperationsInSchema_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._allOperationsInSchema _allOperationsInSchema_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _꘡ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ꘡_ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ : _selectItem
        {
            public _꘡ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ꘡_ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ(__GeneratedOdata.CstNodes.Inners._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ? _ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ_1, __GeneratedOdata.CstNodes.Inners._ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ _ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ_1)
            {
                this._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ_1 = _ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ_1;
                this._ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ_1 = _ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ? _ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ_1 { get; }
            public __GeneratedOdata.CstNodes.Inners._ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ _ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

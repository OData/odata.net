namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _selectListItem
    {
        private _selectListItem()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_selectListItem node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_selectListItem._STAR node, TContext context);
            protected internal abstract TResult Accept(_selectListItem._allOperationsInSchema node, TContext context);
            protected internal abstract TResult Accept(_selectListItem._꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ node, TContext context);
        }
        
        public sealed class _STAR : _selectListItem
        {
            public _STAR(__GeneratedOdataV4.CstNodes.Rules._STAR _STAR_1)
            {
                this._STAR_1 = _STAR_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._STAR _STAR_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _allOperationsInSchema : _selectListItem
        {
            public _allOperationsInSchema(__GeneratedOdataV4.CstNodes.Rules._allOperationsInSchema _allOperationsInSchema_1)
            {
                this._allOperationsInSchema_1 = _allOperationsInSchema_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._allOperationsInSchema _allOperationsInSchema_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ : _selectListItem
        {
            public _꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ(__GeneratedOdataV4.CstNodes.Inners._qualifiedEntityTypeName_ʺx2Fʺ? _qualifiedEntityTypeName_ʺx2Fʺ_1, __GeneratedOdataV4.CstNodes.Inners._ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ _ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ_1)
            {
                this._qualifiedEntityTypeName_ʺx2Fʺ_1 = _qualifiedEntityTypeName_ʺx2Fʺ_1;
                this._ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ_1 = _ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._qualifiedEntityTypeName_ʺx2Fʺ? _qualifiedEntityTypeName_ʺx2Fʺ_1 { get; }
            public __GeneratedOdataV4.CstNodes.Inners._ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ _ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

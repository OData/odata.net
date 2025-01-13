namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _selectProperty
    {
        private _selectProperty()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_selectProperty node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_selectProperty._primitiveProperty node, TContext context);
            protected internal abstract TResult Accept(_selectProperty._primitiveColProperty_꘡OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE꘡ node, TContext context);
            protected internal abstract TResult Accept(_selectProperty._navigationProperty node, TContext context);
            protected internal abstract TResult Accept(_selectProperty._selectPath_꘡OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty꘡ node, TContext context);
        }
        
        public sealed class _primitiveProperty : _selectProperty
        {
            public _primitiveProperty(__GeneratedOdata.CstNodes.Rules._primitiveProperty _primitiveProperty_1)
            {
                this._primitiveProperty_1 = _primitiveProperty_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._primitiveProperty _primitiveProperty_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _primitiveColProperty_꘡OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE꘡ : _selectProperty
        {
            public _primitiveColProperty_꘡OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE꘡(__GeneratedOdata.CstNodes.Rules._primitiveColProperty _primitiveColProperty_1, __GeneratedOdata.CstNodes.Inners._OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE? _OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE_1)
            {
                this._primitiveColProperty_1 = _primitiveColProperty_1;
                this._OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE_1 = _OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._primitiveColProperty _primitiveColProperty_1 { get; }
            public __GeneratedOdata.CstNodes.Inners._OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE? _OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _navigationProperty : _selectProperty
        {
            public _navigationProperty(__GeneratedOdata.CstNodes.Rules._navigationProperty _navigationProperty_1)
            {
                this._navigationProperty_1 = _navigationProperty_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._navigationProperty _navigationProperty_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _selectPath_꘡OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty꘡ : _selectProperty
        {
            public _selectPath_꘡OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty꘡(__GeneratedOdata.CstNodes.Rules._selectPath _selectPath_1, __GeneratedOdata.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty? _OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty_1)
            {
                this._selectPath_1 = _selectPath_1;
                this._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty_1 = _OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._selectPath _selectPath_1 { get; }
            public __GeneratedOdata.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty? _OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _selectListProperty
    {
        private _selectListProperty()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_selectListProperty node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_selectListProperty._primitiveProperty node, TContext context);
            protected internal abstract TResult Accept(_selectListProperty._primitiveColProperty node, TContext context);
            protected internal abstract TResult Accept(_selectListProperty._navigationProperty_꘡ʺx2Bʺ꘡_꘡selectList꘡ node, TContext context);
            protected internal abstract TResult Accept(_selectListProperty._selectPath_꘡ʺx2Fʺ_selectListProperty꘡ node, TContext context);
        }
        
        public sealed class _primitiveProperty : _selectListProperty
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
        
        public sealed class _primitiveColProperty : _selectListProperty
        {
            public _primitiveColProperty(__GeneratedOdata.CstNodes.Rules._primitiveColProperty _primitiveColProperty_1)
            {
                this._primitiveColProperty_1 = _primitiveColProperty_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._primitiveColProperty _primitiveColProperty_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _navigationProperty_꘡ʺx2Bʺ꘡_꘡selectList꘡ : _selectListProperty
        {
            public _navigationProperty_꘡ʺx2Bʺ꘡_꘡selectList꘡(__GeneratedOdata.CstNodes.Rules._navigationProperty _navigationProperty_1, __GeneratedOdata.CstNodes.Inners._ʺx2Bʺ? _ʺx2Bʺ_1, __GeneratedOdata.CstNodes.Rules._selectList? _selectList_1)
            {
                this._navigationProperty_1 = _navigationProperty_1;
                this._ʺx2Bʺ_1 = _ʺx2Bʺ_1;
                this._selectList_1 = _selectList_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._navigationProperty _navigationProperty_1 { get; }
            public __GeneratedOdata.CstNodes.Inners._ʺx2Bʺ? _ʺx2Bʺ_1 { get; }
            public __GeneratedOdata.CstNodes.Rules._selectList? _selectList_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _selectPath_꘡ʺx2Fʺ_selectListProperty꘡ : _selectListProperty
        {
            public _selectPath_꘡ʺx2Fʺ_selectListProperty꘡(__GeneratedOdata.CstNodes.Rules._selectPath _selectPath_1, __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_selectListProperty? _ʺx2Fʺ_selectListProperty_1)
            {
                this._selectPath_1 = _selectPath_1;
                this._ʺx2Fʺ_selectListProperty_1 = _ʺx2Fʺ_selectListProperty_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._selectPath _selectPath_1 { get; }
            public __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_selectListProperty? _ʺx2Fʺ_selectListProperty_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
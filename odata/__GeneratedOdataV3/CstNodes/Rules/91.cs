namespace __GeneratedOdataV3.CstNodes.Rules
{
    public abstract class _selectOptionPC
    {
        private _selectOptionPC()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_selectOptionPC node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_selectOptionPC._filter node, TContext context);
            protected internal abstract TResult Accept(_selectOptionPC._search node, TContext context);
            protected internal abstract TResult Accept(_selectOptionPC._inlinecount node, TContext context);
            protected internal abstract TResult Accept(_selectOptionPC._orderby node, TContext context);
            protected internal abstract TResult Accept(_selectOptionPC._skip node, TContext context);
            protected internal abstract TResult Accept(_selectOptionPC._top node, TContext context);
        }
        
        public sealed class _filter : _selectOptionPC
        {
            public _filter(__GeneratedOdataV3.CstNodes.Rules._filter _filter_1)
            {
                this._filter_1 = _filter_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._filter _filter_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _search : _selectOptionPC
        {
            public _search(__GeneratedOdataV3.CstNodes.Rules._search _search_1)
            {
                this._search_1 = _search_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._search _search_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _inlinecount : _selectOptionPC
        {
            public _inlinecount(__GeneratedOdataV3.CstNodes.Rules._inlinecount _inlinecount_1)
            {
                this._inlinecount_1 = _inlinecount_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._inlinecount _inlinecount_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _orderby : _selectOptionPC
        {
            public _orderby(__GeneratedOdataV3.CstNodes.Rules._orderby _orderby_1)
            {
                this._orderby_1 = _orderby_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._orderby _orderby_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _skip : _selectOptionPC
        {
            public _skip(__GeneratedOdataV3.CstNodes.Rules._skip _skip_1)
            {
                this._skip_1 = _skip_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._skip _skip_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _top : _selectOptionPC
        {
            public _top(__GeneratedOdataV3.CstNodes.Rules._top _top_1)
            {
                this._top_1 = _top_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._top _top_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

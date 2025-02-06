namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _expandRefOption
    {
        private _expandRefOption()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_expandRefOption node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_expandRefOption._expandCountOption node, TContext context);
            protected internal abstract TResult Accept(_expandRefOption._orderby node, TContext context);
            protected internal abstract TResult Accept(_expandRefOption._skip node, TContext context);
            protected internal abstract TResult Accept(_expandRefOption._top node, TContext context);
            protected internal abstract TResult Accept(_expandRefOption._inlinecount node, TContext context);
        }
        
        public sealed class _expandCountOption : _expandRefOption
        {
            public _expandCountOption(__GeneratedOdataV4.CstNodes.Rules._expandCountOption _expandCountOption_1)
            {
                this._expandCountOption_1 = _expandCountOption_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._expandCountOption _expandCountOption_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _orderby : _expandRefOption
        {
            public _orderby(__GeneratedOdataV4.CstNodes.Rules._orderby _orderby_1)
            {
                this._orderby_1 = _orderby_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._orderby _orderby_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _skip : _expandRefOption
        {
            public _skip(__GeneratedOdataV4.CstNodes.Rules._skip _skip_1)
            {
                this._skip_1 = _skip_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._skip _skip_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _top : _expandRefOption
        {
            public _top(__GeneratedOdataV4.CstNodes.Rules._top _top_1)
            {
                this._top_1 = _top_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._top _top_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _inlinecount : _expandRefOption
        {
            public _inlinecount(__GeneratedOdataV4.CstNodes.Rules._inlinecount _inlinecount_1)
            {
                this._inlinecount_1 = _inlinecount_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._inlinecount _inlinecount_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

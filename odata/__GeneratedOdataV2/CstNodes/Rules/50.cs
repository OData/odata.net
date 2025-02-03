namespace __GeneratedOdataV2.CstNodes.Rules
{
    public abstract class _queryOption
    {
        private _queryOption()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_queryOption node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_queryOption._systemQueryOption node, TContext context);
            protected internal abstract TResult Accept(_queryOption._aliasAndValue node, TContext context);
            protected internal abstract TResult Accept(_queryOption._nameAndValue node, TContext context);
            protected internal abstract TResult Accept(_queryOption._customQueryOption node, TContext context);
        }
        
        public sealed class _systemQueryOption : _queryOption
        {
            public _systemQueryOption(__GeneratedOdataV2.CstNodes.Rules._systemQueryOption _systemQueryOption_1)
            {
                this._systemQueryOption_1 = _systemQueryOption_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._systemQueryOption _systemQueryOption_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _aliasAndValue : _queryOption
        {
            public _aliasAndValue(__GeneratedOdataV2.CstNodes.Rules._aliasAndValue _aliasAndValue_1)
            {
                this._aliasAndValue_1 = _aliasAndValue_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._aliasAndValue _aliasAndValue_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _nameAndValue : _queryOption
        {
            public _nameAndValue(__GeneratedOdataV2.CstNodes.Rules._nameAndValue _nameAndValue_1)
            {
                this._nameAndValue_1 = _nameAndValue_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._nameAndValue _nameAndValue_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _customQueryOption : _queryOption
        {
            public _customQueryOption(__GeneratedOdataV2.CstNodes.Rules._customQueryOption _customQueryOption_1)
            {
                this._customQueryOption_1 = _customQueryOption_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._customQueryOption _customQueryOption_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

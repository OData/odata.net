namespace __GeneratedOdataV2.CstNodes.Rules
{
    public abstract class _expandOption
    {
        private _expandOption()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_expandOption node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_expandOption._expandRefOption node, TContext context);
            protected internal abstract TResult Accept(_expandOption._select node, TContext context);
            protected internal abstract TResult Accept(_expandOption._expand node, TContext context);
            protected internal abstract TResult Accept(_expandOption._compute node, TContext context);
            protected internal abstract TResult Accept(_expandOption._levels node, TContext context);
            protected internal abstract TResult Accept(_expandOption._aliasAndValue node, TContext context);
        }
        
        public sealed class _expandRefOption : _expandOption
        {
            public _expandRefOption(__GeneratedOdataV2.CstNodes.Rules._expandRefOption _expandRefOption_1)
            {
                this._expandRefOption_1 = _expandRefOption_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._expandRefOption _expandRefOption_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _select : _expandOption
        {
            public _select(__GeneratedOdataV2.CstNodes.Rules._select _select_1)
            {
                this._select_1 = _select_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._select _select_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _expand : _expandOption
        {
            public _expand(__GeneratedOdataV2.CstNodes.Rules._expand _expand_1)
            {
                this._expand_1 = _expand_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._expand _expand_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _compute : _expandOption
        {
            public _compute(__GeneratedOdataV2.CstNodes.Rules._compute _compute_1)
            {
                this._compute_1 = _compute_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._compute _compute_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _levels : _expandOption
        {
            public _levels(__GeneratedOdataV2.CstNodes.Rules._levels _levels_1)
            {
                this._levels_1 = _levels_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._levels _levels_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _aliasAndValue : _expandOption
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
    }
    
}

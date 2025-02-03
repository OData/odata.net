namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _parameterAliasⳆkeyPropertyValue
    {
        private _parameterAliasⳆkeyPropertyValue()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_parameterAliasⳆkeyPropertyValue node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_parameterAliasⳆkeyPropertyValue._parameterAlias node, TContext context);
            protected internal abstract TResult Accept(_parameterAliasⳆkeyPropertyValue._keyPropertyValue node, TContext context);
        }
        
        public sealed class _parameterAlias : _parameterAliasⳆkeyPropertyValue
        {
            public _parameterAlias(__GeneratedOdataV3.CstNodes.Rules._parameterAlias _parameterAlias_1)
            {
                this._parameterAlias_1 = _parameterAlias_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._parameterAlias _parameterAlias_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _keyPropertyValue : _parameterAliasⳆkeyPropertyValue
        {
            public _keyPropertyValue(__GeneratedOdataV3.CstNodes.Rules._keyPropertyValue _keyPropertyValue_1)
            {
                this._keyPropertyValue_1 = _keyPropertyValue_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._keyPropertyValue _keyPropertyValue_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

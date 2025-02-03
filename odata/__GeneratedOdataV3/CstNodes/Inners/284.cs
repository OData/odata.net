namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _parameterAliasⳆparameterValue
    {
        private _parameterAliasⳆparameterValue()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_parameterAliasⳆparameterValue node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_parameterAliasⳆparameterValue._parameterAlias node, TContext context);
            protected internal abstract TResult Accept(_parameterAliasⳆparameterValue._parameterValue node, TContext context);
        }
        
        public sealed class _parameterAlias : _parameterAliasⳆparameterValue
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
        
        public sealed class _parameterValue : _parameterAliasⳆparameterValue
        {
            public _parameterValue(__GeneratedOdataV3.CstNodes.Rules._parameterValue _parameterValue_1)
            {
                this._parameterValue_1 = _parameterValue_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._parameterValue _parameterValue_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _parameterAliasⳆprimitiveLiteral
    {
        private _parameterAliasⳆprimitiveLiteral()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_parameterAliasⳆprimitiveLiteral node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_parameterAliasⳆprimitiveLiteral._parameterAlias node, TContext context);
            protected internal abstract TResult Accept(_parameterAliasⳆprimitiveLiteral._primitiveLiteral node, TContext context);
        }
        
        public sealed class _parameterAlias : _parameterAliasⳆprimitiveLiteral
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
        
        public sealed class _primitiveLiteral : _parameterAliasⳆprimitiveLiteral
        {
            public _primitiveLiteral(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral _primitiveLiteral_1)
            {
                this._primitiveLiteral_1 = _primitiveLiteral_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral _primitiveLiteral_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

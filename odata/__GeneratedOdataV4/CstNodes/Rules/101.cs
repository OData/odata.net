namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _parameterValue
    {
        private _parameterValue()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_parameterValue node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_parameterValue._arrayOrObject node, TContext context);
            protected internal abstract TResult Accept(_parameterValue._commonExpr node, TContext context);
        }
        
        public sealed class _arrayOrObject : _parameterValue
        {
            public _arrayOrObject(__GeneratedOdataV4.CstNodes.Rules._arrayOrObject _arrayOrObject_1)
            {
                this._arrayOrObject_1 = _arrayOrObject_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._arrayOrObject _arrayOrObject_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _commonExpr : _parameterValue
        {
            public _commonExpr(__GeneratedOdataV4.CstNodes.Rules._commonExpr _commonExpr_1)
            {
                this._commonExpr_1 = _commonExpr_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._commonExpr _commonExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _primitivePath
    {
        private _primitivePath()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_primitivePath node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_primitivePath._value node, TContext context);
            protected internal abstract TResult Accept(_primitivePath._boundOperation node, TContext context);
        }
        
        public sealed class _value : _primitivePath
        {
            public _value(__GeneratedOdata.CstNodes.Rules._value _value_1)
            {
                this._value_1 = _value_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._value _value_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _boundOperation : _primitivePath
        {
            public _boundOperation(__GeneratedOdata.CstNodes.Rules._boundOperation _boundOperation_1)
            {
                this._boundOperation_1 = _boundOperation_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._boundOperation _boundOperation_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
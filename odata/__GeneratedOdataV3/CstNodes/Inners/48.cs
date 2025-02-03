namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _countⳆboundOperation
    {
        private _countⳆboundOperation()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_countⳆboundOperation node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_countⳆboundOperation._count node, TContext context);
            protected internal abstract TResult Accept(_countⳆboundOperation._boundOperation node, TContext context);
        }
        
        public sealed class _count : _countⳆboundOperation
        {
            public _count(__GeneratedOdataV3.CstNodes.Rules._count _count_1)
            {
                this._count_1 = _count_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._count _count_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _boundOperation : _countⳆboundOperation
        {
            public _boundOperation(__GeneratedOdataV3.CstNodes.Rules._boundOperation _boundOperation_1)
            {
                this._boundOperation_1 = _boundOperation_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._boundOperation _boundOperation_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

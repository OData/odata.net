namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _primitiveColPath
    {
        private _primitiveColPath()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_primitiveColPath node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_primitiveColPath._count node, TContext context);
            protected internal abstract TResult Accept(_primitiveColPath._boundOperation node, TContext context);
            protected internal abstract TResult Accept(_primitiveColPath._ordinalIndex node, TContext context);
        }
        
        public sealed class _count : _primitiveColPath
        {
            public _count(__GeneratedOdata.CstNodes.Rules._count _count_1)
            {
                this._count_1 = _count_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._count _count_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _boundOperation : _primitiveColPath
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
        
        public sealed class _ordinalIndex : _primitiveColPath
        {
            public _ordinalIndex(__GeneratedOdata.CstNodes.Rules._ordinalIndex _ordinalIndex_1)
            {
                this._ordinalIndex_1 = _ordinalIndex_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._ordinalIndex _ordinalIndex_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

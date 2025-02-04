namespace __GeneratedOdataV3.CstNodes.Rules
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
            private _count()
            {
                this._count_1 = __GeneratedOdataV3.CstNodes.Rules._count.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._count _count_1 { get; }
            public static _count Instance { get; } = new _count();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _boundOperation : _primitiveColPath
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
        
        public sealed class _ordinalIndex : _primitiveColPath
        {
            public _ordinalIndex(__GeneratedOdataV3.CstNodes.Rules._ordinalIndex _ordinalIndex_1)
            {
                this._ordinalIndex_1 = _ordinalIndex_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._ordinalIndex _ordinalIndex_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

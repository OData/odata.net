namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _complexColPath
    {
        private _complexColPath()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_complexColPath node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_complexColPath._ordinalIndex node, TContext context);
            protected internal abstract TResult Accept(_complexColPath._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_꘡countⳆboundOperation꘡ node, TContext context);
        }
        
        public sealed class _ordinalIndex : _complexColPath
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
        
        public sealed class _꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_꘡countⳆboundOperation꘡ : _complexColPath
        {
            public _꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_꘡countⳆboundOperation꘡(__GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_qualifiedComplexTypeName? _ʺx2Fʺ_qualifiedComplexTypeName_1, __GeneratedOdata.CstNodes.Inners._countⳆboundOperation? _countⳆboundOperation_1)
            {
                this._ʺx2Fʺ_qualifiedComplexTypeName_1 = _ʺx2Fʺ_qualifiedComplexTypeName_1;
                this._countⳆboundOperation_1 = _countⳆboundOperation_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_qualifiedComplexTypeName? _ʺx2Fʺ_qualifiedComplexTypeName_1 { get; }
            public __GeneratedOdata.CstNodes.Inners._countⳆboundOperation? _countⳆboundOperation_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

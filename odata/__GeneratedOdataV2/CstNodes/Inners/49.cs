namespace __GeneratedOdataV2.CstNodes.Inners
{
    public abstract class _ʺx2Fʺ_propertyPathⳆboundOperation
    {
        private _ʺx2Fʺ_propertyPathⳆboundOperation()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx2Fʺ_propertyPathⳆboundOperation node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx2Fʺ_propertyPathⳆboundOperation._ʺx2Fʺ_propertyPath node, TContext context);
            protected internal abstract TResult Accept(_ʺx2Fʺ_propertyPathⳆboundOperation._boundOperation node, TContext context);
        }
        
        public sealed class _ʺx2Fʺ_propertyPath : _ʺx2Fʺ_propertyPathⳆboundOperation
        {
            public _ʺx2Fʺ_propertyPath(__GeneratedOdataV2.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1, __GeneratedOdataV2.CstNodes.Rules._propertyPath _propertyPath_1)
            {
                this._ʺx2Fʺ_1 = _ʺx2Fʺ_1;
                this._propertyPath_1 = _propertyPath_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1 { get; }
            public __GeneratedOdataV2.CstNodes.Rules._propertyPath _propertyPath_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _boundOperation : _ʺx2Fʺ_propertyPathⳆboundOperation
        {
            public _boundOperation(__GeneratedOdataV2.CstNodes.Rules._boundOperation _boundOperation_1)
            {
                this._boundOperation_1 = _boundOperation_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._boundOperation _boundOperation_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue
    {
        private _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._ʺx2Fʺ_propertyPath node, TContext context);
            protected internal abstract TResult Accept(_ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._boundOperation node, TContext context);
            protected internal abstract TResult Accept(_ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._ref node, TContext context);
            protected internal abstract TResult Accept(_ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._value node, TContext context);
        }
        
        public sealed class _ʺx2Fʺ_propertyPath : _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue
        {
            public _ʺx2Fʺ_propertyPath(__GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1, __GeneratedOdataV3.CstNodes.Rules._propertyPath _propertyPath_1)
            {
                this._ʺx2Fʺ_1 = _ʺx2Fʺ_1;
                this._propertyPath_1 = _propertyPath_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._propertyPath _propertyPath_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _boundOperation : _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue
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
        
        public sealed class _ref : _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue
        {
            private _ref()
            {
                this._ref_1 = __GeneratedOdataV3.CstNodes.Rules._ref.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._ref _ref_1 { get; }
            public static _ref Instance { get; } = new _ref();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _value : _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue
        {
            private _value()
            {
                this._value_1 = __GeneratedOdataV3.CstNodes.Rules._value.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._value _value_1 { get; }
            public static _value Instance { get; } = new _value();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

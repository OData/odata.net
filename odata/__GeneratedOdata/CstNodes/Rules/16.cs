namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _propertyPath
    {
        private _propertyPath()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_propertyPath node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_propertyPath._entityColNavigationProperty_꘡collectionNavigation꘡ node, TContext context);
            protected internal abstract TResult Accept(_propertyPath._entityNavigationProperty_꘡singleNavigation꘡ node, TContext context);
            protected internal abstract TResult Accept(_propertyPath._complexColProperty_꘡complexColPath꘡ node, TContext context);
            protected internal abstract TResult Accept(_propertyPath._complexProperty_꘡complexPath꘡ node, TContext context);
            protected internal abstract TResult Accept(_propertyPath._primitiveColProperty_꘡primitiveColPath꘡ node, TContext context);
            protected internal abstract TResult Accept(_propertyPath._primitiveProperty_꘡primitivePath꘡ node, TContext context);
            protected internal abstract TResult Accept(_propertyPath._streamProperty_꘡boundOperation꘡ node, TContext context);
        }
        
        public sealed class _entityColNavigationProperty_꘡collectionNavigation꘡ : _propertyPath
        {
            public _entityColNavigationProperty_꘡collectionNavigation꘡(__GeneratedOdata.CstNodes.Rules._entityColNavigationProperty _entityColNavigationProperty_1, __GeneratedOdata.CstNodes.Rules._collectionNavigation? _collectionNavigation_1)
            {
                this._entityColNavigationProperty_1 = _entityColNavigationProperty_1;
                this._collectionNavigation_1 = _collectionNavigation_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._entityColNavigationProperty _entityColNavigationProperty_1 { get; }
            public __GeneratedOdata.CstNodes.Rules._collectionNavigation? _collectionNavigation_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _entityNavigationProperty_꘡singleNavigation꘡ : _propertyPath
        {
            public _entityNavigationProperty_꘡singleNavigation꘡(__GeneratedOdata.CstNodes.Rules._entityNavigationProperty _entityNavigationProperty_1, __GeneratedOdata.CstNodes.Rules._singleNavigation? _singleNavigation_1)
            {
                this._entityNavigationProperty_1 = _entityNavigationProperty_1;
                this._singleNavigation_1 = _singleNavigation_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._entityNavigationProperty _entityNavigationProperty_1 { get; }
            public __GeneratedOdata.CstNodes.Rules._singleNavigation? _singleNavigation_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _complexColProperty_꘡complexColPath꘡ : _propertyPath
        {
            public _complexColProperty_꘡complexColPath꘡(__GeneratedOdata.CstNodes.Rules._complexColProperty _complexColProperty_1, __GeneratedOdata.CstNodes.Rules._complexColPath? _complexColPath_1)
            {
                this._complexColProperty_1 = _complexColProperty_1;
                this._complexColPath_1 = _complexColPath_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._complexColProperty _complexColProperty_1 { get; }
            public __GeneratedOdata.CstNodes.Rules._complexColPath? _complexColPath_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _complexProperty_꘡complexPath꘡ : _propertyPath
        {
            public _complexProperty_꘡complexPath꘡(__GeneratedOdata.CstNodes.Rules._complexProperty _complexProperty_1, __GeneratedOdata.CstNodes.Rules._complexPath? _complexPath_1)
            {
                this._complexProperty_1 = _complexProperty_1;
                this._complexPath_1 = _complexPath_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._complexProperty _complexProperty_1 { get; }
            public __GeneratedOdata.CstNodes.Rules._complexPath? _complexPath_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _primitiveColProperty_꘡primitiveColPath꘡ : _propertyPath
        {
            public _primitiveColProperty_꘡primitiveColPath꘡(__GeneratedOdata.CstNodes.Rules._primitiveColProperty _primitiveColProperty_1, __GeneratedOdata.CstNodes.Rules._primitiveColPath? _primitiveColPath_1)
            {
                this._primitiveColProperty_1 = _primitiveColProperty_1;
                this._primitiveColPath_1 = _primitiveColPath_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._primitiveColProperty _primitiveColProperty_1 { get; }
            public __GeneratedOdata.CstNodes.Rules._primitiveColPath? _primitiveColPath_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _primitiveProperty_꘡primitivePath꘡ : _propertyPath
        {
            public _primitiveProperty_꘡primitivePath꘡(__GeneratedOdata.CstNodes.Rules._primitiveProperty _primitiveProperty_1, __GeneratedOdata.CstNodes.Rules._primitivePath? _primitivePath_1)
            {
                this._primitiveProperty_1 = _primitiveProperty_1;
                this._primitivePath_1 = _primitivePath_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._primitiveProperty _primitiveProperty_1 { get; }
            public __GeneratedOdata.CstNodes.Rules._primitivePath? _primitivePath_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _streamProperty_꘡boundOperation꘡ : _propertyPath
        {
            public _streamProperty_꘡boundOperation꘡(__GeneratedOdata.CstNodes.Rules._streamProperty _streamProperty_1, __GeneratedOdata.CstNodes.Rules._boundOperation? _boundOperation_1)
            {
                this._streamProperty_1 = _streamProperty_1;
                this._boundOperation_1 = _boundOperation_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._streamProperty _streamProperty_1 { get; }
            public __GeneratedOdata.CstNodes.Rules._boundOperation? _boundOperation_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

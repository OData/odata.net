namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _contextPropertyPath
    {
        private _contextPropertyPath()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_contextPropertyPath node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_contextPropertyPath._primitiveProperty node, TContext context);
            protected internal abstract TResult Accept(_contextPropertyPath._primitiveColProperty node, TContext context);
            protected internal abstract TResult Accept(_contextPropertyPath._complexColProperty node, TContext context);
            protected internal abstract TResult Accept(_contextPropertyPath._complexProperty_꘡꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath꘡ node, TContext context);
        }
        
        public sealed class _primitiveProperty : _contextPropertyPath
        {
            public _primitiveProperty(__GeneratedOdataV4.CstNodes.Rules._primitiveProperty _primitiveProperty_1)
            {
                this._primitiveProperty_1 = _primitiveProperty_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._primitiveProperty _primitiveProperty_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _primitiveColProperty : _contextPropertyPath
        {
            public _primitiveColProperty(__GeneratedOdataV4.CstNodes.Rules._primitiveColProperty _primitiveColProperty_1)
            {
                this._primitiveColProperty_1 = _primitiveColProperty_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._primitiveColProperty _primitiveColProperty_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _complexColProperty : _contextPropertyPath
        {
            public _complexColProperty(__GeneratedOdataV4.CstNodes.Rules._complexColProperty _complexColProperty_1)
            {
                this._complexColProperty_1 = _complexColProperty_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._complexColProperty _complexColProperty_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _complexProperty_꘡꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath꘡ : _contextPropertyPath
        {
            public _complexProperty_꘡꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath꘡(__GeneratedOdataV4.CstNodes.Rules._complexProperty _complexProperty_1, __GeneratedOdataV4.CstNodes.Inners._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath? _꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath_1)
            {
                this._complexProperty_1 = _complexProperty_1;
                this._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath_1 = _꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._complexProperty _complexProperty_1 { get; }
            public __GeneratedOdataV4.CstNodes.Inners._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath? _꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

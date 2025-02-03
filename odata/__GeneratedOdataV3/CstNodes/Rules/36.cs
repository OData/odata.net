namespace __GeneratedOdataV3.CstNodes.Rules
{
    public abstract class _boundFunctionCallNoParens
    {
        private _boundFunctionCallNoParens()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_boundFunctionCallNoParens node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_boundFunctionCallNoParens._namespace_ʺx2Eʺ_entityFunction node, TContext context);
            protected internal abstract TResult Accept(_boundFunctionCallNoParens._namespace_ʺx2Eʺ_entityColFunction node, TContext context);
            protected internal abstract TResult Accept(_boundFunctionCallNoParens._namespace_ʺx2Eʺ_complexFunction node, TContext context);
            protected internal abstract TResult Accept(_boundFunctionCallNoParens._namespace_ʺx2Eʺ_complexColFunction node, TContext context);
            protected internal abstract TResult Accept(_boundFunctionCallNoParens._namespace_ʺx2Eʺ_primitiveFunction node, TContext context);
            protected internal abstract TResult Accept(_boundFunctionCallNoParens._namespace_ʺx2Eʺ_primitiveColFunction node, TContext context);
        }
        
        public sealed class _namespace_ʺx2Eʺ_entityFunction : _boundFunctionCallNoParens
        {
            public _namespace_ʺx2Eʺ_entityFunction(__GeneratedOdataV3.CstNodes.Rules._namespace _namespace_1, __GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ _ʺx2Eʺ_1, __GeneratedOdataV3.CstNodes.Rules._entityFunction _entityFunction_1)
            {
                this._namespace_1 = _namespace_1;
                this._ʺx2Eʺ_1 = _ʺx2Eʺ_1;
                this._entityFunction_1 = _entityFunction_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._namespace _namespace_1 { get; }
            public __GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ _ʺx2Eʺ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._entityFunction _entityFunction_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _namespace_ʺx2Eʺ_entityColFunction : _boundFunctionCallNoParens
        {
            public _namespace_ʺx2Eʺ_entityColFunction(__GeneratedOdataV3.CstNodes.Rules._namespace _namespace_1, __GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ _ʺx2Eʺ_1, __GeneratedOdataV3.CstNodes.Rules._entityColFunction _entityColFunction_1)
            {
                this._namespace_1 = _namespace_1;
                this._ʺx2Eʺ_1 = _ʺx2Eʺ_1;
                this._entityColFunction_1 = _entityColFunction_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._namespace _namespace_1 { get; }
            public __GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ _ʺx2Eʺ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._entityColFunction _entityColFunction_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _namespace_ʺx2Eʺ_complexFunction : _boundFunctionCallNoParens
        {
            public _namespace_ʺx2Eʺ_complexFunction(__GeneratedOdataV3.CstNodes.Rules._namespace _namespace_1, __GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ _ʺx2Eʺ_1, __GeneratedOdataV3.CstNodes.Rules._complexFunction _complexFunction_1)
            {
                this._namespace_1 = _namespace_1;
                this._ʺx2Eʺ_1 = _ʺx2Eʺ_1;
                this._complexFunction_1 = _complexFunction_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._namespace _namespace_1 { get; }
            public __GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ _ʺx2Eʺ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._complexFunction _complexFunction_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _namespace_ʺx2Eʺ_complexColFunction : _boundFunctionCallNoParens
        {
            public _namespace_ʺx2Eʺ_complexColFunction(__GeneratedOdataV3.CstNodes.Rules._namespace _namespace_1, __GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ _ʺx2Eʺ_1, __GeneratedOdataV3.CstNodes.Rules._complexColFunction _complexColFunction_1)
            {
                this._namespace_1 = _namespace_1;
                this._ʺx2Eʺ_1 = _ʺx2Eʺ_1;
                this._complexColFunction_1 = _complexColFunction_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._namespace _namespace_1 { get; }
            public __GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ _ʺx2Eʺ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._complexColFunction _complexColFunction_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _namespace_ʺx2Eʺ_primitiveFunction : _boundFunctionCallNoParens
        {
            public _namespace_ʺx2Eʺ_primitiveFunction(__GeneratedOdataV3.CstNodes.Rules._namespace _namespace_1, __GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ _ʺx2Eʺ_1, __GeneratedOdataV3.CstNodes.Rules._primitiveFunction _primitiveFunction_1)
            {
                this._namespace_1 = _namespace_1;
                this._ʺx2Eʺ_1 = _ʺx2Eʺ_1;
                this._primitiveFunction_1 = _primitiveFunction_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._namespace _namespace_1 { get; }
            public __GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ _ʺx2Eʺ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._primitiveFunction _primitiveFunction_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _namespace_ʺx2Eʺ_primitiveColFunction : _boundFunctionCallNoParens
        {
            public _namespace_ʺx2Eʺ_primitiveColFunction(__GeneratedOdataV3.CstNodes.Rules._namespace _namespace_1, __GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ _ʺx2Eʺ_1, __GeneratedOdataV3.CstNodes.Rules._primitiveColFunction _primitiveColFunction_1)
            {
                this._namespace_1 = _namespace_1;
                this._ʺx2Eʺ_1 = _ʺx2Eʺ_1;
                this._primitiveColFunction_1 = _primitiveColFunction_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._namespace _namespace_1 { get; }
            public __GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ _ʺx2Eʺ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._primitiveColFunction _primitiveColFunction_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

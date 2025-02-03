namespace __GeneratedOdataV2.CstNodes.Rules
{
    public abstract class _functionImportCallNoParens
    {
        private _functionImportCallNoParens()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_functionImportCallNoParens node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_functionImportCallNoParens._entityFunctionImport node, TContext context);
            protected internal abstract TResult Accept(_functionImportCallNoParens._entityColFunctionImport node, TContext context);
            protected internal abstract TResult Accept(_functionImportCallNoParens._complexFunctionImport node, TContext context);
            protected internal abstract TResult Accept(_functionImportCallNoParens._complexColFunctionImport node, TContext context);
            protected internal abstract TResult Accept(_functionImportCallNoParens._primitiveFunctionImport node, TContext context);
            protected internal abstract TResult Accept(_functionImportCallNoParens._primitiveColFunctionImport node, TContext context);
        }
        
        public sealed class _entityFunctionImport : _functionImportCallNoParens
        {
            public _entityFunctionImport(__GeneratedOdataV2.CstNodes.Rules._entityFunctionImport _entityFunctionImport_1)
            {
                this._entityFunctionImport_1 = _entityFunctionImport_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._entityFunctionImport _entityFunctionImport_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _entityColFunctionImport : _functionImportCallNoParens
        {
            public _entityColFunctionImport(__GeneratedOdataV2.CstNodes.Rules._entityColFunctionImport _entityColFunctionImport_1)
            {
                this._entityColFunctionImport_1 = _entityColFunctionImport_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._entityColFunctionImport _entityColFunctionImport_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _complexFunctionImport : _functionImportCallNoParens
        {
            public _complexFunctionImport(__GeneratedOdataV2.CstNodes.Rules._complexFunctionImport _complexFunctionImport_1)
            {
                this._complexFunctionImport_1 = _complexFunctionImport_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._complexFunctionImport _complexFunctionImport_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _complexColFunctionImport : _functionImportCallNoParens
        {
            public _complexColFunctionImport(__GeneratedOdataV2.CstNodes.Rules._complexColFunctionImport _complexColFunctionImport_1)
            {
                this._complexColFunctionImport_1 = _complexColFunctionImport_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._complexColFunctionImport _complexColFunctionImport_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _primitiveFunctionImport : _functionImportCallNoParens
        {
            public _primitiveFunctionImport(__GeneratedOdataV2.CstNodes.Rules._primitiveFunctionImport _primitiveFunctionImport_1)
            {
                this._primitiveFunctionImport_1 = _primitiveFunctionImport_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._primitiveFunctionImport _primitiveFunctionImport_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _primitiveColFunctionImport : _functionImportCallNoParens
        {
            public _primitiveColFunctionImport(__GeneratedOdataV2.CstNodes.Rules._primitiveColFunctionImport _primitiveColFunctionImport_1)
            {
                this._primitiveColFunctionImport_1 = _primitiveColFunctionImport_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._primitiveColFunctionImport _primitiveColFunctionImport_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

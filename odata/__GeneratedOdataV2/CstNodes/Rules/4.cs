namespace __GeneratedOdataV2.CstNodes.Rules
{
    public abstract class _resourcePath
    {
        private _resourcePath()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_resourcePath node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_resourcePath._entitySetName_꘡collectionNavigation꘡ node, TContext context);
            protected internal abstract TResult Accept(_resourcePath._singletonEntity_꘡singleNavigation꘡ node, TContext context);
            protected internal abstract TResult Accept(_resourcePath._actionImportCall node, TContext context);
            protected internal abstract TResult Accept(_resourcePath._entityColFunctionImportCall_꘡collectionNavigation꘡ node, TContext context);
            protected internal abstract TResult Accept(_resourcePath._entityFunctionImportCall_꘡singleNavigation꘡ node, TContext context);
            protected internal abstract TResult Accept(_resourcePath._complexColFunctionImportCall_꘡complexColPath꘡ node, TContext context);
            protected internal abstract TResult Accept(_resourcePath._complexFunctionImportCall_꘡complexPath꘡ node, TContext context);
            protected internal abstract TResult Accept(_resourcePath._primitiveColFunctionImportCall_꘡primitiveColPath꘡ node, TContext context);
            protected internal abstract TResult Accept(_resourcePath._primitiveFunctionImportCall_꘡primitivePath꘡ node, TContext context);
            protected internal abstract TResult Accept(_resourcePath._functionImportCallNoParens node, TContext context);
            protected internal abstract TResult Accept(_resourcePath._crossjoin node, TContext context);
            protected internal abstract TResult Accept(_resourcePath._ʺx24x61x6Cx6Cʺ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡ node, TContext context);
        }
        
        public sealed class _entitySetName_꘡collectionNavigation꘡ : _resourcePath
        {
            public _entitySetName_꘡collectionNavigation꘡(__GeneratedOdataV2.CstNodes.Rules._entitySetName _entitySetName_1, __GeneratedOdataV2.CstNodes.Rules._collectionNavigation? _collectionNavigation_1)
            {
                this._entitySetName_1 = _entitySetName_1;
                this._collectionNavigation_1 = _collectionNavigation_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._entitySetName _entitySetName_1 { get; }
            public __GeneratedOdataV2.CstNodes.Rules._collectionNavigation? _collectionNavigation_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _singletonEntity_꘡singleNavigation꘡ : _resourcePath
        {
            public _singletonEntity_꘡singleNavigation꘡(__GeneratedOdataV2.CstNodes.Rules._singletonEntity _singletonEntity_1, __GeneratedOdataV2.CstNodes.Rules._singleNavigation? _singleNavigation_1)
            {
                this._singletonEntity_1 = _singletonEntity_1;
                this._singleNavigation_1 = _singleNavigation_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._singletonEntity _singletonEntity_1 { get; }
            public __GeneratedOdataV2.CstNodes.Rules._singleNavigation? _singleNavigation_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _actionImportCall : _resourcePath
        {
            public _actionImportCall(__GeneratedOdataV2.CstNodes.Rules._actionImportCall _actionImportCall_1)
            {
                this._actionImportCall_1 = _actionImportCall_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._actionImportCall _actionImportCall_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _entityColFunctionImportCall_꘡collectionNavigation꘡ : _resourcePath
        {
            public _entityColFunctionImportCall_꘡collectionNavigation꘡(__GeneratedOdataV2.CstNodes.Rules._entityColFunctionImportCall _entityColFunctionImportCall_1, __GeneratedOdataV2.CstNodes.Rules._collectionNavigation? _collectionNavigation_1)
            {
                this._entityColFunctionImportCall_1 = _entityColFunctionImportCall_1;
                this._collectionNavigation_1 = _collectionNavigation_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._entityColFunctionImportCall _entityColFunctionImportCall_1 { get; }
            public __GeneratedOdataV2.CstNodes.Rules._collectionNavigation? _collectionNavigation_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _entityFunctionImportCall_꘡singleNavigation꘡ : _resourcePath
        {
            public _entityFunctionImportCall_꘡singleNavigation꘡(__GeneratedOdataV2.CstNodes.Rules._entityFunctionImportCall _entityFunctionImportCall_1, __GeneratedOdataV2.CstNodes.Rules._singleNavigation? _singleNavigation_1)
            {
                this._entityFunctionImportCall_1 = _entityFunctionImportCall_1;
                this._singleNavigation_1 = _singleNavigation_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._entityFunctionImportCall _entityFunctionImportCall_1 { get; }
            public __GeneratedOdataV2.CstNodes.Rules._singleNavigation? _singleNavigation_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _complexColFunctionImportCall_꘡complexColPath꘡ : _resourcePath
        {
            public _complexColFunctionImportCall_꘡complexColPath꘡(__GeneratedOdataV2.CstNodes.Rules._complexColFunctionImportCall _complexColFunctionImportCall_1, __GeneratedOdataV2.CstNodes.Rules._complexColPath? _complexColPath_1)
            {
                this._complexColFunctionImportCall_1 = _complexColFunctionImportCall_1;
                this._complexColPath_1 = _complexColPath_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._complexColFunctionImportCall _complexColFunctionImportCall_1 { get; }
            public __GeneratedOdataV2.CstNodes.Rules._complexColPath? _complexColPath_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _complexFunctionImportCall_꘡complexPath꘡ : _resourcePath
        {
            public _complexFunctionImportCall_꘡complexPath꘡(__GeneratedOdataV2.CstNodes.Rules._complexFunctionImportCall _complexFunctionImportCall_1, __GeneratedOdataV2.CstNodes.Rules._complexPath? _complexPath_1)
            {
                this._complexFunctionImportCall_1 = _complexFunctionImportCall_1;
                this._complexPath_1 = _complexPath_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._complexFunctionImportCall _complexFunctionImportCall_1 { get; }
            public __GeneratedOdataV2.CstNodes.Rules._complexPath? _complexPath_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _primitiveColFunctionImportCall_꘡primitiveColPath꘡ : _resourcePath
        {
            public _primitiveColFunctionImportCall_꘡primitiveColPath꘡(__GeneratedOdataV2.CstNodes.Rules._primitiveColFunctionImportCall _primitiveColFunctionImportCall_1, __GeneratedOdataV2.CstNodes.Rules._primitiveColPath? _primitiveColPath_1)
            {
                this._primitiveColFunctionImportCall_1 = _primitiveColFunctionImportCall_1;
                this._primitiveColPath_1 = _primitiveColPath_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._primitiveColFunctionImportCall _primitiveColFunctionImportCall_1 { get; }
            public __GeneratedOdataV2.CstNodes.Rules._primitiveColPath? _primitiveColPath_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _primitiveFunctionImportCall_꘡primitivePath꘡ : _resourcePath
        {
            public _primitiveFunctionImportCall_꘡primitivePath꘡(__GeneratedOdataV2.CstNodes.Rules._primitiveFunctionImportCall _primitiveFunctionImportCall_1, __GeneratedOdataV2.CstNodes.Rules._primitivePath? _primitivePath_1)
            {
                this._primitiveFunctionImportCall_1 = _primitiveFunctionImportCall_1;
                this._primitivePath_1 = _primitivePath_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._primitiveFunctionImportCall _primitiveFunctionImportCall_1 { get; }
            public __GeneratedOdataV2.CstNodes.Rules._primitivePath? _primitivePath_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _functionImportCallNoParens : _resourcePath
        {
            public _functionImportCallNoParens(__GeneratedOdataV2.CstNodes.Rules._functionImportCallNoParens _functionImportCallNoParens_1)
            {
                this._functionImportCallNoParens_1 = _functionImportCallNoParens_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._functionImportCallNoParens _functionImportCallNoParens_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _crossjoin : _resourcePath
        {
            public _crossjoin(__GeneratedOdataV2.CstNodes.Rules._crossjoin _crossjoin_1)
            {
                this._crossjoin_1 = _crossjoin_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._crossjoin _crossjoin_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx24x61x6Cx6Cʺ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡ : _resourcePath
        {
            public _ʺx24x61x6Cx6Cʺ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡(__GeneratedOdataV2.CstNodes.Inners._ʺx24x61x6Cx6Cʺ _ʺx24x61x6Cx6Cʺ_1, __GeneratedOdataV2.CstNodes.Inners._ʺx2Fʺ_qualifiedEntityTypeName? _ʺx2Fʺ_qualifiedEntityTypeName_1)
            {
                this._ʺx24x61x6Cx6Cʺ_1 = _ʺx24x61x6Cx6Cʺ_1;
                this._ʺx2Fʺ_qualifiedEntityTypeName_1 = _ʺx2Fʺ_qualifiedEntityTypeName_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx24x61x6Cx6Cʺ _ʺx24x61x6Cx6Cʺ_1 { get; }
            public __GeneratedOdataV2.CstNodes.Inners._ʺx2Fʺ_qualifiedEntityTypeName? _ʺx2Fʺ_qualifiedEntityTypeName_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

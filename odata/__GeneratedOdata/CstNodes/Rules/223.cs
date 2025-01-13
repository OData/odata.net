namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _singleQualifiedTypeName
    {
        private _singleQualifiedTypeName()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_singleQualifiedTypeName node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_singleQualifiedTypeName._qualifiedEntityTypeName node, TContext context);
            protected internal abstract TResult Accept(_singleQualifiedTypeName._qualifiedComplexTypeName node, TContext context);
            protected internal abstract TResult Accept(_singleQualifiedTypeName._qualifiedTypeDefinitionName node, TContext context);
            protected internal abstract TResult Accept(_singleQualifiedTypeName._qualifiedEnumTypeName node, TContext context);
            protected internal abstract TResult Accept(_singleQualifiedTypeName._primitiveTypeName node, TContext context);
        }
        
        public sealed class _qualifiedEntityTypeName : _singleQualifiedTypeName
        {
            public _qualifiedEntityTypeName(__GeneratedOdata.CstNodes.Rules._qualifiedEntityTypeName _qualifiedEntityTypeName_1)
            {
                this._qualifiedEntityTypeName_1 = _qualifiedEntityTypeName_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._qualifiedEntityTypeName _qualifiedEntityTypeName_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _qualifiedComplexTypeName : _singleQualifiedTypeName
        {
            public _qualifiedComplexTypeName(__GeneratedOdata.CstNodes.Rules._qualifiedComplexTypeName _qualifiedComplexTypeName_1)
            {
                this._qualifiedComplexTypeName_1 = _qualifiedComplexTypeName_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._qualifiedComplexTypeName _qualifiedComplexTypeName_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _qualifiedTypeDefinitionName : _singleQualifiedTypeName
        {
            public _qualifiedTypeDefinitionName(__GeneratedOdata.CstNodes.Rules._qualifiedTypeDefinitionName _qualifiedTypeDefinitionName_1)
            {
                this._qualifiedTypeDefinitionName_1 = _qualifiedTypeDefinitionName_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._qualifiedTypeDefinitionName _qualifiedTypeDefinitionName_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _qualifiedEnumTypeName : _singleQualifiedTypeName
        {
            public _qualifiedEnumTypeName(__GeneratedOdata.CstNodes.Rules._qualifiedEnumTypeName _qualifiedEnumTypeName_1)
            {
                this._qualifiedEnumTypeName_1 = _qualifiedEnumTypeName_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._qualifiedEnumTypeName _qualifiedEnumTypeName_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _primitiveTypeName : _singleQualifiedTypeName
        {
            public _primitiveTypeName(__GeneratedOdata.CstNodes.Rules._primitiveTypeName _primitiveTypeName_1)
            {
                this._primitiveTypeName_1 = _primitiveTypeName_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._primitiveTypeName _primitiveTypeName_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

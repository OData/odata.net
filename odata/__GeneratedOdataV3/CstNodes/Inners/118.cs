namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _qualifiedEntityTypeNameⳆqualifiedComplexTypeName
    {
        private _qualifiedEntityTypeNameⳆqualifiedComplexTypeName()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_qualifiedEntityTypeNameⳆqualifiedComplexTypeName node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_qualifiedEntityTypeNameⳆqualifiedComplexTypeName._qualifiedEntityTypeName node, TContext context);
            protected internal abstract TResult Accept(_qualifiedEntityTypeNameⳆqualifiedComplexTypeName._qualifiedComplexTypeName node, TContext context);
        }
        
        public sealed class _qualifiedEntityTypeName : _qualifiedEntityTypeNameⳆqualifiedComplexTypeName
        {
            public _qualifiedEntityTypeName(__GeneratedOdataV3.CstNodes.Rules._qualifiedEntityTypeName _qualifiedEntityTypeName_1)
            {
                this._qualifiedEntityTypeName_1 = _qualifiedEntityTypeName_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._qualifiedEntityTypeName _qualifiedEntityTypeName_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _qualifiedComplexTypeName : _qualifiedEntityTypeNameⳆqualifiedComplexTypeName
        {
            public _qualifiedComplexTypeName(__GeneratedOdataV3.CstNodes.Rules._qualifiedComplexTypeName _qualifiedComplexTypeName_1)
            {
                this._qualifiedComplexTypeName_1 = _qualifiedComplexTypeName_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._qualifiedComplexTypeName _qualifiedComplexTypeName_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

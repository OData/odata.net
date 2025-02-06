namespace __GeneratedOdataV4.CstNodes.Inners
{
    public abstract class _qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty
    {
        private _qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._qualifiedActionName node, TContext context);
            protected internal abstract TResult Accept(_qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._qualifiedFunctionName node, TContext context);
            protected internal abstract TResult Accept(_qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._selectListProperty node, TContext context);
        }
        
        public sealed class _qualifiedActionName : _qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty
        {
            public _qualifiedActionName(__GeneratedOdataV4.CstNodes.Rules._qualifiedActionName _qualifiedActionName_1)
            {
                this._qualifiedActionName_1 = _qualifiedActionName_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._qualifiedActionName _qualifiedActionName_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _qualifiedFunctionName : _qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty
        {
            public _qualifiedFunctionName(__GeneratedOdataV4.CstNodes.Rules._qualifiedFunctionName _qualifiedFunctionName_1)
            {
                this._qualifiedFunctionName_1 = _qualifiedFunctionName_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._qualifiedFunctionName _qualifiedFunctionName_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _selectListProperty : _qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty
        {
            public _selectListProperty(__GeneratedOdataV4.CstNodes.Rules._selectListProperty _selectListProperty_1)
            {
                this._selectListProperty_1 = _selectListProperty_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._selectListProperty _selectListProperty_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

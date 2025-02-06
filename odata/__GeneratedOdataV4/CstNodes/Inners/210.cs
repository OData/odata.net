namespace __GeneratedOdataV4.CstNodes.Inners
{
    public abstract class _selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName
    {
        private _selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._selectProperty node, TContext context);
            protected internal abstract TResult Accept(_selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._qualifiedActionName node, TContext context);
            protected internal abstract TResult Accept(_selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._qualifiedFunctionName node, TContext context);
        }
        
        public sealed class _selectProperty : _selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName
        {
            public _selectProperty(__GeneratedOdataV4.CstNodes.Rules._selectProperty _selectProperty_1)
            {
                this._selectProperty_1 = _selectProperty_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._selectProperty _selectProperty_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _qualifiedActionName : _selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName
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
        
        public sealed class _qualifiedFunctionName : _selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName
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
    }
    
}

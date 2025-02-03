namespace __GeneratedOdataV3.CstNodes.Rules
{
    public abstract class _metadataOption
    {
        private _metadataOption()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_metadataOption node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_metadataOption._format node, TContext context);
            protected internal abstract TResult Accept(_metadataOption._customQueryOption node, TContext context);
        }
        
        public sealed class _format : _metadataOption
        {
            public _format(__GeneratedOdataV3.CstNodes.Rules._format _format_1)
            {
                this._format_1 = _format_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._format _format_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _customQueryOption : _metadataOption
        {
            public _customQueryOption(__GeneratedOdataV3.CstNodes.Rules._customQueryOption _customQueryOption_1)
            {
                this._customQueryOption_1 = _customQueryOption_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._customQueryOption _customQueryOption_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

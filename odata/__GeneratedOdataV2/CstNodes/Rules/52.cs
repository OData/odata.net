namespace __GeneratedOdataV2.CstNodes.Rules
{
    public abstract class _batchOption
    {
        private _batchOption()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_batchOption node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_batchOption._format node, TContext context);
            protected internal abstract TResult Accept(_batchOption._customQueryOption node, TContext context);
        }
        
        public sealed class _format : _batchOption
        {
            public _format(__GeneratedOdataV2.CstNodes.Rules._format _format_1)
            {
                this._format_1 = _format_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._format _format_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _customQueryOption : _batchOption
        {
            public _customQueryOption(__GeneratedOdataV2.CstNodes.Rules._customQueryOption _customQueryOption_1)
            {
                this._customQueryOption_1 = _customQueryOption_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._customQueryOption _customQueryOption_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

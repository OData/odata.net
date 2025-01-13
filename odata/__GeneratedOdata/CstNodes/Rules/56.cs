namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _entityIdOption
    {
        private _entityIdOption()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_entityIdOption node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_entityIdOption._format node, TContext context);
            protected internal abstract TResult Accept(_entityIdOption._customQueryOption node, TContext context);
        }
        
        public sealed class _format : _entityIdOption
        {
            public _format(__GeneratedOdata.CstNodes.Rules._format _format_1)
            {
                this._format_1 = _format_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._format _format_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _customQueryOption : _entityIdOption
        {
            public _customQueryOption(__GeneratedOdata.CstNodes.Rules._customQueryOption _customQueryOption_1)
            {
                this._customQueryOption_1 = _customQueryOption_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._customQueryOption _customQueryOption_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

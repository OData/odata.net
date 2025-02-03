namespace __GeneratedOdataV2.CstNodes.Inners
{
    public abstract class _SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE
    {
        private _SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._SQUOTEⲻinⲻstring node, TContext context);
            protected internal abstract TResult Accept(_SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE node, TContext context);
        }
        
        public sealed class _SQUOTEⲻinⲻstring : _SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE
        {
            public _SQUOTEⲻinⲻstring(__GeneratedOdataV2.CstNodes.Rules._SQUOTEⲻinⲻstring _SQUOTEⲻinⲻstring_1)
            {
                this._SQUOTEⲻinⲻstring_1 = _SQUOTEⲻinⲻstring_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._SQUOTEⲻinⲻstring _SQUOTEⲻinⲻstring_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _pcharⲻnoⲻSQUOTE : _SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE
        {
            public _pcharⲻnoⲻSQUOTE(__GeneratedOdataV2.CstNodes.Rules._pcharⲻnoⲻSQUOTE _pcharⲻnoⲻSQUOTE_1)
            {
                this._pcharⲻnoⲻSQUOTE_1 = _pcharⲻnoⲻSQUOTE_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._pcharⲻnoⲻSQUOTE _pcharⲻnoⲻSQUOTE_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

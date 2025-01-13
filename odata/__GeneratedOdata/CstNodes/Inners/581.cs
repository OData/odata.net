namespace __GeneratedOdata.CstNodes.Inners
{
    public abstract class _termNameⳆSTAR
    {
        private _termNameⳆSTAR()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_termNameⳆSTAR node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_termNameⳆSTAR._termName node, TContext context);
            protected internal abstract TResult Accept(_termNameⳆSTAR._STAR node, TContext context);
        }
        
        public sealed class _termName : _termNameⳆSTAR
        {
            public _termName(__GeneratedOdata.CstNodes.Rules._termName _termName_1)
            {
                this._termName_1 = _termName_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._termName _termName_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _STAR : _termNameⳆSTAR
        {
            public _STAR(__GeneratedOdata.CstNodes.Rules._STAR _STAR_1)
            {
                this._STAR_1 = _STAR_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._STAR _STAR_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

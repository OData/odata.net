namespace __GeneratedOdataV2.CstNodes.Inners
{
    public abstract class _STARⳆ1Жunreserved
    {
        private _STARⳆ1Жunreserved()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_STARⳆ1Жunreserved node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_STARⳆ1Жunreserved._STAR node, TContext context);
            protected internal abstract TResult Accept(_STARⳆ1Жunreserved._1Жunreserved node, TContext context);
        }
        
        public sealed class _STAR : _STARⳆ1Жunreserved
        {
            public _STAR(__GeneratedOdataV2.CstNodes.Rules._STAR _STAR_1)
            {
                this._STAR_1 = _STAR_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._STAR _STAR_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _1Жunreserved : _STARⳆ1Жunreserved
        {
            public _1Жunreserved(__GeneratedOdataV2.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV2.CstNodes.Rules._unreserved> _unreserved_1)
            {
                this._unreserved_1 = _unreserved_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV2.CstNodes.Rules._unreserved> _unreserved_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

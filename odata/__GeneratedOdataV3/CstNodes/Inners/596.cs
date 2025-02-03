namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ
    {
        private _STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ._STAR node, TContext context);
            protected internal abstract TResult Accept(_STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ._namespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ node, TContext context);
        }
        
        public sealed class _STAR : _STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ
        {
            public _STAR(__GeneratedOdataV3.CstNodes.Rules._STAR _STAR_1)
            {
                this._STAR_1 = _STAR_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._STAR _STAR_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _namespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ : _STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ
        {
            public _namespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ(__GeneratedOdataV3.CstNodes.Rules._namespace _namespace_1, __GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ _ʺx2Eʺ_1, __GeneratedOdataV3.CstNodes.Inners._ⲤtermNameⳆSTARↃ _ⲤtermNameⳆSTARↃ_1)
            {
                this._namespace_1 = _namespace_1;
                this._ʺx2Eʺ_1 = _ʺx2Eʺ_1;
                this._ⲤtermNameⳆSTARↃ_1 = _ⲤtermNameⳆSTARↃ_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._namespace _namespace_1 { get; }
            public __GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ _ʺx2Eʺ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Inners._ⲤtermNameⳆSTARↃ _ⲤtermNameⳆSTARↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

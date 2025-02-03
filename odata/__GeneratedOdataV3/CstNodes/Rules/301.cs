namespace __GeneratedOdataV3.CstNodes.Rules
{
    public abstract class _hour
    {
        private _hour()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_hour node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_hour._Ⲥʺx30ʺⳆʺx31ʺↃ_DIGIT node, TContext context);
            protected internal abstract TResult Accept(_hour._ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ node, TContext context);
        }
        
        public sealed class _Ⲥʺx30ʺⳆʺx31ʺↃ_DIGIT : _hour
        {
            public _Ⲥʺx30ʺⳆʺx31ʺↃ_DIGIT(__GeneratedOdataV3.CstNodes.Inners._Ⲥʺx30ʺⳆʺx31ʺↃ _Ⲥʺx30ʺⳆʺx31ʺↃ_1, __GeneratedOdataV3.CstNodes.Rules._DIGIT _DIGIT_1)
            {
                this._Ⲥʺx30ʺⳆʺx31ʺↃ_1 = _Ⲥʺx30ʺⳆʺx31ʺↃ_1;
                this._DIGIT_1 = _DIGIT_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx30ʺⳆʺx31ʺↃ _Ⲥʺx30ʺⳆʺx31ʺↃ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._DIGIT _DIGIT_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ : _hour
        {
            public _ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ(__GeneratedOdataV3.CstNodes.Inners._ʺx32ʺ _ʺx32ʺ_1, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ _Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ_1)
            {
                this._ʺx32ʺ_1 = _ʺx32ʺ_1;
                this._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ_1 = _Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx32ʺ _ʺx32ʺ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ _Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

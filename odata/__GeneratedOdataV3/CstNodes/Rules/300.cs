namespace __GeneratedOdataV3.CstNodes.Rules
{
    public abstract class _day
    {
        private _day()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_day node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_day._ʺx30ʺ_oneToNine node, TContext context);
            protected internal abstract TResult Accept(_day._Ⲥʺx31ʺⳆʺx32ʺↃ_DIGIT node, TContext context);
            protected internal abstract TResult Accept(_day._ʺx33ʺ_Ⲥʺx30ʺⳆʺx31ʺↃ node, TContext context);
        }
        
        public sealed class _ʺx30ʺ_oneToNine : _day
        {
            public _ʺx30ʺ_oneToNine(__GeneratedOdataV3.CstNodes.Inners._ʺx30ʺ _ʺx30ʺ_1, __GeneratedOdataV3.CstNodes.Rules._oneToNine _oneToNine_1)
            {
                this._ʺx30ʺ_1 = _ʺx30ʺ_1;
                this._oneToNine_1 = _oneToNine_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺ _ʺx30ʺ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._oneToNine _oneToNine_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _Ⲥʺx31ʺⳆʺx32ʺↃ_DIGIT : _day
        {
            public _Ⲥʺx31ʺⳆʺx32ʺↃ_DIGIT(__GeneratedOdataV3.CstNodes.Inners._Ⲥʺx31ʺⳆʺx32ʺↃ _Ⲥʺx31ʺⳆʺx32ʺↃ_1, __GeneratedOdataV3.CstNodes.Rules._DIGIT _DIGIT_1)
            {
                this._Ⲥʺx31ʺⳆʺx32ʺↃ_1 = _Ⲥʺx31ʺⳆʺx32ʺↃ_1;
                this._DIGIT_1 = _DIGIT_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx31ʺⳆʺx32ʺↃ _Ⲥʺx31ʺⳆʺx32ʺↃ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._DIGIT _DIGIT_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx33ʺ_Ⲥʺx30ʺⳆʺx31ʺↃ : _day
        {
            public _ʺx33ʺ_Ⲥʺx30ʺⳆʺx31ʺↃ(__GeneratedOdataV3.CstNodes.Inners._ʺx33ʺ _ʺx33ʺ_1, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx30ʺⳆʺx31ʺↃ _Ⲥʺx30ʺⳆʺx31ʺↃ_1)
            {
                this._ʺx33ʺ_1 = _ʺx33ʺ_1;
                this._Ⲥʺx30ʺⳆʺx31ʺↃ_1 = _Ⲥʺx30ʺⳆʺx31ʺↃ_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx33ʺ _ʺx33ʺ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx30ʺⳆʺx31ʺↃ _Ⲥʺx30ʺⳆʺx31ʺↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

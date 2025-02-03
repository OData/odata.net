namespace __GeneratedOdataV2.CstNodes.Inners
{
    public abstract class _base64b16Ⳇbase64b8
    {
        private _base64b16Ⳇbase64b8()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_base64b16Ⳇbase64b8 node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_base64b16Ⳇbase64b8._base64b16 node, TContext context);
            protected internal abstract TResult Accept(_base64b16Ⳇbase64b8._base64b8 node, TContext context);
        }
        
        public sealed class _base64b16 : _base64b16Ⳇbase64b8
        {
            public _base64b16(__GeneratedOdataV2.CstNodes.Rules._base64b16 _base64b16_1)
            {
                this._base64b16_1 = _base64b16_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._base64b16 _base64b16_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _base64b8 : _base64b16Ⳇbase64b8
        {
            public _base64b8(__GeneratedOdataV2.CstNodes.Rules._base64b8 _base64b8_1)
            {
                this._base64b8_1 = _base64b8_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._base64b8 _base64b8_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

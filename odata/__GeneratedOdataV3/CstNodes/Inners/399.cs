namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _ʺx2FʺⳆʺx25x32x46ʺ
    {
        private _ʺx2FʺⳆʺx25x32x46ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx2FʺⳆʺx25x32x46ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx2FʺⳆʺx25x32x46ʺ._ʺx2Fʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx2FʺⳆʺx25x32x46ʺ._ʺx25x32x46ʺ node, TContext context);
        }
        
        public sealed class _ʺx2Fʺ : _ʺx2FʺⳆʺx25x32x46ʺ
        {
            public _ʺx2Fʺ(__GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1)
            {
                this._ʺx2Fʺ_1 = _ʺx2Fʺ_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x32x46ʺ : _ʺx2FʺⳆʺx25x32x46ʺ
        {
            public _ʺx25x32x46ʺ(__GeneratedOdataV3.CstNodes.Inners._ʺx25x32x46ʺ _ʺx25x32x46ʺ_1)
            {
                this._ʺx25x32x46ʺ_1 = _ʺx25x32x46ʺ_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx25x32x46ʺ _ʺx25x32x46ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

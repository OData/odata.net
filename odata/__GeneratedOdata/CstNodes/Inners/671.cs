namespace __GeneratedOdata.CstNodes.Inners
{
    public abstract class _pcharⳆʺx2FʺⳆʺx3Fʺ
    {
        private _pcharⳆʺx2FʺⳆʺx3Fʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_pcharⳆʺx2FʺⳆʺx3Fʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_pcharⳆʺx2FʺⳆʺx3Fʺ._pchar node, TContext context);
            protected internal abstract TResult Accept(_pcharⳆʺx2FʺⳆʺx3Fʺ._ʺx2Fʺ node, TContext context);
            protected internal abstract TResult Accept(_pcharⳆʺx2FʺⳆʺx3Fʺ._ʺx3Fʺ node, TContext context);
        }
        
        public sealed class _pchar : _pcharⳆʺx2FʺⳆʺx3Fʺ
        {
            public _pchar(__GeneratedOdata.CstNodes.Rules._pchar _pchar_1)
            {
                this._pchar_1 = _pchar_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._pchar _pchar_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Fʺ : _pcharⳆʺx2FʺⳆʺx3Fʺ
        {
            public _ʺx2Fʺ(__GeneratedOdata.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1)
            {
                this._ʺx2Fʺ_1 = _ʺx2Fʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx3Fʺ : _pcharⳆʺx2FʺⳆʺx3Fʺ
        {
            public _ʺx3Fʺ(__GeneratedOdata.CstNodes.Inners._ʺx3Fʺ _ʺx3Fʺ_1)
            {
                this._ʺx3Fʺ_1 = _ʺx3Fʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx3Fʺ _ʺx3Fʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
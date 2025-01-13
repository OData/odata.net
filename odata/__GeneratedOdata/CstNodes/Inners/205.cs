namespace __GeneratedOdata.CstNodes.Inners
{
    public abstract class _ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ
    {
        private _ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ._ʺx24x73x65x6Cx65x63x74ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ._ʺx73x65x6Cx65x63x74ʺ node, TContext context);
        }
        
        public sealed class _ʺx24x73x65x6Cx65x63x74ʺ : _ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ
        {
            public _ʺx24x73x65x6Cx65x63x74ʺ(__GeneratedOdata.CstNodes.Inners._ʺx24x73x65x6Cx65x63x74ʺ _ʺx24x73x65x6Cx65x63x74ʺ_1)
            {
                this._ʺx24x73x65x6Cx65x63x74ʺ_1 = _ʺx24x73x65x6Cx65x63x74ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx24x73x65x6Cx65x63x74ʺ _ʺx24x73x65x6Cx65x63x74ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx73x65x6Cx65x63x74ʺ : _ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ
        {
            public _ʺx73x65x6Cx65x63x74ʺ(__GeneratedOdata.CstNodes.Inners._ʺx73x65x6Cx65x63x74ʺ _ʺx73x65x6Cx65x63x74ʺ_1)
            {
                this._ʺx73x65x6Cx65x63x74ʺ_1 = _ʺx73x65x6Cx65x63x74ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx73x65x6Cx65x63x74ʺ _ʺx73x65x6Cx65x63x74ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

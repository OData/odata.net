namespace __GeneratedOdataV3.CstNodes.Rules
{
    public abstract class _ls32
    {
        private _ls32()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ls32 node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ls32._Ⲥh16_ʺx3Aʺ_h16Ↄ node, TContext context);
            protected internal abstract TResult Accept(_ls32._IPv4address node, TContext context);
        }
        
        public sealed class _Ⲥh16_ʺx3Aʺ_h16Ↄ : _ls32
        {
            public _Ⲥh16_ʺx3Aʺ_h16Ↄ(__GeneratedOdataV3.CstNodes.Inners._Ⲥh16_ʺx3Aʺ_h16Ↄ _Ⲥh16_ʺx3Aʺ_h16Ↄ_1)
            {
                this._Ⲥh16_ʺx3Aʺ_h16Ↄ_1 = _Ⲥh16_ʺx3Aʺ_h16Ↄ_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._Ⲥh16_ʺx3Aʺ_h16Ↄ _Ⲥh16_ʺx3Aʺ_h16Ↄ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _IPv4address : _ls32
        {
            public _IPv4address(__GeneratedOdataV3.CstNodes.Rules._IPv4address _IPv4address_1)
            {
                this._IPv4address_1 = _IPv4address_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._IPv4address _IPv4address_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

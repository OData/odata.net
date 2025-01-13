namespace __GeneratedOdata.CstNodes.Inners
{
    public abstract class _ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute
    {
        private _ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute._ʺx5Aʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute._SIGN_hour_ʺx3Aʺ_minute node, TContext context);
        }
        
        public sealed class _ʺx5Aʺ : _ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute
        {
            public _ʺx5Aʺ(__GeneratedOdata.CstNodes.Inners._ʺx5Aʺ _ʺx5Aʺ_1)
            {
                this._ʺx5Aʺ_1 = _ʺx5Aʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx5Aʺ _ʺx5Aʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _SIGN_hour_ʺx3Aʺ_minute : _ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute
        {
            public _SIGN_hour_ʺx3Aʺ_minute(__GeneratedOdata.CstNodes.Rules._SIGN _SIGN_1, __GeneratedOdata.CstNodes.Rules._hour _hour_1, __GeneratedOdata.CstNodes.Inners._ʺx3Aʺ _ʺx3Aʺ_1, __GeneratedOdata.CstNodes.Rules._minute _minute_1)
            {
                this._SIGN_1 = _SIGN_1;
                this._hour_1 = _hour_1;
                this._ʺx3Aʺ_1 = _ʺx3Aʺ_1;
                this._minute_1 = _minute_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._SIGN _SIGN_1 { get; }
            public __GeneratedOdata.CstNodes.Rules._hour _hour_1 { get; }
            public __GeneratedOdata.CstNodes.Inners._ʺx3Aʺ _ʺx3Aʺ_1 { get; }
            public __GeneratedOdata.CstNodes.Rules._minute _minute_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

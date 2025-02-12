namespace __GeneratedOdataV2.CstNodes.Inners
{
    public abstract class _unreservedⳆsubⲻdelimsⳆʺx3Aʺ
    {
        private _unreservedⳆsubⲻdelimsⳆʺx3Aʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_unreservedⳆsubⲻdelimsⳆʺx3Aʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_unreservedⳆsubⲻdelimsⳆʺx3Aʺ._unreserved node, TContext context);
            protected internal abstract TResult Accept(_unreservedⳆsubⲻdelimsⳆʺx3Aʺ._subⲻdelims node, TContext context);
            protected internal abstract TResult Accept(_unreservedⳆsubⲻdelimsⳆʺx3Aʺ._ʺx3Aʺ node, TContext context);
        }
        
        public sealed class _unreserved : _unreservedⳆsubⲻdelimsⳆʺx3Aʺ
        {
            public _unreserved(__GeneratedOdataV2.CstNodes.Rules._unreserved _unreserved_1)
            {
                this._unreserved_1 = _unreserved_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._unreserved _unreserved_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _subⲻdelims : _unreservedⳆsubⲻdelimsⳆʺx3Aʺ
        {
            public _subⲻdelims(__GeneratedOdataV2.CstNodes.Rules._subⲻdelims _subⲻdelims_1)
            {
                this._subⲻdelims_1 = _subⲻdelims_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._subⲻdelims _subⲻdelims_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx3Aʺ : _unreservedⳆsubⲻdelimsⳆʺx3Aʺ
        {
            private _ʺx3Aʺ()
            {
                this._ʺx3Aʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx3Aʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx3Aʺ _ʺx3Aʺ_1 { get; }
            public static _ʺx3Aʺ Instance { get; } = new _ʺx3Aʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

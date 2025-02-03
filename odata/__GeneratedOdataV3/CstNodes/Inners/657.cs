namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ
    {
        private _unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ._unreserved node, TContext context);
            protected internal abstract TResult Accept(_unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ._pctⲻencoded node, TContext context);
            protected internal abstract TResult Accept(_unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ._subⲻdelims node, TContext context);
            protected internal abstract TResult Accept(_unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ._ʺx3Aʺ node, TContext context);
        }
        
        public sealed class _unreserved : _unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ
        {
            public _unreserved(__GeneratedOdataV3.CstNodes.Rules._unreserved _unreserved_1)
            {
                this._unreserved_1 = _unreserved_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._unreserved _unreserved_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _pctⲻencoded : _unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ
        {
            public _pctⲻencoded(__GeneratedOdataV3.CstNodes.Rules._pctⲻencoded _pctⲻencoded_1)
            {
                this._pctⲻencoded_1 = _pctⲻencoded_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._pctⲻencoded _pctⲻencoded_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _subⲻdelims : _unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ
        {
            public _subⲻdelims(__GeneratedOdataV3.CstNodes.Rules._subⲻdelims _subⲻdelims_1)
            {
                this._subⲻdelims_1 = _subⲻdelims_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._subⲻdelims _subⲻdelims_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx3Aʺ : _unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ
        {
            public _ʺx3Aʺ(__GeneratedOdataV3.CstNodes.Inners._ʺx3Aʺ _ʺx3Aʺ_1)
            {
                this._ʺx3Aʺ_1 = _ʺx3Aʺ_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx3Aʺ _ʺx3Aʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

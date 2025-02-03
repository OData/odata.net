namespace __GeneratedOdataV2.CstNodes.Inners
{
    public abstract class _unreservedⳆpctⲻencodedⳆsubⲻdelims
    {
        private _unreservedⳆpctⲻencodedⳆsubⲻdelims()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_unreservedⳆpctⲻencodedⳆsubⲻdelims node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_unreservedⳆpctⲻencodedⳆsubⲻdelims._unreserved node, TContext context);
            protected internal abstract TResult Accept(_unreservedⳆpctⲻencodedⳆsubⲻdelims._pctⲻencoded node, TContext context);
            protected internal abstract TResult Accept(_unreservedⳆpctⲻencodedⳆsubⲻdelims._subⲻdelims node, TContext context);
        }
        
        public sealed class _unreserved : _unreservedⳆpctⲻencodedⳆsubⲻdelims
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
        
        public sealed class _pctⲻencoded : _unreservedⳆpctⲻencodedⳆsubⲻdelims
        {
            public _pctⲻencoded(__GeneratedOdataV2.CstNodes.Rules._pctⲻencoded _pctⲻencoded_1)
            {
                this._pctⲻencoded_1 = _pctⲻencoded_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._pctⲻencoded _pctⲻencoded_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _subⲻdelims : _unreservedⳆpctⲻencodedⳆsubⲻdelims
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
    }
    
}

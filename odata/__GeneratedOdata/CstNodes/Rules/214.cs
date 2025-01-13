namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _primitiveLiteralInJSON
    {
        private _primitiveLiteralInJSON()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_primitiveLiteralInJSON node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_primitiveLiteralInJSON._stringInJSON node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteralInJSON._numberInJSON node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteralInJSON._ʺx74x72x75x65ʺ node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteralInJSON._ʺx66x61x6Cx73x65ʺ node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteralInJSON._ʺx6Ex75x6Cx6Cʺ node, TContext context);
        }
        
        public sealed class _stringInJSON : _primitiveLiteralInJSON
        {
            public _stringInJSON(__GeneratedOdata.CstNodes.Rules._stringInJSON _stringInJSON_1)
            {
                this._stringInJSON_1 = _stringInJSON_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._stringInJSON _stringInJSON_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _numberInJSON : _primitiveLiteralInJSON
        {
            public _numberInJSON(__GeneratedOdata.CstNodes.Rules._numberInJSON _numberInJSON_1)
            {
                this._numberInJSON_1 = _numberInJSON_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._numberInJSON _numberInJSON_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx74x72x75x65ʺ : _primitiveLiteralInJSON
        {
            public _ʺx74x72x75x65ʺ(__GeneratedOdata.CstNodes.Inners._ʺx74x72x75x65ʺ _ʺx74x72x75x65ʺ_1)
            {
                this._ʺx74x72x75x65ʺ_1 = _ʺx74x72x75x65ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx74x72x75x65ʺ _ʺx74x72x75x65ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx66x61x6Cx73x65ʺ : _primitiveLiteralInJSON
        {
            public _ʺx66x61x6Cx73x65ʺ(__GeneratedOdata.CstNodes.Inners._ʺx66x61x6Cx73x65ʺ _ʺx66x61x6Cx73x65ʺ_1)
            {
                this._ʺx66x61x6Cx73x65ʺ_1 = _ʺx66x61x6Cx73x65ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx66x61x6Cx73x65ʺ _ʺx66x61x6Cx73x65ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx6Ex75x6Cx6Cʺ : _primitiveLiteralInJSON
        {
            public _ʺx6Ex75x6Cx6Cʺ(__GeneratedOdata.CstNodes.Inners._ʺx6Ex75x6Cx6Cʺ _ʺx6Ex75x6Cx6Cʺ_1)
            {
                this._ʺx6Ex75x6Cx6Cʺ_1 = _ʺx6Ex75x6Cx6Cʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx6Ex75x6Cx6Cʺ _ʺx6Ex75x6Cx6Cʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

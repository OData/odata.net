namespace __GeneratedOdataV4.CstNodes.Rules
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
            public _stringInJSON(__GeneratedOdataV4.CstNodes.Rules._stringInJSON _stringInJSON_1)
            {
                this._stringInJSON_1 = _stringInJSON_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._stringInJSON _stringInJSON_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _numberInJSON : _primitiveLiteralInJSON
        {
            public _numberInJSON(__GeneratedOdataV4.CstNodes.Rules._numberInJSON _numberInJSON_1)
            {
                this._numberInJSON_1 = _numberInJSON_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._numberInJSON _numberInJSON_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx74x72x75x65ʺ : _primitiveLiteralInJSON
        {
            private _ʺx74x72x75x65ʺ()
            {
                this._ʺx74x72x75x65ʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx74x72x75x65ʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx74x72x75x65ʺ _ʺx74x72x75x65ʺ_1 { get; }
            public static _ʺx74x72x75x65ʺ Instance { get; } = new _ʺx74x72x75x65ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx66x61x6Cx73x65ʺ : _primitiveLiteralInJSON
        {
            private _ʺx66x61x6Cx73x65ʺ()
            {
                this._ʺx66x61x6Cx73x65ʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx66x61x6Cx73x65ʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx66x61x6Cx73x65ʺ _ʺx66x61x6Cx73x65ʺ_1 { get; }
            public static _ʺx66x61x6Cx73x65ʺ Instance { get; } = new _ʺx66x61x6Cx73x65ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx6Ex75x6Cx6Cʺ : _primitiveLiteralInJSON
        {
            private _ʺx6Ex75x6Cx6Cʺ()
            {
                this._ʺx6Ex75x6Cx6Cʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx6Ex75x6Cx6Cʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx6Ex75x6Cx6Cʺ _ʺx6Ex75x6Cx6Cʺ_1 { get; }
            public static _ʺx6Ex75x6Cx6Cʺ Instance { get; } = new _ʺx6Ex75x6Cx6Cʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

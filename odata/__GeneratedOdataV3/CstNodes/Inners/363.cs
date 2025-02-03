namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri
    {
        private _complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._complexInUri node, TContext context);
            protected internal abstract TResult Accept(_complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._complexColInUri node, TContext context);
            protected internal abstract TResult Accept(_complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._primitiveLiteralInJSON node, TContext context);
            protected internal abstract TResult Accept(_complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._primitiveColInUri node, TContext context);
        }
        
        public sealed class _complexInUri : _complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri
        {
            public _complexInUri(__GeneratedOdataV3.CstNodes.Rules._complexInUri _complexInUri_1)
            {
                this._complexInUri_1 = _complexInUri_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._complexInUri _complexInUri_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _complexColInUri : _complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri
        {
            public _complexColInUri(__GeneratedOdataV3.CstNodes.Rules._complexColInUri _complexColInUri_1)
            {
                this._complexColInUri_1 = _complexColInUri_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._complexColInUri _complexColInUri_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _primitiveLiteralInJSON : _complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri
        {
            public _primitiveLiteralInJSON(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON _primitiveLiteralInJSON_1)
            {
                this._primitiveLiteralInJSON_1 = _primitiveLiteralInJSON_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON _primitiveLiteralInJSON_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _primitiveColInUri : _complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri
        {
            public _primitiveColInUri(__GeneratedOdataV3.CstNodes.Rules._primitiveColInUri _primitiveColInUri_1)
            {
                this._primitiveColInUri_1 = _primitiveColInUri_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._primitiveColInUri _primitiveColInUri_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

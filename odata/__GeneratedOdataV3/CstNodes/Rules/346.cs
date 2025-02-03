namespace __GeneratedOdataV3.CstNodes.Rules
{
    public abstract class _header
    {
        private _header()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_header node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_header._contentⲻid node, TContext context);
            protected internal abstract TResult Accept(_header._entityid node, TContext context);
            protected internal abstract TResult Accept(_header._isolation node, TContext context);
            protected internal abstract TResult Accept(_header._odataⲻmaxversion node, TContext context);
            protected internal abstract TResult Accept(_header._odataⲻversion node, TContext context);
            protected internal abstract TResult Accept(_header._prefer node, TContext context);
        }
        
        public sealed class _contentⲻid : _header
        {
            public _contentⲻid(__GeneratedOdataV3.CstNodes.Rules._contentⲻid _contentⲻid_1)
            {
                this._contentⲻid_1 = _contentⲻid_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._contentⲻid _contentⲻid_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _entityid : _header
        {
            public _entityid(__GeneratedOdataV3.CstNodes.Rules._entityid _entityid_1)
            {
                this._entityid_1 = _entityid_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._entityid _entityid_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _isolation : _header
        {
            public _isolation(__GeneratedOdataV3.CstNodes.Rules._isolation _isolation_1)
            {
                this._isolation_1 = _isolation_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._isolation _isolation_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _odataⲻmaxversion : _header
        {
            public _odataⲻmaxversion(__GeneratedOdataV3.CstNodes.Rules._odataⲻmaxversion _odataⲻmaxversion_1)
            {
                this._odataⲻmaxversion_1 = _odataⲻmaxversion_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._odataⲻmaxversion _odataⲻmaxversion_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _odataⲻversion : _header
        {
            public _odataⲻversion(__GeneratedOdataV3.CstNodes.Rules._odataⲻversion _odataⲻversion_1)
            {
                this._odataⲻversion_1 = _odataⲻversion_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._odataⲻversion _odataⲻversion_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _prefer : _header
        {
            public _prefer(__GeneratedOdataV3.CstNodes.Rules._prefer _prefer_1)
            {
                this._prefer_1 = _prefer_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._prefer _prefer_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

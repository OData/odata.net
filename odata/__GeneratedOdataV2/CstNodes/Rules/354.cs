namespace __GeneratedOdataV2.CstNodes.Rules
{
    public abstract class _preference
    {
        private _preference()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_preference node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_preference._allowEntityReferencesPreference node, TContext context);
            protected internal abstract TResult Accept(_preference._callbackPreference node, TContext context);
            protected internal abstract TResult Accept(_preference._continueOnErrorPreference node, TContext context);
            protected internal abstract TResult Accept(_preference._includeAnnotationsPreference node, TContext context);
            protected internal abstract TResult Accept(_preference._maxpagesizePreference node, TContext context);
            protected internal abstract TResult Accept(_preference._respondAsyncPreference node, TContext context);
            protected internal abstract TResult Accept(_preference._returnPreference node, TContext context);
            protected internal abstract TResult Accept(_preference._trackChangesPreference node, TContext context);
            protected internal abstract TResult Accept(_preference._waitPreference node, TContext context);
        }
        
        public sealed class _allowEntityReferencesPreference : _preference
        {
            public _allowEntityReferencesPreference(__GeneratedOdataV2.CstNodes.Rules._allowEntityReferencesPreference _allowEntityReferencesPreference_1)
            {
                this._allowEntityReferencesPreference_1 = _allowEntityReferencesPreference_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._allowEntityReferencesPreference _allowEntityReferencesPreference_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _callbackPreference : _preference
        {
            public _callbackPreference(__GeneratedOdataV2.CstNodes.Rules._callbackPreference _callbackPreference_1)
            {
                this._callbackPreference_1 = _callbackPreference_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._callbackPreference _callbackPreference_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _continueOnErrorPreference : _preference
        {
            public _continueOnErrorPreference(__GeneratedOdataV2.CstNodes.Rules._continueOnErrorPreference _continueOnErrorPreference_1)
            {
                this._continueOnErrorPreference_1 = _continueOnErrorPreference_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._continueOnErrorPreference _continueOnErrorPreference_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _includeAnnotationsPreference : _preference
        {
            public _includeAnnotationsPreference(__GeneratedOdataV2.CstNodes.Rules._includeAnnotationsPreference _includeAnnotationsPreference_1)
            {
                this._includeAnnotationsPreference_1 = _includeAnnotationsPreference_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._includeAnnotationsPreference _includeAnnotationsPreference_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _maxpagesizePreference : _preference
        {
            public _maxpagesizePreference(__GeneratedOdataV2.CstNodes.Rules._maxpagesizePreference _maxpagesizePreference_1)
            {
                this._maxpagesizePreference_1 = _maxpagesizePreference_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._maxpagesizePreference _maxpagesizePreference_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _respondAsyncPreference : _preference
        {
            private _respondAsyncPreference()
            {
                this._respondAsyncPreference_1 = __GeneratedOdataV2.CstNodes.Rules._respondAsyncPreference.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._respondAsyncPreference _respondAsyncPreference_1 { get; }
            public static _respondAsyncPreference Instance { get; } = new _respondAsyncPreference();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _returnPreference : _preference
        {
            public _returnPreference(__GeneratedOdataV2.CstNodes.Rules._returnPreference _returnPreference_1)
            {
                this._returnPreference_1 = _returnPreference_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._returnPreference _returnPreference_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _trackChangesPreference : _preference
        {
            public _trackChangesPreference(__GeneratedOdataV2.CstNodes.Rules._trackChangesPreference _trackChangesPreference_1)
            {
                this._trackChangesPreference_1 = _trackChangesPreference_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._trackChangesPreference _trackChangesPreference_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _waitPreference : _preference
        {
            public _waitPreference(__GeneratedOdataV2.CstNodes.Rules._waitPreference _waitPreference_1)
            {
                this._waitPreference_1 = _waitPreference_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._waitPreference _waitPreference_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

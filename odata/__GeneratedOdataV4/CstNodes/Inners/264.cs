namespace __GeneratedOdataV4.CstNodes.Inners
{
    public abstract class _entitySetName_keyPredicateⳆsingletonEntity
    {
        private _entitySetName_keyPredicateⳆsingletonEntity()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_entitySetName_keyPredicateⳆsingletonEntity node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_entitySetName_keyPredicateⳆsingletonEntity._entitySetName_keyPredicate node, TContext context);
            protected internal abstract TResult Accept(_entitySetName_keyPredicateⳆsingletonEntity._singletonEntity node, TContext context);
        }
        
        public sealed class _entitySetName_keyPredicate : _entitySetName_keyPredicateⳆsingletonEntity
        {
            public _entitySetName_keyPredicate(__GeneratedOdataV4.CstNodes.Rules._entitySetName _entitySetName_1, __GeneratedOdataV4.CstNodes.Rules._keyPredicate _keyPredicate_1)
            {
                this._entitySetName_1 = _entitySetName_1;
                this._keyPredicate_1 = _keyPredicate_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._entitySetName _entitySetName_1 { get; }
            public __GeneratedOdataV4.CstNodes.Rules._keyPredicate _keyPredicate_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _singletonEntity : _entitySetName_keyPredicateⳆsingletonEntity
        {
            public _singletonEntity(__GeneratedOdataV4.CstNodes.Rules._singletonEntity _singletonEntity_1)
            {
                this._singletonEntity_1 = _singletonEntity_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._singletonEntity _singletonEntity_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}

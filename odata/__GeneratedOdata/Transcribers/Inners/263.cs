namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _entitySetName_keyPredicateⳆsingletonEntityTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity>
    {
        private _entitySetName_keyPredicateⳆsingletonEntityTranscriber()
        {
        }
        
        public static _entitySetName_keyPredicateⳆsingletonEntityTranscriber Instance { get; } = new _entitySetName_keyPredicateⳆsingletonEntityTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity._entitySetName_keyPredicate node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._entitySetNameTranscriber.Instance.Transcribe(node._entitySetName_1, context);
__GeneratedOdata.Trancsribers.Rules._keyPredicateTranscriber.Instance.Transcribe(node._keyPredicate_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity._singletonEntity node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._singletonEntityTranscriber.Instance.Transcribe(node._singletonEntity_1, context);

return default;
            }
        }
    }
    
}

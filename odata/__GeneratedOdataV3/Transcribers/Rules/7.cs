namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _keyPredicateTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._keyPredicate>
    {
        private _keyPredicateTranscriber()
        {
        }
        
        public static _keyPredicateTranscriber Instance { get; } = new _keyPredicateTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._keyPredicate value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Rules._keyPredicate.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._keyPredicate._simpleKey node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._simpleKeyTranscriber.Instance.Transcribe(node._simpleKey_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._keyPredicate._compoundKey node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._compoundKeyTranscriber.Instance.Transcribe(node._compoundKey_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._keyPredicate._keyPathSegments node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._keyPathSegmentsTranscriber.Instance.Transcribe(node._keyPathSegments_1, context);

return default;
            }
        }
    }
    
}

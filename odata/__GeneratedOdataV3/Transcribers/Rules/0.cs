namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _dummyStartRuleTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._dummyStartRule>
    {
        private _dummyStartRuleTranscriber()
        {
        }
        
        public static _dummyStartRuleTranscriber Instance { get; } = new _dummyStartRuleTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._dummyStartRule value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Rules._dummyStartRule.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._dummyStartRule._odataUri node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._odataUriTranscriber.Instance.Transcribe(node._odataUri_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._dummyStartRule._header node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._headerTranscriber.Instance.Transcribe(node._header_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._dummyStartRule._primitiveValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._primitiveValueTranscriber.Instance.Transcribe(node._primitiveValue_1, context);

return default;
            }
        }
    }
    
}

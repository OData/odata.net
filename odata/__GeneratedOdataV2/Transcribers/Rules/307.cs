namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _singleEnumValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._singleEnumValue>
    {
        private _singleEnumValueTranscriber()
        {
        }
        
        public static _singleEnumValueTranscriber Instance { get; } = new _singleEnumValueTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._singleEnumValue value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV2.CstNodes.Rules._singleEnumValue.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._singleEnumValue._enumerationMember node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._enumerationMemberTranscriber.Instance.Transcribe(node._enumerationMember_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._singleEnumValue._enumMemberValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._enumMemberValueTranscriber.Instance.Transcribe(node._enumMemberValue_1, context);

return default;
            }
        }
    }
    
}

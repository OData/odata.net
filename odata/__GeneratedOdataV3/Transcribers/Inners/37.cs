namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _parameterAliasⳆkeyPropertyValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._parameterAliasⳆkeyPropertyValue>
    {
        private _parameterAliasⳆkeyPropertyValueTranscriber()
        {
        }
        
        public static _parameterAliasⳆkeyPropertyValueTranscriber Instance { get; } = new _parameterAliasⳆkeyPropertyValueTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._parameterAliasⳆkeyPropertyValue value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Inners._parameterAliasⳆkeyPropertyValue.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._parameterAliasⳆkeyPropertyValue._parameterAlias node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._parameterAliasTranscriber.Instance.Transcribe(node._parameterAlias_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._parameterAliasⳆkeyPropertyValue._keyPropertyValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._keyPropertyValueTranscriber.Instance.Transcribe(node._keyPropertyValue_1, context);

return default;
            }
        }
    }
    
}

namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _computedPropertyTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._computedProperty>
    {
        private _computedPropertyTranscriber()
        {
        }
        
        public static _computedPropertyTranscriber Instance { get; } = new _computedPropertyTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._computedProperty value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}

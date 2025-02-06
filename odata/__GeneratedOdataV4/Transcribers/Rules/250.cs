namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _complexColPropertyTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._complexColProperty>
    {
        private _complexColPropertyTranscriber()
        {
        }
        
        public static _complexColPropertyTranscriber Instance { get; } = new _complexColPropertyTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._complexColProperty value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}

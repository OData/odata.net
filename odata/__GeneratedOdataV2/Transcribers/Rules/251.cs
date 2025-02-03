namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _streamPropertyTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._streamProperty>
    {
        private _streamPropertyTranscriber()
        {
        }
        
        public static _streamPropertyTranscriber Instance { get; } = new _streamPropertyTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._streamProperty value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}

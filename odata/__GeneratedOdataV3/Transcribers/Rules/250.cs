namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _complexColPropertyTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._complexColProperty>
    {
        private _complexColPropertyTranscriber()
        {
        }
        
        public static _complexColPropertyTranscriber Instance { get; } = new _complexColPropertyTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._complexColProperty value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}

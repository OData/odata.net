namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _complexPropertyTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._complexProperty>
    {
        private _complexPropertyTranscriber()
        {
        }
        
        public static _complexPropertyTranscriber Instance { get; } = new _complexPropertyTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._complexProperty value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}

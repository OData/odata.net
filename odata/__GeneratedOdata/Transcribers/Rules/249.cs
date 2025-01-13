namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _complexPropertyTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._complexProperty>
    {
        private _complexPropertyTranscriber()
        {
        }
        
        public static _complexPropertyTranscriber Instance { get; } = new _complexPropertyTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._complexProperty value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}

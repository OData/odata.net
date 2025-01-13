namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _complexColPropertyTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._complexColProperty>
    {
        private _complexColPropertyTranscriber()
        {
        }
        
        public static _complexColPropertyTranscriber Instance { get; } = new _complexColPropertyTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._complexColProperty value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}

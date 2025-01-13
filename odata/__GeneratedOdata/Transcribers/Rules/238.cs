namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _termNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._termName>
    {
        private _termNameTranscriber()
        {
        }
        
        public static _termNameTranscriber Instance { get; } = new _termNameTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._termName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}

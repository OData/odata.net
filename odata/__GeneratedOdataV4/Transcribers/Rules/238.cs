namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _termNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._termName>
    {
        private _termNameTranscriber()
        {
        }
        
        public static _termNameTranscriber Instance { get; } = new _termNameTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._termName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}

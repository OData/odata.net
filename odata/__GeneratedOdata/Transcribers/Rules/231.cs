namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _entitySetNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._entitySetName>
    {
        private _entitySetNameTranscriber()
        {
        }
        
        public static _entitySetNameTranscriber Instance { get; } = new _entitySetNameTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._entitySetName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}

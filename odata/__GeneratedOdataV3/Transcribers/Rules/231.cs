namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _entitySetNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._entitySetName>
    {
        private _entitySetNameTranscriber()
        {
        }
        
        public static _entitySetNameTranscriber Instance { get; } = new _entitySetNameTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._entitySetName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}

namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _keyPropertyAliasTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._keyPropertyAlias>
    {
        private _keyPropertyAliasTranscriber()
        {
        }
        
        public static _keyPropertyAliasTranscriber Instance { get; } = new _keyPropertyAliasTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._keyPropertyAlias value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}

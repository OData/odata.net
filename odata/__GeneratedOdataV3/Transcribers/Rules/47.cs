namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _parameterAliasTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._parameterAlias>
    {
        private _parameterAliasTranscriber()
        {
        }
        
        public static _parameterAliasTranscriber Instance { get; } = new _parameterAliasTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._parameterAlias value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._ATTranscriber.Instance.Transcribe(value._AT_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}

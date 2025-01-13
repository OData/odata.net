namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _parameterAliasTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._parameterAlias>
    {
        private _parameterAliasTranscriber()
        {
        }
        
        public static _parameterAliasTranscriber Instance { get; } = new _parameterAliasTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._parameterAlias value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._ATTranscriber.Instance.Transcribe(value._AT_1, builder);
__GeneratedOdata.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}

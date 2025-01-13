namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _aliasAndValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._aliasAndValue>
    {
        private _aliasAndValueTranscriber()
        {
        }
        
        public static _aliasAndValueTranscriber Instance { get; } = new _aliasAndValueTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._aliasAndValue value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._parameterAliasTranscriber.Instance.Transcribe(value._parameterAlias_1, builder);
__GeneratedOdata.Trancsribers.Rules._EQTranscriber.Instance.Transcribe(value._EQ_1, builder);
__GeneratedOdata.Trancsribers.Rules._parameterValueTranscriber.Instance.Transcribe(value._parameterValue_1, builder);

        }
    }
    
}

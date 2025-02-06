namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _aliasAndValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._aliasAndValue>
    {
        private _aliasAndValueTranscriber()
        {
        }
        
        public static _aliasAndValueTranscriber Instance { get; } = new _aliasAndValueTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._aliasAndValue value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._parameterAliasTranscriber.Instance.Transcribe(value._parameterAlias_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._EQTranscriber.Instance.Transcribe(value._EQ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._parameterValueTranscriber.Instance.Transcribe(value._parameterValue_1, builder);

        }
    }
    
}

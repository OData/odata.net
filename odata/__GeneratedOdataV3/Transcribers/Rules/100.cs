namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _nameAndValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._nameAndValue>
    {
        private _nameAndValueTranscriber()
        {
        }
        
        public static _nameAndValueTranscriber Instance { get; } = new _nameAndValueTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._nameAndValue value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._parameterNameTranscriber.Instance.Transcribe(value._parameterName_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._EQTranscriber.Instance.Transcribe(value._EQ_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._parameterValueTranscriber.Instance.Transcribe(value._parameterValue_1, builder);

        }
    }
    
}

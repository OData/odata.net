namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _singleValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._singleValue>
    {
        private _singleValueTranscriber()
        {
        }
        
        public static _singleValueTranscriber Instance { get; } = new _singleValueTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._singleValue value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._decimalValueTranscriber.Instance.Transcribe(value._decimalValue_1, builder);

        }
    }
    
}

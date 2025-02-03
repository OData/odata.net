namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _doubleValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._doubleValue>
    {
        private _doubleValueTranscriber()
        {
        }
        
        public static _doubleValueTranscriber Instance { get; } = new _doubleValueTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._doubleValue value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._decimalValueTranscriber.Instance.Transcribe(value._decimalValue_1, builder);

        }
    }
    
}

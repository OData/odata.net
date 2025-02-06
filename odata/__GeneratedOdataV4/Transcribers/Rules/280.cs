namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _doubleValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._doubleValue>
    {
        private _doubleValueTranscriber()
        {
        }
        
        public static _doubleValueTranscriber Instance { get; } = new _doubleValueTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._doubleValue value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._decimalValueTranscriber.Instance.Transcribe(value._decimalValue_1, builder);

        }
    }
    
}

namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _doubleValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._doubleValue>
    {
        private _doubleValueTranscriber()
        {
        }
        
        public static _doubleValueTranscriber Instance { get; } = new _doubleValueTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._doubleValue value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._decimalValueTranscriber.Instance.Transcribe(value._decimalValue_1, builder);

        }
    }
    
}

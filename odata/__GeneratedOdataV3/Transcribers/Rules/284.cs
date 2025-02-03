namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _byteValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._byteValue>
    {
        private _byteValueTranscriber()
        {
        }
        
        public static _byteValueTranscriber Instance { get; } = new _byteValueTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._byteValue value, System.Text.StringBuilder builder)
        {
            foreach (var _DIGIT_1 in value._DIGIT_1)
{
__GeneratedOdataV3.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}

        }
    }
    
}

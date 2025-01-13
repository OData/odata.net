namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _byteValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._byteValue>
    {
        private _byteValueTranscriber()
        {
        }
        
        public static _byteValueTranscriber Instance { get; } = new _byteValueTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._byteValue value, System.Text.StringBuilder builder)
        {
            foreach (var _DIGIT_1 in value._DIGIT_1)
{
__GeneratedOdata.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}

        }
    }
    
}

namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _sbyteValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._sbyteValue>
    {
        private _sbyteValueTranscriber()
        {
        }
        
        public static _sbyteValueTranscriber Instance { get; } = new _sbyteValueTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._sbyteValue value, System.Text.StringBuilder builder)
        {
            if (value._SIGN_1 != null)
{
__GeneratedOdataV2.Trancsribers.Rules._SIGNTranscriber.Instance.Transcribe(value._SIGN_1, builder);
}
foreach (var _DIGIT_1 in value._DIGIT_1)
{
__GeneratedOdataV2.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}

        }
    }
    
}

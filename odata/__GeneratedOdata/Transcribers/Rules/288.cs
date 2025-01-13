namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _int64ValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._int64Value>
    {
        private _int64ValueTranscriber()
        {
        }
        
        public static _int64ValueTranscriber Instance { get; } = new _int64ValueTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._int64Value value, System.Text.StringBuilder builder)
        {
            if (value._SIGN_1 != null)
{
__GeneratedOdata.Trancsribers.Rules._SIGNTranscriber.Instance.Transcribe(value._SIGN_1, builder);
}
foreach (var _DIGIT_1 in value._DIGIT_1)
{
__GeneratedOdata.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}

        }
    }
    
}

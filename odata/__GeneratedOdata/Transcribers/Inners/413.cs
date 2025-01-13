namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _oneToNine_ЖDIGITTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._oneToNine_ЖDIGIT>
    {
        private _oneToNine_ЖDIGITTranscriber()
        {
        }
        
        public static _oneToNine_ЖDIGITTranscriber Instance { get; } = new _oneToNine_ЖDIGITTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._oneToNine_ЖDIGIT value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._oneToNineTranscriber.Instance.Transcribe(value._oneToNine_1, builder);
foreach (var _DIGIT_1 in value._DIGIT_1)
{
__GeneratedOdata.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}

        }
    }
    
}

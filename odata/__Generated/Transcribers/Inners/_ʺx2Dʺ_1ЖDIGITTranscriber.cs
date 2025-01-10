namespace __Generated.Trancsribers.Inners
{
    public sealed class _ʺx2Dʺ_1ЖDIGITTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Inners._ʺx2Dʺ_1ЖDIGIT>
    {
        private _ʺx2Dʺ_1ЖDIGITTranscriber()
        {
        }
        
        public static _ʺx2Dʺ_1ЖDIGITTranscriber Instance { get; } = new _ʺx2Dʺ_1ЖDIGITTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Inners._ʺx2Dʺ_1ЖDIGIT value, System.Text.StringBuilder builder)
        {
            __Generated.Trancsribers.Inners._ʺx2DʺTranscriber.Instance.Transcribe(value._ʺx2Dʺ_1, builder);
foreach (var _DIGIT_1 in value._DIGIT_1)
{
__Generated.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}

        }
    }
    
}

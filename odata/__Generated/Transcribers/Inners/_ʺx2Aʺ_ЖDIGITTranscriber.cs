namespace __Generated.Trancsribers.Inners
{
    public sealed class _ʺx2Aʺ_ЖDIGITTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Inners._ʺx2Aʺ_ЖDIGIT>
    {
        private _ʺx2Aʺ_ЖDIGITTranscriber()
        {
        }
        
        public static _ʺx2Aʺ_ЖDIGITTranscriber Instance { get; } = new _ʺx2Aʺ_ЖDIGITTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Inners._ʺx2Aʺ_ЖDIGIT value, System.Text.StringBuilder builder)
        {
            __Generated.Trancsribers.Inners._ʺx2AʺTranscriber.Instance.Transcribe(value._ʺx2Aʺ_1, builder);
foreach (var _DIGIT_1 in value._DIGIT_1)
{
__Generated.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}

        }
    }
    
}

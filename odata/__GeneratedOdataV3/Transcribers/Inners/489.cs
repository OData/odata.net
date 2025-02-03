namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _ʺx2Eʺ_1ЖDIGITTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ_1ЖDIGIT>
    {
        private _ʺx2Eʺ_1ЖDIGITTranscriber()
        {
        }
        
        public static _ʺx2Eʺ_1ЖDIGITTranscriber Instance { get; } = new _ʺx2Eʺ_1ЖDIGITTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ_1ЖDIGIT value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
foreach (var _DIGIT_1 in value._DIGIT_1)
{
__GeneratedOdataV3.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}

        }
    }
    
}

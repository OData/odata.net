namespace __GeneratedOdataV2.Trancsribers.Inners
{
    public sealed class _ʺx65ʺ_꘡SIGN꘡_1ЖDIGITTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Inners._ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT>
    {
        private _ʺx65ʺ_꘡SIGN꘡_1ЖDIGITTranscriber()
        {
        }
        
        public static _ʺx65ʺ_꘡SIGN꘡_1ЖDIGITTranscriber Instance { get; } = new _ʺx65ʺ_꘡SIGN꘡_1ЖDIGITTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Inners._ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Inners._ʺx65ʺTranscriber.Instance.Transcribe(value._ʺx65ʺ_1, builder);
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

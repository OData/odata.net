namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ>
    {
        private _1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺTranscriber()
        {
        }
        
        public static _1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺTranscriber Instance { get; } = new _1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ value, System.Text.StringBuilder builder)
        {
            foreach (var _DIGIT_1 in value._DIGIT_1)
{
__GeneratedOdataV3.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}
if (value._ʺx2Eʺ_1ЖDIGIT_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._ʺx2Eʺ_1ЖDIGITTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1ЖDIGIT_1, builder);
}
__GeneratedOdataV3.Trancsribers.Inners._ʺx53ʺTranscriber.Instance.Transcribe(value._ʺx53ʺ_1, builder);

        }
    }
    
}

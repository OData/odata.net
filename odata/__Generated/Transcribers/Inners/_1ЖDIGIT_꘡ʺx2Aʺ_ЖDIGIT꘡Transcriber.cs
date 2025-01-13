namespace __Generated.Trancsribers.Inners
{
    public sealed class _1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡Transcriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Inners._1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡>
    {
        private _1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡Transcriber()
        {
        }
        
        public static _1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡Transcriber Instance { get; } = new _1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡Transcriber();
        
        public void Transcribe(__Generated.CstNodes.Inners._1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡ value, System.Text.StringBuilder builder)
        {
            foreach (var _DIGIT_1 in value._DIGIT_1)
{
__Generated.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}
if (value._ʺx2Aʺ_ЖDIGIT_1 != null)
{
__Generated.Trancsribers.Inners._ʺx2Aʺ_ЖDIGITTranscriber.Instance.Transcribe(value._ʺx2Aʺ_ЖDIGIT_1, builder);
}

        }
    }
    
}

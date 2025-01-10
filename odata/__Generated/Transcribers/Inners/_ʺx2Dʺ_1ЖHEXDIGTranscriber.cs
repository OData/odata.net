namespace __Generated.Trancsribers.Inners
{
    public sealed class _ʺx2Dʺ_1ЖHEXDIGTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Inners._ʺx2Dʺ_1ЖHEXDIG>
    {
        private _ʺx2Dʺ_1ЖHEXDIGTranscriber()
        {
        }
        
        public static _ʺx2Dʺ_1ЖHEXDIGTranscriber Instance { get; } = new _ʺx2Dʺ_1ЖHEXDIGTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Inners._ʺx2Dʺ_1ЖHEXDIG value, System.Text.StringBuilder builder)
        {
            __Generated.Trancsribers.Inners._ʺx2DʺTranscriber.Instance.Transcribe(value._ʺx2Dʺ_1, builder);
foreach (var _HEXDIG_1 in value._HEXDIG_1)
{
__Generated.Trancsribers.Rules._HEXDIGTranscriber.Instance.Transcribe(_HEXDIG_1, builder);
}

        }
    }
    
}

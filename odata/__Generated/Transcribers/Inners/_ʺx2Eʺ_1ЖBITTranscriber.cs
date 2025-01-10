namespace __Generated.Trancsribers.Inners
{
    public sealed class _ʺx2Eʺ_1ЖBITTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Inners._ʺx2Eʺ_1ЖBIT>
    {
        private _ʺx2Eʺ_1ЖBITTranscriber()
        {
        }
        
        public static _ʺx2Eʺ_1ЖBITTranscriber Instance { get; } = new _ʺx2Eʺ_1ЖBITTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Inners._ʺx2Eʺ_1ЖBIT value, System.Text.StringBuilder builder)
        {
            __Generated.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
foreach (var _BIT_1 in value._BIT_1)
{
__Generated.Trancsribers.Rules._BITTranscriber.Instance.Transcribe(_BIT_1, builder);
}

        }
    }
    
}

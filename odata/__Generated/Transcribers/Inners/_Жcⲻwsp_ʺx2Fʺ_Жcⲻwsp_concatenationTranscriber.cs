namespace __Generated.Trancsribers.Inners
{
    public sealed class _Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Inners._Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenation>
    {
        private _Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationTranscriber()
        {
        }
        
        public static _Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationTranscriber Instance { get; } = new _Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Inners._Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenation value, System.Text.StringBuilder builder)
        {
            foreach (var _cⲻwsp_1 in value._cⲻwsp_1)
{
__Generated.Trancsribers.Rules._cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp_1, builder);
}
__Generated.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(value._ʺx2Fʺ_1, builder);
foreach (var _cⲻwsp_2 in value._cⲻwsp_2)
{
__Generated.Trancsribers.Rules._cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp_2, builder);
}
__Generated.Trancsribers.Rules._concatenationTranscriber.Instance.Transcribe(value._concatenation_1, builder);

        }
    }
    
}

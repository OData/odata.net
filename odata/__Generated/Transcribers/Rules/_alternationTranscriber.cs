namespace __Generated.Trancsribers.Rules
{
    public sealed class _alternationTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Rules._alternation>
    {
        private _alternationTranscriber()
        {
        }
        
        public static _alternationTranscriber Instance { get; } = new _alternationTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Rules._alternation value, System.Text.StringBuilder builder)
        {
            __Generated.Trancsribers.Rules._concatenationTranscriber.Instance.Transcribe(value._concatenation_1, builder);
foreach (var _ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ_1 in value._ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ_1)
{
__Generated.Trancsribers.Inners._ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃTranscriber.Instance.Transcribe(_ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ_1, builder);
}

        }
    }
    
}

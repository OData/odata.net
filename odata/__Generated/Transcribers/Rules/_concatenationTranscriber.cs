namespace __Generated.Trancsribers.Rules
{
    public sealed class _concatenationTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Rules._concatenation>
    {
        private _concatenationTranscriber()
        {
        }
        
        public static _concatenationTranscriber Instance { get; } = new _concatenationTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Rules._concatenation value, System.Text.StringBuilder builder)
        {
            __Generated.Trancsribers.Rules._repetitionTranscriber.Instance.Transcribe(value._repetition_1, builder);
foreach (var _Ⲥ1Жcⲻwsp_repetitionↃ_1 in value._Ⲥ1Жcⲻwsp_repetitionↃ_1)
{
__Generated.Trancsribers.Inners._Ⲥ1Жcⲻwsp_repetitionↃTranscriber.Instance.Transcribe(_Ⲥ1Жcⲻwsp_repetitionↃ_1, builder);
}

        }
    }
    
}

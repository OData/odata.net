namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Inners;

    public sealed class _alternationTranscriber : ITranscriber<_alternation>
    {
        private _alternationTranscriber()
        {
        }

        public static _alternationTranscriber Instance { get; } = new _alternationTranscriber();

        public void Transcribe(_alternation value, StringBuilder builder)
        {
            _concatenationTranscriber.Instance.Transcribe(value._concatenation_1, builder);

            foreach (var _ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ in value._ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ_1)
            {
                _ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃTranscriber.Instance.Transcribe(_ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ, builder);
            }
        }
    }
}

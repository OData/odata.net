namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;


    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Inners;

    public sealed class _numⲻvalTranscriber : ITranscriber<_numⲻval>
    {
        private _numⲻvalTranscriber()
        {
        }

        public static _numⲻvalTranscriber Instance { get; } = new _numⲻvalTranscriber();

        public void Transcribe(_numⲻval value, StringBuilder builder)
        {
            _ʺx25ʺTranscriber.Instance.Transcribe(value._ʺx25ʺ_1, builder);
            _ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃTranscriber.Instance.Transcribe(value._ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ_1, builder);
        }
    }
}

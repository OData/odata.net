namespace __Generated.Trancsribers.Rules
{
    public sealed class _repetitionTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Rules._repetition>
    {
        private _repetitionTranscriber()
        {
        }
        
        public static _repetitionTranscriber Instance { get; } = new _repetitionTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Rules._repetition value, System.Text.StringBuilder builder)
        {
            if (value._repeat_1 != null)
{
__Generated.Trancsribers.Rules._repeatTranscriber.Instance.Transcribe(value._repeat_1, builder);
}
__Generated.Trancsribers.Rules._elementTranscriber.Instance.Transcribe(value._element_1, builder);

        }
    }
    
}

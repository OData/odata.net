namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _minuteTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._minute>
    {
        private _minuteTranscriber()
        {
        }
        
        public static _minuteTranscriber Instance { get; } = new _minuteTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._minute value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._zeroToFiftyNineTranscriber.Instance.Transcribe(value._zeroToFiftyNine_1, builder);

        }
    }
    
}

namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _minuteTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._minute>
    {
        private _minuteTranscriber()
        {
        }
        
        public static _minuteTranscriber Instance { get; } = new _minuteTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._minute value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._zeroToFiftyNineTranscriber.Instance.Transcribe(value._zeroToFiftyNine_1, builder);

        }
    }
    
}

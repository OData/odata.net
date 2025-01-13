namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _secondTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._second>
    {
        private _secondTranscriber()
        {
        }
        
        public static _secondTranscriber Instance { get; } = new _secondTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._second value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._zeroToFiftyNineTranscriber.Instance.Transcribe(value._zeroToFiftyNine_1, builder);

        }
    }
    
}

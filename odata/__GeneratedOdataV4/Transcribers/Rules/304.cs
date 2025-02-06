namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _fractionalSecondsTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._fractionalSeconds>
    {
        private _fractionalSecondsTranscriber()
        {
        }
        
        public static _fractionalSecondsTranscriber Instance { get; } = new _fractionalSecondsTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._fractionalSeconds value, System.Text.StringBuilder builder)
        {
            foreach (var _DIGIT_1 in value._DIGIT_1)
{
__GeneratedOdataV4.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}

        }
    }
    
}

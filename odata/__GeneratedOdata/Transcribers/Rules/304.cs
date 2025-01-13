namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _fractionalSecondsTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._fractionalSeconds>
    {
        private _fractionalSecondsTranscriber()
        {
        }
        
        public static _fractionalSecondsTranscriber Instance { get; } = new _fractionalSecondsTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._fractionalSeconds value, System.Text.StringBuilder builder)
        {
            foreach (var _DIGIT_1 in value._DIGIT_1)
{
__GeneratedOdata.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}

        }
    }
    
}

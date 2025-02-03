namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _portTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._port>
    {
        private _portTranscriber()
        {
        }
        
        public static _portTranscriber Instance { get; } = new _portTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._port value, System.Text.StringBuilder builder)
        {
            foreach (var _DIGIT_1 in value._DIGIT_1)
{
__GeneratedOdataV2.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}

        }
    }
    
}

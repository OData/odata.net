namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _fracTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._frac>
    {
        private _fracTranscriber()
        {
        }
        
        public static _fracTranscriber Instance { get; } = new _fracTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._frac value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
foreach (var _DIGIT_1 in value._DIGIT_1)
{
__GeneratedOdataV4.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}

        }
    }
    
}

namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _fracTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._frac>
    {
        private _fracTranscriber()
        {
        }
        
        public static _fracTranscriber Instance { get; } = new _fracTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._frac value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
foreach (var _DIGIT_1 in value._DIGIT_1)
{
__GeneratedOdata.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}

        }
    }
    
}

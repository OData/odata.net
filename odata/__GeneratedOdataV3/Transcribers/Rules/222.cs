namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _expTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._exp>
    {
        private _expTranscriber()
        {
        }
        
        public static _expTranscriber Instance { get; } = new _expTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._exp value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Inners._ʺx65ʺTranscriber.Instance.Transcribe(value._ʺx65ʺ_1, builder);
if (value._ʺx2DʺⳆʺx2Bʺ_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._ʺx2DʺⳆʺx2BʺTranscriber.Instance.Transcribe(value._ʺx2DʺⳆʺx2Bʺ_1, builder);
}
foreach (var _DIGIT_1 in value._DIGIT_1)
{
__GeneratedOdataV3.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}

        }
    }
    
}

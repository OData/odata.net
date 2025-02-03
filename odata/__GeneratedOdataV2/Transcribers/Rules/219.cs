namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _numberInJSONTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._numberInJSON>
    {
        private _numberInJSONTranscriber()
        {
        }
        
        public static _numberInJSONTranscriber Instance { get; } = new _numberInJSONTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._numberInJSON value, System.Text.StringBuilder builder)
        {
            if (value._ʺx2Dʺ_1 != null)
{
__GeneratedOdataV2.Trancsribers.Inners._ʺx2DʺTranscriber.Instance.Transcribe(value._ʺx2Dʺ_1, builder);
}
__GeneratedOdataV2.Trancsribers.Rules._intTranscriber.Instance.Transcribe(value._int_1, builder);
if (value._frac_1 != null)
{
__GeneratedOdataV2.Trancsribers.Rules._fracTranscriber.Instance.Transcribe(value._frac_1, builder);
}
if (value._exp_1 != null)
{
__GeneratedOdataV2.Trancsribers.Rules._expTranscriber.Instance.Transcribe(value._exp_1, builder);
}

        }
    }
    
}

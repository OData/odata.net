namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _ʺx3Fʺ_queryOptionsTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_queryOptions>
    {
        private _ʺx3Fʺ_queryOptionsTranscriber()
        {
        }
        
        public static _ʺx3Fʺ_queryOptionsTranscriber Instance { get; } = new _ʺx3Fʺ_queryOptionsTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_queryOptions value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Inners._ʺx3FʺTranscriber.Instance.Transcribe(value._ʺx3Fʺ_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._queryOptionsTranscriber.Instance.Transcribe(value._queryOptions_1, builder);

        }
    }
    
}

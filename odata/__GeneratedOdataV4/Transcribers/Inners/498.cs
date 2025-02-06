namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡Transcriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡>
    {
        private _ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡Transcriber()
        {
        }
        
        public static _ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡Transcriber Instance { get; } = new _ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡Transcriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡ value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Inners._ʺx3AʺTranscriber.Instance.Transcribe(value._ʺx3Aʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._secondTranscriber.Instance.Transcribe(value._second_1, builder);
if (value._ʺx2Eʺ_fractionalSeconds_1 != null)
{
__GeneratedOdataV4.Trancsribers.Inners._ʺx2Eʺ_fractionalSecondsTranscriber.Instance.Transcribe(value._ʺx2Eʺ_fractionalSeconds_1, builder);
}

        }
    }
    
}

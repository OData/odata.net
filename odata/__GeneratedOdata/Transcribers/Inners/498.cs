namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡Transcriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡>
    {
        private _ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡Transcriber()
        {
        }
        
        public static _ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡Transcriber Instance { get; } = new _ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡Transcriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡ value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ʺx3AʺTranscriber.Instance.Transcribe(value._ʺx3Aʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._secondTranscriber.Instance.Transcribe(value._second_1, builder);
if (value._ʺx2Eʺ_fractionalSeconds_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx2Eʺ_fractionalSecondsTranscriber.Instance.Transcribe(value._ʺx2Eʺ_fractionalSeconds_1, builder);
}

        }
    }
    
}

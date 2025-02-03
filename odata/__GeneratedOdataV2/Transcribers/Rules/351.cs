namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _odataⲻmaxversionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._odataⲻmaxversion>
    {
        private _odataⲻmaxversionTranscriber()
        {
        }
        
        public static _odataⲻmaxversionTranscriber Instance { get; } = new _odataⲻmaxversionTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._odataⲻmaxversion value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Inners._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6EʺTranscriber.Instance.Transcribe(value._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ_1, builder);
__GeneratedOdataV2.Trancsribers.Inners._ʺx3AʺTranscriber.Instance.Transcribe(value._ʺx3Aʺ_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._OWSTranscriber.Instance.Transcribe(value._OWS_1, builder);
foreach (var _DIGIT_1 in value._DIGIT_1)
{
__GeneratedOdataV2.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}
__GeneratedOdataV2.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
foreach (var _DIGIT_2 in value._DIGIT_2)
{
__GeneratedOdataV2.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_2, builder);
}

        }
    }
    
}

namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _pointData_ЖⲤCOMMA_pointDataↃTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._pointData_ЖⲤCOMMA_pointDataↃ>
    {
        private _pointData_ЖⲤCOMMA_pointDataↃTranscriber()
        {
        }
        
        public static _pointData_ЖⲤCOMMA_pointDataↃTranscriber Instance { get; } = new _pointData_ЖⲤCOMMA_pointDataↃTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._pointData_ЖⲤCOMMA_pointDataↃ value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._pointDataTranscriber.Instance.Transcribe(value._pointData_1, builder);
foreach (var _ⲤCOMMA_pointDataↃ_1 in value._ⲤCOMMA_pointDataↃ_1)
{
Inners._ⲤCOMMA_pointDataↃTranscriber.Instance.Transcribe(_ⲤCOMMA_pointDataↃ_1, builder);
}

        }
    }
    
}

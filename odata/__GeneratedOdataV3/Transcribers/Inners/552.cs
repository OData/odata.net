namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _polygonData_ЖⲤCOMMA_polygonDataↃTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._polygonData_ЖⲤCOMMA_polygonDataↃ>
    {
        private _polygonData_ЖⲤCOMMA_polygonDataↃTranscriber()
        {
        }
        
        public static _polygonData_ЖⲤCOMMA_polygonDataↃTranscriber Instance { get; } = new _polygonData_ЖⲤCOMMA_polygonDataↃTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._polygonData_ЖⲤCOMMA_polygonDataↃ value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._polygonDataTranscriber.Instance.Transcribe(value._polygonData_1, builder);
foreach (var _ⲤCOMMA_polygonDataↃ_1 in value._ⲤCOMMA_polygonDataↃ_1)
{
Inners._ⲤCOMMA_polygonDataↃTranscriber.Instance.Transcribe(_ⲤCOMMA_polygonDataↃ_1, builder);
}

        }
    }
    
}

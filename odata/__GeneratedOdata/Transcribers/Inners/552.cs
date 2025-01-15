namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _polygonData_ЖⲤCOMMA_polygonDataↃTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._polygonData_ЖⲤCOMMA_polygonDataↃ>
    {
        private _polygonData_ЖⲤCOMMA_polygonDataↃTranscriber()
        {
        }
        
        public static _polygonData_ЖⲤCOMMA_polygonDataↃTranscriber Instance { get; } = new _polygonData_ЖⲤCOMMA_polygonDataↃTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._polygonData_ЖⲤCOMMA_polygonDataↃ value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._polygonDataTranscriber.Instance.Transcribe(value._polygonData_1, builder);
foreach (var _ⲤCOMMA_polygonDataↃ_1 in value._ⲤCOMMA_polygonDataↃ_1)
{
Inners._ⲤCOMMA_polygonDataↃTranscriber.Instance.Transcribe(_ⲤCOMMA_polygonDataↃ_1, builder);
}

        }
    }
    
}

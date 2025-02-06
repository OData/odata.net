namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _multiPolygonLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._multiPolygonLiteral>
    {
        private _multiPolygonLiteralTranscriber()
        {
        }
        
        public static _multiPolygonLiteralTranscriber Instance { get; } = new _multiPolygonLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._multiPolygonLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Inners._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺTranscriber.Instance.Transcribe(value._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ_1, builder);
if (value._polygonData_ЖⲤCOMMA_polygonDataↃ_1 != null)
{
__GeneratedOdataV4.Trancsribers.Inners._polygonData_ЖⲤCOMMA_polygonDataↃTranscriber.Instance.Transcribe(value._polygonData_ЖⲤCOMMA_polygonDataↃ_1, builder);
}
__GeneratedOdataV4.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _multiPolygonLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._multiPolygonLiteral>
    {
        private _multiPolygonLiteralTranscriber()
        {
        }
        
        public static _multiPolygonLiteralTranscriber Instance { get; } = new _multiPolygonLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._multiPolygonLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺTranscriber.Instance.Transcribe(value._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ_1, builder);
if (value._polygonData_ЖⲤCOMMA_polygonDataↃ_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._polygonData_ЖⲤCOMMA_polygonDataↃTranscriber.Instance.Transcribe(value._polygonData_ЖⲤCOMMA_polygonDataↃ_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

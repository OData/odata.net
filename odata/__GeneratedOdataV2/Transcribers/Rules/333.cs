namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _fullPolygonLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._fullPolygonLiteral>
    {
        private _fullPolygonLiteralTranscriber()
        {
        }
        
        public static _fullPolygonLiteralTranscriber Instance { get; } = new _fullPolygonLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._fullPolygonLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._sridLiteralTranscriber.Instance.Transcribe(value._sridLiteral_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._polygonLiteralTranscriber.Instance.Transcribe(value._polygonLiteral_1, builder);

        }
    }
    
}

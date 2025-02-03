namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _geometryPolygonTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._geometryPolygon>
    {
        private _geometryPolygonTranscriber()
        {
        }
        
        public static _geometryPolygonTranscriber Instance { get; } = new _geometryPolygonTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._geometryPolygon value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._geometryPrefixTranscriber.Instance.Transcribe(value._geometryPrefix_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._fullPolygonLiteralTranscriber.Instance.Transcribe(value._fullPolygonLiteral_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

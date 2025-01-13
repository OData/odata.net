namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _geometryPolygonTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._geometryPolygon>
    {
        private _geometryPolygonTranscriber()
        {
        }
        
        public static _geometryPolygonTranscriber Instance { get; } = new _geometryPolygonTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._geometryPolygon value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._geometryPrefixTranscriber.Instance.Transcribe(value._geometryPrefix_1, builder);
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdata.Trancsribers.Rules._fullPolygonLiteralTranscriber.Instance.Transcribe(value._fullPolygonLiteral_1, builder);
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

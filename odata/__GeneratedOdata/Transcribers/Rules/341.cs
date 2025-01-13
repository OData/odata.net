namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _geometryMultiPolygonTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._geometryMultiPolygon>
    {
        private _geometryMultiPolygonTranscriber()
        {
        }
        
        public static _geometryMultiPolygonTranscriber Instance { get; } = new _geometryMultiPolygonTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._geometryMultiPolygon value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._geometryPrefixTranscriber.Instance.Transcribe(value._geometryPrefix_1, builder);
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdata.Trancsribers.Rules._fullMultiPolygonLiteralTranscriber.Instance.Transcribe(value._fullMultiPolygonLiteral_1, builder);
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

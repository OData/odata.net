namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _geographyMultiPolygonTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._geographyMultiPolygon>
    {
        private _geographyMultiPolygonTranscriber()
        {
        }
        
        public static _geographyMultiPolygonTranscriber Instance { get; } = new _geographyMultiPolygonTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._geographyMultiPolygon value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._geographyPrefixTranscriber.Instance.Transcribe(value._geographyPrefix_1, builder);
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdata.Trancsribers.Rules._fullMultiPolygonLiteralTranscriber.Instance.Transcribe(value._fullMultiPolygonLiteral_1, builder);
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}
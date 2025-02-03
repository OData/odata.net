namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _geographyPolygonTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._geographyPolygon>
    {
        private _geographyPolygonTranscriber()
        {
        }
        
        public static _geographyPolygonTranscriber Instance { get; } = new _geographyPolygonTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._geographyPolygon value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._geographyPrefixTranscriber.Instance.Transcribe(value._geographyPrefix_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._fullPolygonLiteralTranscriber.Instance.Transcribe(value._fullPolygonLiteral_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

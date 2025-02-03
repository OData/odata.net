namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _geometryCollectionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._geometryCollection>
    {
        private _geometryCollectionTranscriber()
        {
        }
        
        public static _geometryCollectionTranscriber Instance { get; } = new _geometryCollectionTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._geometryCollection value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._geometryPrefixTranscriber.Instance.Transcribe(value._geometryPrefix_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._fullCollectionLiteralTranscriber.Instance.Transcribe(value._fullCollectionLiteral_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

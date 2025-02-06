namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _fullCollectionLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._fullCollectionLiteral>
    {
        private _fullCollectionLiteralTranscriber()
        {
        }
        
        public static _fullCollectionLiteralTranscriber Instance { get; } = new _fullCollectionLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._fullCollectionLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._sridLiteralTranscriber.Instance.Transcribe(value._sridLiteral_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._collectionLiteralTranscriber.Instance.Transcribe(value._collectionLiteral_1, builder);

        }
    }
    
}

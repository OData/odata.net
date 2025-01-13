namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _fullCollectionLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._fullCollectionLiteral>
    {
        private _fullCollectionLiteralTranscriber()
        {
        }
        
        public static _fullCollectionLiteralTranscriber Instance { get; } = new _fullCollectionLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._fullCollectionLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._sridLiteralTranscriber.Instance.Transcribe(value._sridLiteral_1, builder);
__GeneratedOdata.Trancsribers.Rules._collectionLiteralTranscriber.Instance.Transcribe(value._collectionLiteral_1, builder);

        }
    }
    
}

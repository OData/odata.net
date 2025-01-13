namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _geographyCollectionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._geographyCollection>
    {
        private _geographyCollectionTranscriber()
        {
        }
        
        public static _geographyCollectionTranscriber Instance { get; } = new _geographyCollectionTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._geographyCollection value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._geographyPrefixTranscriber.Instance.Transcribe(value._geographyPrefix_1, builder);
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdata.Trancsribers.Rules._fullCollectionLiteralTranscriber.Instance.Transcribe(value._fullCollectionLiteral_1, builder);
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

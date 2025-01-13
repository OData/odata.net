namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _geographyMultiLineStringTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._geographyMultiLineString>
    {
        private _geographyMultiLineStringTranscriber()
        {
        }
        
        public static _geographyMultiLineStringTranscriber Instance { get; } = new _geographyMultiLineStringTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._geographyMultiLineString value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._geographyPrefixTranscriber.Instance.Transcribe(value._geographyPrefix_1, builder);
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdata.Trancsribers.Rules._fullMultiLineStringLiteralTranscriber.Instance.Transcribe(value._fullMultiLineStringLiteral_1, builder);
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

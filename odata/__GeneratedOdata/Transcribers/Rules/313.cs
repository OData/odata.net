namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _geographyLineStringTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._geographyLineString>
    {
        private _geographyLineStringTranscriber()
        {
        }
        
        public static _geographyLineStringTranscriber Instance { get; } = new _geographyLineStringTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._geographyLineString value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._geographyPrefixTranscriber.Instance.Transcribe(value._geographyPrefix_1, builder);
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdata.Trancsribers.Rules._fullLineStringLiteralTranscriber.Instance.Transcribe(value._fullLineStringLiteral_1, builder);
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

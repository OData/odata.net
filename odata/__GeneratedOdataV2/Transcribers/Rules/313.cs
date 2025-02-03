namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _geographyLineStringTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._geographyLineString>
    {
        private _geographyLineStringTranscriber()
        {
        }
        
        public static _geographyLineStringTranscriber Instance { get; } = new _geographyLineStringTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._geographyLineString value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._geographyPrefixTranscriber.Instance.Transcribe(value._geographyPrefix_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._fullLineStringLiteralTranscriber.Instance.Transcribe(value._fullLineStringLiteral_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

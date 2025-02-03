namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _geometryMultiLineStringTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._geometryMultiLineString>
    {
        private _geometryMultiLineStringTranscriber()
        {
        }
        
        public static _geometryMultiLineStringTranscriber Instance { get; } = new _geometryMultiLineStringTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._geometryMultiLineString value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._geometryPrefixTranscriber.Instance.Transcribe(value._geometryPrefix_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._fullMultiLineStringLiteralTranscriber.Instance.Transcribe(value._fullMultiLineStringLiteral_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

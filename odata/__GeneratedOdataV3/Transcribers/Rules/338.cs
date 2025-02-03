namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _geometryLineStringTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._geometryLineString>
    {
        private _geometryLineStringTranscriber()
        {
        }
        
        public static _geometryLineStringTranscriber Instance { get; } = new _geometryLineStringTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._geometryLineString value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._geometryPrefixTranscriber.Instance.Transcribe(value._geometryPrefix_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._fullLineStringLiteralTranscriber.Instance.Transcribe(value._fullLineStringLiteral_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

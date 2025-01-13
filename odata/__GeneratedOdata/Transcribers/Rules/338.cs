namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _geometryLineStringTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._geometryLineString>
    {
        private _geometryLineStringTranscriber()
        {
        }
        
        public static _geometryLineStringTranscriber Instance { get; } = new _geometryLineStringTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._geometryLineString value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._geometryPrefixTranscriber.Instance.Transcribe(value._geometryPrefix_1, builder);
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdata.Trancsribers.Rules._fullLineStringLiteralTranscriber.Instance.Transcribe(value._fullLineStringLiteral_1, builder);
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

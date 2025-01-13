namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _geometryMultiPointTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._geometryMultiPoint>
    {
        private _geometryMultiPointTranscriber()
        {
        }
        
        public static _geometryMultiPointTranscriber Instance { get; } = new _geometryMultiPointTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._geometryMultiPoint value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._geometryPrefixTranscriber.Instance.Transcribe(value._geometryPrefix_1, builder);
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdata.Trancsribers.Rules._fullMultiPointLiteralTranscriber.Instance.Transcribe(value._fullMultiPointLiteral_1, builder);
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

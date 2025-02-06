namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _geometryMultiPointTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._geometryMultiPoint>
    {
        private _geometryMultiPointTranscriber()
        {
        }
        
        public static _geometryMultiPointTranscriber Instance { get; } = new _geometryMultiPointTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._geometryMultiPoint value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._geometryPrefixTranscriber.Instance.Transcribe(value._geometryPrefix_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._fullMultiPointLiteralTranscriber.Instance.Transcribe(value._fullMultiPointLiteral_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

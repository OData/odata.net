namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _geographyMultiPointTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._geographyMultiPoint>
    {
        private _geographyMultiPointTranscriber()
        {
        }
        
        public static _geographyMultiPointTranscriber Instance { get; } = new _geographyMultiPointTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._geographyMultiPoint value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._geographyPrefixTranscriber.Instance.Transcribe(value._geographyPrefix_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._fullMultiPointLiteralTranscriber.Instance.Transcribe(value._fullMultiPointLiteral_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _geographyMultiPointTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._geographyMultiPoint>
    {
        private _geographyMultiPointTranscriber()
        {
        }
        
        public static _geographyMultiPointTranscriber Instance { get; } = new _geographyMultiPointTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._geographyMultiPoint value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._geographyPrefixTranscriber.Instance.Transcribe(value._geographyPrefix_1, builder);
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdata.Trancsribers.Rules._fullMultiPointLiteralTranscriber.Instance.Transcribe(value._fullMultiPointLiteral_1, builder);
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

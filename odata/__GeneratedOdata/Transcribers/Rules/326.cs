namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _geographyPointTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._geographyPoint>
    {
        private _geographyPointTranscriber()
        {
        }
        
        public static _geographyPointTranscriber Instance { get; } = new _geographyPointTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._geographyPoint value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._geographyPrefixTranscriber.Instance.Transcribe(value._geographyPrefix_1, builder);
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdata.Trancsribers.Rules._fullPointLiteralTranscriber.Instance.Transcribe(value._fullPointLiteral_1, builder);
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

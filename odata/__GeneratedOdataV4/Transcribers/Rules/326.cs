namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _geographyPointTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._geographyPoint>
    {
        private _geographyPointTranscriber()
        {
        }
        
        public static _geographyPointTranscriber Instance { get; } = new _geographyPointTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._geographyPoint value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._geographyPrefixTranscriber.Instance.Transcribe(value._geographyPrefix_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._fullPointLiteralTranscriber.Instance.Transcribe(value._fullPointLiteral_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _geometryPointTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._geometryPoint>
    {
        private _geometryPointTranscriber()
        {
        }
        
        public static _geometryPointTranscriber Instance { get; } = new _geometryPointTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._geometryPoint value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._geometryPrefixTranscriber.Instance.Transcribe(value._geometryPrefix_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._fullPointLiteralTranscriber.Instance.Transcribe(value._fullPointLiteral_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _geometryPointTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._geometryPoint>
    {
        private _geometryPointTranscriber()
        {
        }
        
        public static _geometryPointTranscriber Instance { get; } = new _geometryPointTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._geometryPoint value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._geometryPrefixTranscriber.Instance.Transcribe(value._geometryPrefix_1, builder);
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdata.Trancsribers.Rules._fullPointLiteralTranscriber.Instance.Transcribe(value._fullPointLiteral_1, builder);
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

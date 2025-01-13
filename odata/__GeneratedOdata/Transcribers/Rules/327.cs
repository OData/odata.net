namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _fullPointLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._fullPointLiteral>
    {
        private _fullPointLiteralTranscriber()
        {
        }
        
        public static _fullPointLiteralTranscriber Instance { get; } = new _fullPointLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._fullPointLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._sridLiteralTranscriber.Instance.Transcribe(value._sridLiteral_1, builder);
__GeneratedOdata.Trancsribers.Rules._pointLiteralTranscriber.Instance.Transcribe(value._pointLiteral_1, builder);

        }
    }
    
}

namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _fullPointLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._fullPointLiteral>
    {
        private _fullPointLiteralTranscriber()
        {
        }
        
        public static _fullPointLiteralTranscriber Instance { get; } = new _fullPointLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._fullPointLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._sridLiteralTranscriber.Instance.Transcribe(value._sridLiteral_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._pointLiteralTranscriber.Instance.Transcribe(value._pointLiteral_1, builder);

        }
    }
    
}

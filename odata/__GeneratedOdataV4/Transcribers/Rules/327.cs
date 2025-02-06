namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _fullPointLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._fullPointLiteral>
    {
        private _fullPointLiteralTranscriber()
        {
        }
        
        public static _fullPointLiteralTranscriber Instance { get; } = new _fullPointLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._fullPointLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._sridLiteralTranscriber.Instance.Transcribe(value._sridLiteral_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._pointLiteralTranscriber.Instance.Transcribe(value._pointLiteral_1, builder);

        }
    }
    
}

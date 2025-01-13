namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _fullMultiPointLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._fullMultiPointLiteral>
    {
        private _fullMultiPointLiteralTranscriber()
        {
        }
        
        public static _fullMultiPointLiteralTranscriber Instance { get; } = new _fullMultiPointLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._fullMultiPointLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._sridLiteralTranscriber.Instance.Transcribe(value._sridLiteral_1, builder);
__GeneratedOdata.Trancsribers.Rules._multiPointLiteralTranscriber.Instance.Transcribe(value._multiPointLiteral_1, builder);

        }
    }
    
}

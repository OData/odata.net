namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _fullMultiPointLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._fullMultiPointLiteral>
    {
        private _fullMultiPointLiteralTranscriber()
        {
        }
        
        public static _fullMultiPointLiteralTranscriber Instance { get; } = new _fullMultiPointLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._fullMultiPointLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._sridLiteralTranscriber.Instance.Transcribe(value._sridLiteral_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._multiPointLiteralTranscriber.Instance.Transcribe(value._multiPointLiteral_1, builder);

        }
    }
    
}

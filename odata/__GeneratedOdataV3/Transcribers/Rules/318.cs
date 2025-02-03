namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _fullMultiLineStringLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._fullMultiLineStringLiteral>
    {
        private _fullMultiLineStringLiteralTranscriber()
        {
        }
        
        public static _fullMultiLineStringLiteralTranscriber Instance { get; } = new _fullMultiLineStringLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._fullMultiLineStringLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._sridLiteralTranscriber.Instance.Transcribe(value._sridLiteral_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._multiLineStringLiteralTranscriber.Instance.Transcribe(value._multiLineStringLiteral_1, builder);

        }
    }
    
}

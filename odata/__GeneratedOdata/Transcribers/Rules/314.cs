namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _fullLineStringLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._fullLineStringLiteral>
    {
        private _fullLineStringLiteralTranscriber()
        {
        }
        
        public static _fullLineStringLiteralTranscriber Instance { get; } = new _fullLineStringLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._fullLineStringLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._sridLiteralTranscriber.Instance.Transcribe(value._sridLiteral_1, builder);
__GeneratedOdata.Trancsribers.Rules._lineStringLiteralTranscriber.Instance.Transcribe(value._lineStringLiteral_1, builder);

        }
    }
    
}

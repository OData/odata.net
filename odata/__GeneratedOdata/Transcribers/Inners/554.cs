namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _COMMA_ringLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._COMMA_ringLiteral>
    {
        private _COMMA_ringLiteralTranscriber()
        {
        }
        
        public static _COMMA_ringLiteralTranscriber Instance { get; } = new _COMMA_ringLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._COMMA_ringLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdata.Trancsribers.Rules._ringLiteralTranscriber.Instance.Transcribe(value._ringLiteral_1, builder);

        }
    }
    
}

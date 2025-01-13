namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _COMMA_positionLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._COMMA_positionLiteral>
    {
        private _COMMA_positionLiteralTranscriber()
        {
        }
        
        public static _COMMA_positionLiteralTranscriber Instance { get; } = new _COMMA_positionLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._COMMA_positionLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdata.Trancsribers.Rules._positionLiteralTranscriber.Instance.Transcribe(value._positionLiteral_1, builder);

        }
    }
    
}

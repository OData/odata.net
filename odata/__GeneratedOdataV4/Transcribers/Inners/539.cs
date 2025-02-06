namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _COMMA_positionLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._COMMA_positionLiteral>
    {
        private _COMMA_positionLiteralTranscriber()
        {
        }
        
        public static _COMMA_positionLiteralTranscriber Instance { get; } = new _COMMA_positionLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._COMMA_positionLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._positionLiteralTranscriber.Instance.Transcribe(value._positionLiteral_1, builder);

        }
    }
    
}

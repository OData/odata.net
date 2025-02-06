namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _COMMA_ringLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._COMMA_ringLiteral>
    {
        private _COMMA_ringLiteralTranscriber()
        {
        }
        
        public static _COMMA_ringLiteralTranscriber Instance { get; } = new _COMMA_ringLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._COMMA_ringLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._ringLiteralTranscriber.Instance.Transcribe(value._ringLiteral_1, builder);

        }
    }
    
}

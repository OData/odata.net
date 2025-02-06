namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _COMMA_geoLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._COMMA_geoLiteral>
    {
        private _COMMA_geoLiteralTranscriber()
        {
        }
        
        public static _COMMA_geoLiteralTranscriber Instance { get; } = new _COMMA_geoLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._COMMA_geoLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._geoLiteralTranscriber.Instance.Transcribe(value._geoLiteral_1, builder);

        }
    }
    
}

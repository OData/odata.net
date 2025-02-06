namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _fullMultiPolygonLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._fullMultiPolygonLiteral>
    {
        private _fullMultiPolygonLiteralTranscriber()
        {
        }
        
        public static _fullMultiPolygonLiteralTranscriber Instance { get; } = new _fullMultiPolygonLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._fullMultiPolygonLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._sridLiteralTranscriber.Instance.Transcribe(value._sridLiteral_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._multiPolygonLiteralTranscriber.Instance.Transcribe(value._multiPolygonLiteral_1, builder);

        }
    }
    
}

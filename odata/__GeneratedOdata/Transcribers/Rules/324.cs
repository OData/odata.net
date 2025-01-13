namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _fullMultiPolygonLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._fullMultiPolygonLiteral>
    {
        private _fullMultiPolygonLiteralTranscriber()
        {
        }
        
        public static _fullMultiPolygonLiteralTranscriber Instance { get; } = new _fullMultiPolygonLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._fullMultiPolygonLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._sridLiteralTranscriber.Instance.Transcribe(value._sridLiteral_1, builder);
__GeneratedOdata.Trancsribers.Rules._multiPolygonLiteralTranscriber.Instance.Transcribe(value._multiPolygonLiteral_1, builder);

        }
    }
    
}

namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _fullPolygonLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._fullPolygonLiteral>
    {
        private _fullPolygonLiteralTranscriber()
        {
        }
        
        public static _fullPolygonLiteralTranscriber Instance { get; } = new _fullPolygonLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._fullPolygonLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._sridLiteralTranscriber.Instance.Transcribe(value._sridLiteral_1, builder);
__GeneratedOdata.Trancsribers.Rules._polygonLiteralTranscriber.Instance.Transcribe(value._polygonLiteral_1, builder);

        }
    }
    
}

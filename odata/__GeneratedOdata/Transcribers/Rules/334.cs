namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _polygonLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._polygonLiteral>
    {
        private _polygonLiteralTranscriber()
        {
        }
        
        public static _polygonLiteralTranscriber Instance { get; } = new _polygonLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._polygonLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ʺx50x6Fx6Cx79x67x6Fx6EʺTranscriber.Instance.Transcribe(value._ʺx50x6Fx6Cx79x67x6Fx6Eʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._polygonDataTranscriber.Instance.Transcribe(value._polygonData_1, builder);

        }
    }
    
}
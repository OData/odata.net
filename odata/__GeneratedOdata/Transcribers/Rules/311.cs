namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _collectionLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._collectionLiteral>
    {
        private _collectionLiteralTranscriber()
        {
        }
        
        public static _collectionLiteralTranscriber Instance { get; } = new _collectionLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._collectionLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28ʺTranscriber.Instance.Transcribe(value._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._geoLiteralTranscriber.Instance.Transcribe(value._geoLiteral_1, builder);
foreach (var _ⲤCOMMA_geoLiteralↃ_1 in value._ⲤCOMMA_geoLiteralↃ_1)
{
Inners._ⲤCOMMA_geoLiteralↃTranscriber.Instance.Transcribe(_ⲤCOMMA_geoLiteralↃ_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _polygonDataTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._polygonData>
    {
        private _polygonDataTranscriber()
        {
        }
        
        public static _polygonDataTranscriber Instance { get; } = new _polygonDataTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._polygonData value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._ringLiteralTranscriber.Instance.Transcribe(value._ringLiteral_1, builder);
foreach (var _ⲤCOMMA_ringLiteralↃ_1 in value._ⲤCOMMA_ringLiteralↃ_1)
{
Inners._ⲤCOMMA_ringLiteralↃTranscriber.Instance.Transcribe(_ⲤCOMMA_ringLiteralↃ_1, builder);
}
__GeneratedOdataV3.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

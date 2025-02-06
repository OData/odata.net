namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _lineStringDataTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._lineStringData>
    {
        private _lineStringDataTranscriber()
        {
        }
        
        public static _lineStringDataTranscriber Instance { get; } = new _lineStringDataTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._lineStringData value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._positionLiteralTranscriber.Instance.Transcribe(value._positionLiteral_1, builder);
foreach (var _ⲤCOMMA_positionLiteralↃ_1 in value._ⲤCOMMA_positionLiteralↃ_1)
{
__GeneratedOdataV4.Trancsribers.Inners._ⲤCOMMA_positionLiteralↃTranscriber.Instance.Transcribe(_ⲤCOMMA_positionLiteralↃ_1, builder);
}
__GeneratedOdataV4.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _ringLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._ringLiteral>
    {
        private _ringLiteralTranscriber()
        {
        }
        
        public static _ringLiteralTranscriber Instance { get; } = new _ringLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._ringLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdata.Trancsribers.Rules._positionLiteralTranscriber.Instance.Transcribe(value._positionLiteral_1, builder);
foreach (var _ⲤCOMMA_positionLiteralↃ_1 in value._ⲤCOMMA_positionLiteralↃ_1)
{
Inners._ⲤCOMMA_positionLiteralↃTranscriber.Instance.Transcribe(_ⲤCOMMA_positionLiteralↃ_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

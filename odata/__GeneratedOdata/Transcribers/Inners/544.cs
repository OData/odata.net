namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _lineStringData_ЖⲤCOMMA_lineStringDataↃTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._lineStringData_ЖⲤCOMMA_lineStringDataↃ>
    {
        private _lineStringData_ЖⲤCOMMA_lineStringDataↃTranscriber()
        {
        }
        
        public static _lineStringData_ЖⲤCOMMA_lineStringDataↃTranscriber Instance { get; } = new _lineStringData_ЖⲤCOMMA_lineStringDataↃTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._lineStringData_ЖⲤCOMMA_lineStringDataↃ value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._lineStringDataTranscriber.Instance.Transcribe(value._lineStringData_1, builder);
foreach (var _ⲤCOMMA_lineStringDataↃ_1 in value._ⲤCOMMA_lineStringDataↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._ⲤCOMMA_lineStringDataↃTranscriber.Instance.Transcribe(_ⲤCOMMA_lineStringDataↃ_1, builder);
}

        }
    }
    
}
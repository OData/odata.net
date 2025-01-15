namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _expandTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._expand>
    {
        private _expandTranscriber()
        {
        }
        
        public static _expandTranscriber Instance { get; } = new _expandTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._expand value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._Ⲥʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺↃTranscriber.Instance.Transcribe(value._Ⲥʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺↃ_1, builder);
__GeneratedOdata.Trancsribers.Rules._EQTranscriber.Instance.Transcribe(value._EQ_1, builder);
__GeneratedOdata.Trancsribers.Rules._expandItemTranscriber.Instance.Transcribe(value._expandItem_1, builder);
foreach (var _ⲤCOMMA_expandItemↃ_1 in value._ⲤCOMMA_expandItemↃ_1)
{
Inners._ⲤCOMMA_expandItemↃTranscriber.Instance.Transcribe(_ⲤCOMMA_expandItemↃ_1, builder);
}

        }
    }
    
}

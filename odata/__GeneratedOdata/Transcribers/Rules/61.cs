namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _computeTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._compute>
    {
        private _computeTranscriber()
        {
        }
        
        public static _computeTranscriber Instance { get; } = new _computeTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._compute value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._Ⲥʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺↃTranscriber.Instance.Transcribe(value._Ⲥʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺↃ_1, builder);
__GeneratedOdata.Trancsribers.Rules._EQTranscriber.Instance.Transcribe(value._EQ_1, builder);
__GeneratedOdata.Trancsribers.Rules._computeItemTranscriber.Instance.Transcribe(value._computeItem_1, builder);
foreach (var _ⲤCOMMA_computeItemↃ_1 in value._ⲤCOMMA_computeItemↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._ⲤCOMMA_computeItemↃTranscriber.Instance.Transcribe(_ⲤCOMMA_computeItemↃ_1, builder);
}

        }
    }
    
}

namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _crossjoinTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._crossjoin>
    {
        private _crossjoinTranscriber()
        {
        }
        
        public static _crossjoinTranscriber Instance { get; } = new _crossjoinTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._crossjoin value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ʺx24x63x72x6Fx73x73x6Ax6Fx69x6EʺTranscriber.Instance.Transcribe(value._ʺx24x63x72x6Fx73x73x6Ax6Fx69x6Eʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdata.Trancsribers.Rules._entitySetNameTranscriber.Instance.Transcribe(value._entitySetName_1, builder);
foreach (var _ⲤCOMMA_entitySetNameↃ_1 in value._ⲤCOMMA_entitySetNameↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._ⲤCOMMA_entitySetNameↃTranscriber.Instance.Transcribe(_ⲤCOMMA_entitySetNameↃ_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

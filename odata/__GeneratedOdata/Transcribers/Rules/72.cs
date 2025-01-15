namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _orderbyTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._orderby>
    {
        private _orderbyTranscriber()
        {
        }
        
        public static _orderbyTranscriber Instance { get; } = new _orderbyTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._orderby value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._Ⲥʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺↃTranscriber.Instance.Transcribe(value._Ⲥʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺↃ_1, builder);
__GeneratedOdata.Trancsribers.Rules._EQTranscriber.Instance.Transcribe(value._EQ_1, builder);
__GeneratedOdata.Trancsribers.Rules._orderbyItemTranscriber.Instance.Transcribe(value._orderbyItem_1, builder);
foreach (var _ⲤCOMMA_orderbyItemↃ_1 in value._ⲤCOMMA_orderbyItemↃ_1)
{
Inners._ⲤCOMMA_orderbyItemↃTranscriber.Instance.Transcribe(_ⲤCOMMA_orderbyItemↃ_1, builder);
}

        }
    }
    
}

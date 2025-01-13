namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _orderbyItemTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._orderbyItem>
    {
        private _orderbyItemTranscriber()
        {
        }
        
        public static _orderbyItemTranscriber Instance { get; } = new _orderbyItemTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._orderbyItem value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);
if (value._RWS_Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃ_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._RWS_Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃTranscriber.Instance.Transcribe(value._RWS_Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃ_1, builder);
}

        }
    }
    
}

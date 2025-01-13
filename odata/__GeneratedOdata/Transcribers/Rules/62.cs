namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _computeItemTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._computeItem>
    {
        private _computeItemTranscriber()
        {
        }
        
        public static _computeItemTranscriber Instance { get; } = new _computeItemTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._computeItem value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);
__GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx61x73ʺTranscriber.Instance.Transcribe(value._ʺx61x73ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdata.Trancsribers.Rules._computedPropertyTranscriber.Instance.Transcribe(value._computedProperty_1, builder);

        }
    }
    
}

namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _subExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._subExpr>
    {
        private _subExprTranscriber()
        {
        }
        
        public static _subExprTranscriber Instance { get; } = new _subExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._subExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx73x75x62ʺTranscriber.Instance.Transcribe(value._ʺx73x75x62ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdata.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);

        }
    }
    
}

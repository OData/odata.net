namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _inExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._inExpr>
    {
        private _inExprTranscriber()
        {
        }
        
        public static _inExprTranscriber Instance { get; } = new _inExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._inExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx69x6EʺTranscriber.Instance.Transcribe(value._ʺx69x6Eʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdata.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);

        }
    }
    
}

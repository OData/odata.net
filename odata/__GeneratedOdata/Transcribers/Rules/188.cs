namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _divExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._divExpr>
    {
        private _divExprTranscriber()
        {
        }
        
        public static _divExprTranscriber Instance { get; } = new _divExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._divExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx64x69x76ʺTranscriber.Instance.Transcribe(value._ʺx64x69x76ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdata.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);

        }
    }
    
}

namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _divbyExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._divbyExpr>
    {
        private _divbyExprTranscriber()
        {
        }
        
        public static _divbyExprTranscriber Instance { get; } = new _divbyExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._divbyExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx64x69x76x62x79ʺTranscriber.Instance.Transcribe(value._ʺx64x69x76x62x79ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdata.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);

        }
    }
    
}

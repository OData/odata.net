namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _inExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._inExpr>
    {
        private _inExprTranscriber()
        {
        }
        
        public static _inExprTranscriber Instance { get; } = new _inExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._inExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdataV4.Trancsribers.Inners._ʺx69x6EʺTranscriber.Instance.Transcribe(value._ʺx69x6Eʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);

        }
    }
    
}

namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _leExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._leExpr>
    {
        private _leExprTranscriber()
        {
        }
        
        public static _leExprTranscriber Instance { get; } = new _leExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._leExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx6Cx65ʺTranscriber.Instance.Transcribe(value._ʺx6Cx65ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdata.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);

        }
    }
    
}

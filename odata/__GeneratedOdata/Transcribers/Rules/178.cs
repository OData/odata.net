namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _neExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._neExpr>
    {
        private _neExprTranscriber()
        {
        }
        
        public static _neExprTranscriber Instance { get; } = new _neExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._neExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx6Ex65ʺTranscriber.Instance.Transcribe(value._ʺx6Ex65ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdata.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);

        }
    }
    
}
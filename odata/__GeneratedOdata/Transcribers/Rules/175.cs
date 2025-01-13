namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _andExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._andExpr>
    {
        private _andExprTranscriber()
        {
        }
        
        public static _andExprTranscriber Instance { get; } = new _andExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._andExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx61x6Ex64ʺTranscriber.Instance.Transcribe(value._ʺx61x6Ex64ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdata.Trancsribers.Rules._boolCommonExprTranscriber.Instance.Transcribe(value._boolCommonExpr_1, builder);

        }
    }
    
}

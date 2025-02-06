namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _andExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._andExpr>
    {
        private _andExprTranscriber()
        {
        }
        
        public static _andExprTranscriber Instance { get; } = new _andExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._andExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdataV4.Trancsribers.Inners._ʺx61x6Ex64ʺTranscriber.Instance.Transcribe(value._ʺx61x6Ex64ʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._boolCommonExprTranscriber.Instance.Transcribe(value._boolCommonExpr_1, builder);

        }
    }
    
}

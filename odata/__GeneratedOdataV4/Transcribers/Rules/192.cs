namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _notExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._notExpr>
    {
        private _notExprTranscriber()
        {
        }
        
        public static _notExprTranscriber Instance { get; } = new _notExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._notExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Inners._ʺx6Ex6Fx74ʺTranscriber.Instance.Transcribe(value._ʺx6Ex6Fx74ʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._boolCommonExprTranscriber.Instance.Transcribe(value._boolCommonExpr_1, builder);

        }
    }
    
}

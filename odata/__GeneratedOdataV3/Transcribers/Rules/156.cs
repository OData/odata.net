namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _secondMethodCallExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._secondMethodCallExpr>
    {
        private _secondMethodCallExprTranscriber()
        {
        }
        
        public static _secondMethodCallExprTranscriber Instance { get; } = new _secondMethodCallExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._secondMethodCallExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Inners._ʺx73x65x63x6Fx6Ex64ʺTranscriber.Instance.Transcribe(value._ʺx73x65x63x6Fx6Ex64ʺ_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);
__GeneratedOdataV3.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

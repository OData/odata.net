namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _roundMethodCallExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._roundMethodCallExpr>
    {
        private _roundMethodCallExprTranscriber()
        {
        }
        
        public static _roundMethodCallExprTranscriber Instance { get; } = new _roundMethodCallExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._roundMethodCallExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Inners._ʺx72x6Fx75x6Ex64ʺTranscriber.Instance.Transcribe(value._ʺx72x6Fx75x6Ex64ʺ_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);
__GeneratedOdataV3.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

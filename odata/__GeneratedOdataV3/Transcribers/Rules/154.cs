namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _hourMethodCallExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._hourMethodCallExpr>
    {
        private _hourMethodCallExprTranscriber()
        {
        }
        
        public static _hourMethodCallExprTranscriber Instance { get; } = new _hourMethodCallExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._hourMethodCallExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Inners._ʺx68x6Fx75x72ʺTranscriber.Instance.Transcribe(value._ʺx68x6Fx75x72ʺ_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);
__GeneratedOdataV3.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

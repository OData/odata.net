namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _dayMethodCallExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._dayMethodCallExpr>
    {
        private _dayMethodCallExprTranscriber()
        {
        }
        
        public static _dayMethodCallExprTranscriber Instance { get; } = new _dayMethodCallExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._dayMethodCallExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Inners._ʺx64x61x79ʺTranscriber.Instance.Transcribe(value._ʺx64x61x79ʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

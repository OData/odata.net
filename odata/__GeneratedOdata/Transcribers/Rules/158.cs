namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _totalsecondsMethodCallExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._totalsecondsMethodCallExpr>
    {
        private _totalsecondsMethodCallExprTranscriber()
        {
        }
        
        public static _totalsecondsMethodCallExprTranscriber Instance { get; } = new _totalsecondsMethodCallExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._totalsecondsMethodCallExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ʺx74x6Fx74x61x6Cx73x65x63x6Fx6Ex64x73ʺTranscriber.Instance.Transcribe(value._ʺx74x6Fx74x61x6Cx73x65x63x6Fx6Ex64x73ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdata.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdata.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);
__GeneratedOdata.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);
__GeneratedOdata.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}
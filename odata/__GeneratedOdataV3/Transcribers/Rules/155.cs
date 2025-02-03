namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _minuteMethodCallExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._minuteMethodCallExpr>
    {
        private _minuteMethodCallExprTranscriber()
        {
        }
        
        public static _minuteMethodCallExprTranscriber Instance { get; } = new _minuteMethodCallExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._minuteMethodCallExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Inners._ʺx6Dx69x6Ex75x74x65ʺTranscriber.Instance.Transcribe(value._ʺx6Dx69x6Ex75x74x65ʺ_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);
__GeneratedOdataV3.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

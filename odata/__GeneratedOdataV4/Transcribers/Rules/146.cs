namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _startsWithMethodCallExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._startsWithMethodCallExpr>
    {
        private _startsWithMethodCallExprTranscriber()
        {
        }
        
        public static _startsWithMethodCallExprTranscriber Instance { get; } = new _startsWithMethodCallExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._startsWithMethodCallExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Inners._ʺx73x74x61x72x74x73x77x69x74x68ʺTranscriber.Instance.Transcribe(value._ʺx73x74x61x72x74x73x77x69x74x68ʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_3, builder);
__GeneratedOdataV4.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_4, builder);
__GeneratedOdataV4.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

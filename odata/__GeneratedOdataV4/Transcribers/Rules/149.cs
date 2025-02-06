namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _toUpperMethodCallExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._toUpperMethodCallExpr>
    {
        private _toUpperMethodCallExprTranscriber()
        {
        }
        
        public static _toUpperMethodCallExprTranscriber Instance { get; } = new _toUpperMethodCallExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._toUpperMethodCallExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Inners._ʺx74x6Fx75x70x70x65x72ʺTranscriber.Instance.Transcribe(value._ʺx74x6Fx75x70x70x65x72ʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

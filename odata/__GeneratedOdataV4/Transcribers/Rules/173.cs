namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _parenExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._parenExpr>
    {
        private _parenExprTranscriber()
        {
        }
        
        public static _parenExprTranscriber Instance { get; } = new _parenExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._parenExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

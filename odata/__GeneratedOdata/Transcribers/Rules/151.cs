namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _yearMethodCallExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._yearMethodCallExpr>
    {
        private _yearMethodCallExprTranscriber()
        {
        }
        
        public static _yearMethodCallExprTranscriber Instance { get; } = new _yearMethodCallExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._yearMethodCallExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ʺx79x65x61x72ʺTranscriber.Instance.Transcribe(value._ʺx79x65x61x72ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdata.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdata.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);
__GeneratedOdata.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);
__GeneratedOdata.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}
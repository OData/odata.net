namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _monthMethodCallExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._monthMethodCallExpr>
    {
        private _monthMethodCallExprTranscriber()
        {
        }
        
        public static _monthMethodCallExprTranscriber Instance { get; } = new _monthMethodCallExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._monthMethodCallExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ʺx6Dx6Fx6Ex74x68ʺTranscriber.Instance.Transcribe(value._ʺx6Dx6Fx6Ex74x68ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdata.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdata.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);
__GeneratedOdata.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);
__GeneratedOdata.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

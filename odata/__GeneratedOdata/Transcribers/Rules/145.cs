namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _lengthMethodCallExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._lengthMethodCallExpr>
    {
        private _lengthMethodCallExprTranscriber()
        {
        }
        
        public static _lengthMethodCallExprTranscriber Instance { get; } = new _lengthMethodCallExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._lengthMethodCallExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ʺx6Cx65x6Ex67x74x68ʺTranscriber.Instance.Transcribe(value._ʺx6Cx65x6Ex67x74x68ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdata.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdata.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);
__GeneratedOdata.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);
__GeneratedOdata.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

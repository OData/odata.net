namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _isofExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._isofExpr>
    {
        private _isofExprTranscriber()
        {
        }
        
        public static _isofExprTranscriber Instance { get; } = new _isofExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._isofExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Inners._ʺx69x73x6Fx66ʺTranscriber.Instance.Transcribe(value._ʺx69x73x6Fx66ʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
if (value._commonExpr_BWS_COMMA_BWS_1 != null)
{
__GeneratedOdataV4.Trancsribers.Inners._commonExpr_BWS_COMMA_BWSTranscriber.Instance.Transcribe(value._commonExpr_BWS_COMMA_BWS_1, builder);
}
__GeneratedOdataV4.Trancsribers.Rules._qualifiedTypeNameTranscriber.Instance.Transcribe(value._qualifiedTypeName_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

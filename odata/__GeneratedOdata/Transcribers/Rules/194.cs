namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _castExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._castExpr>
    {
        private _castExprTranscriber()
        {
        }
        
        public static _castExprTranscriber Instance { get; } = new _castExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._castExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ʺx63x61x73x74ʺTranscriber.Instance.Transcribe(value._ʺx63x61x73x74ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdata.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
if (value._commonExpr_BWS_COMMA_BWS_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._commonExpr_BWS_COMMA_BWSTranscriber.Instance.Transcribe(value._commonExpr_BWS_COMMA_BWS_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._qualifiedTypeNameTranscriber.Instance.Transcribe(value._qualifiedTypeName_1, builder);
__GeneratedOdata.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);
__GeneratedOdata.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

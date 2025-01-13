namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _memberExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._memberExpr>
    {
        private _memberExprTranscriber()
        {
        }
        
        public static _memberExprTranscriber Instance { get; } = new _memberExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._memberExpr value, System.Text.StringBuilder builder)
        {
            if (value._qualifiedEntityTypeName_ʺx2Fʺ_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._qualifiedEntityTypeName_ʺx2FʺTranscriber.Instance.Transcribe(value._qualifiedEntityTypeName_ʺx2Fʺ_1, builder);
}
__GeneratedOdata.Trancsribers.Inners._ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃTranscriber.Instance.Transcribe(value._ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ_1, builder);

        }
    }
    
}

namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _complexColPathExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._complexColPathExpr>
    {
        private _complexColPathExprTranscriber()
        {
        }
        
        public static _complexColPathExprTranscriber Instance { get; } = new _complexColPathExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._complexColPathExpr value, System.Text.StringBuilder builder)
        {
            if (value._ʺx2Fʺ_qualifiedComplexTypeName_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameTranscriber.Instance.Transcribe(value._ʺx2Fʺ_qualifiedComplexTypeName_1, builder);
}
if (value._collectionPathExpr_1 != null)
{
__GeneratedOdata.Trancsribers.Rules._collectionPathExprTranscriber.Instance.Transcribe(value._collectionPathExpr_1, builder);
}

        }
    }
    
}
namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _collectionNavigationExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._collectionNavigationExpr>
    {
        private _collectionNavigationExprTranscriber()
        {
        }
        
        public static _collectionNavigationExprTranscriber Instance { get; } = new _collectionNavigationExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._collectionNavigationExpr value, System.Text.StringBuilder builder)
        {
            if (value._ʺx2Fʺ_qualifiedEntityTypeName_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameTranscriber.Instance.Transcribe(value._ʺx2Fʺ_qualifiedEntityTypeName_1, builder);
}
if (value._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExprTranscriber.Instance.Transcribe(value._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr_1, builder);
}

        }
    }
    
}

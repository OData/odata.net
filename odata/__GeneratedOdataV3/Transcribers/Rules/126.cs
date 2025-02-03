namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _collectionNavigationExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._collectionNavigationExpr>
    {
        private _collectionNavigationExprTranscriber()
        {
        }
        
        public static _collectionNavigationExprTranscriber Instance { get; } = new _collectionNavigationExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._collectionNavigationExpr value, System.Text.StringBuilder builder)
        {
            if (value._ʺx2Fʺ_qualifiedEntityTypeName_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameTranscriber.Instance.Transcribe(value._ʺx2Fʺ_qualifiedEntityTypeName_1, builder);
}
if (value._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExprTranscriber.Instance.Transcribe(value._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr_1, builder);
}

        }
    }
    
}

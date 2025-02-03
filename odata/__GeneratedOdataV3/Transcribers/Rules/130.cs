namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _complexPathExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._complexPathExpr>
    {
        private _complexPathExprTranscriber()
        {
        }
        
        public static _complexPathExprTranscriber Instance { get; } = new _complexPathExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._complexPathExpr value, System.Text.StringBuilder builder)
        {
            if (value._ʺx2Fʺ_qualifiedComplexTypeName_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameTranscriber.Instance.Transcribe(value._ʺx2Fʺ_qualifiedComplexTypeName_1, builder);
}
if (value._ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExprTranscriber.Instance.Transcribe(value._ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr_1, builder);
}

        }
    }
    
}

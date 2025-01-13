namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _annotationExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._annotationExpr>
    {
        private _annotationExprTranscriber()
        {
        }
        
        public static _annotationExprTranscriber Instance { get; } = new _annotationExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._annotationExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._annotationTranscriber.Instance.Transcribe(value._annotation_1, builder);
if (value._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExprTranscriber.Instance.Transcribe(value._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr_1, builder);
}

        }
    }
    
}

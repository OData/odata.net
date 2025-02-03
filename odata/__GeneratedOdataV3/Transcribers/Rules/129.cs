namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _collectionPathExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._collectionPathExpr>
    {
        private _collectionPathExprTranscriber()
        {
        }
        
        public static _collectionPathExprTranscriber Instance { get; } = new _collectionPathExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._collectionPathExpr value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Rules._collectionPathExpr.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._collectionPathExpr._count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._countTranscriber.Instance.Transcribe(node._count_1, context);
if (node._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSETranscriber.Instance.Transcribe(node._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_boundFunctionExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(node._ʺx2Fʺ_1, context);
__GeneratedOdataV3.Trancsribers.Rules._boundFunctionExprTranscriber.Instance.Transcribe(node._boundFunctionExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_annotationExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(node._ʺx2Fʺ_1, context);
__GeneratedOdataV3.Trancsribers.Rules._annotationExprTranscriber.Instance.Transcribe(node._annotationExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_anyExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(node._ʺx2Fʺ_1, context);
__GeneratedOdataV3.Trancsribers.Rules._anyExprTranscriber.Instance.Transcribe(node._anyExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_allExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(node._ʺx2Fʺ_1, context);
__GeneratedOdataV3.Trancsribers.Rules._allExprTranscriber.Instance.Transcribe(node._allExpr_1, context);

return default;
            }
        }
    }
    
}

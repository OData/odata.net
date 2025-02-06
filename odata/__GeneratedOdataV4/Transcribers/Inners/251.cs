namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _qualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty>
    {
        private _qualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyTranscriber()
        {
        }
        
        public static _qualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyTranscriber Instance { get; } = new _qualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._qualifiedActionName node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._qualifiedActionNameTranscriber.Instance.Transcribe(node._qualifiedActionName_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._qualifiedFunctionName node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._qualifiedFunctionNameTranscriber.Instance.Transcribe(node._qualifiedFunctionName_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._selectListProperty node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._selectListPropertyTranscriber.Instance.Transcribe(node._selectListProperty_1, context);

return default;
            }
        }
    }
    
}

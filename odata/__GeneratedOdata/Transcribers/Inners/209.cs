namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName>
    {
        private _selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameTranscriber()
        {
        }
        
        public static _selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameTranscriber Instance { get; } = new _selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._selectProperty node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._selectPropertyTranscriber.Instance.Transcribe(node._selectProperty_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._qualifiedActionName node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._qualifiedActionNameTranscriber.Instance.Transcribe(node._qualifiedActionName_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._qualifiedFunctionName node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._qualifiedFunctionNameTranscriber.Instance.Transcribe(node._qualifiedFunctionName_1, context);

return default;
            }
        }
    }
    
}
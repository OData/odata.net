namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _qualifiedEntityTypeNameⳆqualifiedComplexTypeNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName>
    {
        private _qualifiedEntityTypeNameⳆqualifiedComplexTypeNameTranscriber()
        {
        }
        
        public static _qualifiedEntityTypeNameⳆqualifiedComplexTypeNameTranscriber Instance { get; } = new _qualifiedEntityTypeNameⳆqualifiedComplexTypeNameTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName._qualifiedEntityTypeName node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._qualifiedEntityTypeNameTranscriber.Instance.Transcribe(node._qualifiedEntityTypeName_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName._qualifiedComplexTypeName node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._qualifiedComplexTypeNameTranscriber.Instance.Transcribe(node._qualifiedComplexTypeName_1, context);

return default;
            }
        }
    }
    
}
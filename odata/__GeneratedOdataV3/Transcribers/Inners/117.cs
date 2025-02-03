namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _qualifiedEntityTypeNameⳆqualifiedComplexTypeNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName>
    {
        private _qualifiedEntityTypeNameⳆqualifiedComplexTypeNameTranscriber()
        {
        }
        
        public static _qualifiedEntityTypeNameⳆqualifiedComplexTypeNameTranscriber Instance { get; } = new _qualifiedEntityTypeNameⳆqualifiedComplexTypeNameTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName._qualifiedEntityTypeName node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._qualifiedEntityTypeNameTranscriber.Instance.Transcribe(node._qualifiedEntityTypeName_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName._qualifiedComplexTypeName node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._qualifiedComplexTypeNameTranscriber.Instance.Transcribe(node._qualifiedComplexTypeName_1, context);

return default;
            }
        }
    }
    
}

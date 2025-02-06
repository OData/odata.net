namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _singleQualifiedTypeNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._singleQualifiedTypeName>
    {
        private _singleQualifiedTypeNameTranscriber()
        {
        }
        
        public static _singleQualifiedTypeNameTranscriber Instance { get; } = new _singleQualifiedTypeNameTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._singleQualifiedTypeName value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Rules._singleQualifiedTypeName.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._singleQualifiedTypeName._qualifiedEntityTypeName node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._qualifiedEntityTypeNameTranscriber.Instance.Transcribe(node._qualifiedEntityTypeName_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._singleQualifiedTypeName._qualifiedComplexTypeName node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._qualifiedComplexTypeNameTranscriber.Instance.Transcribe(node._qualifiedComplexTypeName_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._singleQualifiedTypeName._qualifiedTypeDefinitionName node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._qualifiedTypeDefinitionNameTranscriber.Instance.Transcribe(node._qualifiedTypeDefinitionName_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._singleQualifiedTypeName._qualifiedEnumTypeName node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._qualifiedEnumTypeNameTranscriber.Instance.Transcribe(node._qualifiedEnumTypeName_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._singleQualifiedTypeName._primitiveTypeName node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._primitiveTypeNameTranscriber.Instance.Transcribe(node._primitiveTypeName_1, context);

return default;
            }
        }
    }
    
}

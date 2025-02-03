namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _primitiveKeyPropertyⳆkeyPropertyAliasTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._primitiveKeyPropertyⳆkeyPropertyAlias>
    {
        private _primitiveKeyPropertyⳆkeyPropertyAliasTranscriber()
        {
        }
        
        public static _primitiveKeyPropertyⳆkeyPropertyAliasTranscriber Instance { get; } = new _primitiveKeyPropertyⳆkeyPropertyAliasTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._primitiveKeyPropertyⳆkeyPropertyAlias value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Inners._primitiveKeyPropertyⳆkeyPropertyAlias.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._primitiveKeyPropertyⳆkeyPropertyAlias._primitiveKeyProperty node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._primitiveKeyPropertyTranscriber.Instance.Transcribe(node._primitiveKeyProperty_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._primitiveKeyPropertyⳆkeyPropertyAlias._keyPropertyAlias node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._keyPropertyAliasTranscriber.Instance.Transcribe(node._keyPropertyAlias_1, context);

return default;
            }
        }
    }
    
}

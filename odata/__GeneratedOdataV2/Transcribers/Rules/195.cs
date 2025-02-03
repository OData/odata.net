namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _arrayOrObjectTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._arrayOrObject>
    {
        private _arrayOrObjectTranscriber()
        {
        }
        
        public static _arrayOrObjectTranscriber Instance { get; } = new _arrayOrObjectTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._arrayOrObject value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV2.CstNodes.Rules._arrayOrObject.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._arrayOrObject._complexColInUri node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._complexColInUriTranscriber.Instance.Transcribe(node._complexColInUri_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._arrayOrObject._complexInUri node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._complexInUriTranscriber.Instance.Transcribe(node._complexInUri_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._arrayOrObject._rootExprCol node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._rootExprColTranscriber.Instance.Transcribe(node._rootExprCol_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._arrayOrObject._primitiveColInUri node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._primitiveColInUriTranscriber.Instance.Transcribe(node._primitiveColInUri_1, context);

return default;
            }
        }
    }
    
}

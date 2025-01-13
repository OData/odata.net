namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _identifierLeadingCharacterTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._identifierLeadingCharacter>
    {
        private _identifierLeadingCharacterTranscriber()
        {
        }
        
        public static _identifierLeadingCharacterTranscriber Instance { get; } = new _identifierLeadingCharacterTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._identifierLeadingCharacter value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._identifierLeadingCharacter.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._identifierLeadingCharacter._ALPHA node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._ALPHATranscriber.Instance.Transcribe(node._ALPHA_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._identifierLeadingCharacter._ʺx5Fʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx5FʺTranscriber.Instance.Transcribe(node._ʺx5Fʺ_1, context);

return default;
            }
        }
    }
    
}

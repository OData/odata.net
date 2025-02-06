namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _identifierCharacterTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._identifierCharacter>
    {
        private _identifierCharacterTranscriber()
        {
        }
        
        public static _identifierCharacterTranscriber Instance { get; } = new _identifierCharacterTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._identifierCharacter value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Rules._identifierCharacter.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._ALPHATranscriber.Instance.Transcribe(node._ALPHA_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ʺx5Fʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx5FʺTranscriber.Instance.Transcribe(node._ʺx5Fʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._identifierCharacter._DIGIT node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(node._DIGIT_1, context);

return default;
            }
        }
    }
    
}

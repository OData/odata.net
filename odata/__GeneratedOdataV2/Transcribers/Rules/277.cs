namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _base64charTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._base64char>
    {
        private _base64charTranscriber()
        {
        }
        
        public static _base64charTranscriber Instance { get; } = new _base64charTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._base64char value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV2.CstNodes.Rules._base64char.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._base64char._ALPHA node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._ALPHATranscriber.Instance.Transcribe(node._ALPHA_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._base64char._DIGIT node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(node._DIGIT_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._base64char._ʺx2Dʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx2DʺTranscriber.Instance.Transcribe(node._ʺx2Dʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._base64char._ʺx5Fʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx5FʺTranscriber.Instance.Transcribe(node._ʺx5Fʺ_1, context);

return default;
            }
        }
    }
    
}

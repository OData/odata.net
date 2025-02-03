namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _unreservedTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._unreserved>
    {
        private _unreservedTranscriber()
        {
        }
        
        public static _unreservedTranscriber Instance { get; } = new _unreservedTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._unreserved value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Rules._unreserved.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._unreserved._ALPHA node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._ALPHATranscriber.Instance.Transcribe(node._ALPHA_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._unreserved._DIGIT node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(node._DIGIT_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._unreserved._ʺx2Dʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx2DʺTranscriber.Instance.Transcribe(node._ʺx2Dʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._unreserved._ʺx2Eʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(node._ʺx2Eʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._unreserved._ʺx5Fʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx5FʺTranscriber.Instance.Transcribe(node._ʺx5Fʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._unreserved._ʺx7Eʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx7EʺTranscriber.Instance.Transcribe(node._ʺx7Eʺ_1, context);

return default;
            }
        }
    }
    
}

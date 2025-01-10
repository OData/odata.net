namespace __Generated.Trancsribers.Rules
{
    public sealed class _HEXDIGTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Rules._HEXDIG>
    {
        private _HEXDIGTranscriber()
        {
        }
        
        public static _HEXDIGTranscriber Instance { get; } = new _HEXDIGTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Rules._HEXDIG value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __Generated.CstNodes.Rules._HEXDIG.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._HEXDIG._DIGIT node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(node._DIGIT_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._HEXDIG._ʺx41ʺ node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Inners._ʺx41ʺTranscriber.Instance.Transcribe(node._ʺx41ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._HEXDIG._ʺx42ʺ node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Inners._ʺx42ʺTranscriber.Instance.Transcribe(node._ʺx42ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._HEXDIG._ʺx43ʺ node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Inners._ʺx43ʺTranscriber.Instance.Transcribe(node._ʺx43ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._HEXDIG._ʺx44ʺ node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Inners._ʺx44ʺTranscriber.Instance.Transcribe(node._ʺx44ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._HEXDIG._ʺx45ʺ node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Inners._ʺx45ʺTranscriber.Instance.Transcribe(node._ʺx45ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._HEXDIG._ʺx46ʺ node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Inners._ʺx46ʺTranscriber.Instance.Transcribe(node._ʺx46ʺ_1, context);

return default;
            }
        }
    }
    
}

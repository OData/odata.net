namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _HEXDIGTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._HEXDIG>
    {
        private _HEXDIGTranscriber()
        {
        }
        
        public static _HEXDIGTranscriber Instance { get; } = new _HEXDIGTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._HEXDIG value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._HEXDIG.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._HEXDIG._DIGIT node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(node._DIGIT_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._HEXDIG._AⲻtoⲻF node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._AⲻtoⲻFTranscriber.Instance.Transcribe(node._AⲻtoⲻF_1, context);

return default;
            }
        }
    }
    
}

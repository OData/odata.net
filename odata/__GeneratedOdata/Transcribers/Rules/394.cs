namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _ls32Transcriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._ls32>
    {
        private _ls32Transcriber()
        {
        }
        
        public static _ls32Transcriber Instance { get; } = new _ls32Transcriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._ls32 value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._ls32.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._ls32._Ⲥh16_ʺx3Aʺ_h16Ↄ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._Ⲥh16_ʺx3Aʺ_h16ↃTranscriber.Instance.Transcribe(node._Ⲥh16_ʺx3Aʺ_h16Ↄ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._ls32._IPv4address node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._IPv4addressTranscriber.Instance.Transcribe(node._IPv4address_1, context);

return default;
            }
        }
    }
    
}
namespace __Generated.Trancsribers.Rules
{
    public sealed class _BITTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Rules._BIT>
    {
        private _BITTranscriber()
        {
        }
        
        public static _BITTranscriber Instance { get; } = new _BITTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Rules._BIT value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __Generated.CstNodes.Rules._BIT.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._BIT._ʺx30ʺ node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Inners._ʺx30ʺTranscriber.Instance.Transcribe(node._ʺx30ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._BIT._ʺx31ʺ node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Inners._ʺx31ʺTranscriber.Instance.Transcribe(node._ʺx31ʺ_1, context);

return default;
            }
        }
    }
    
}

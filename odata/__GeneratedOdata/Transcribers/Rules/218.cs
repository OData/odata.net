namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _escapeTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._escape>
    {
        private _escapeTranscriber()
        {
        }
        
        public static _escapeTranscriber Instance { get; } = new _escapeTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._escape value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._escape.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._escape._ʺx5Cʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx5CʺTranscriber.Instance.Transcribe(node._ʺx5Cʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._escape._ʺx25x35x43ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx25x35x43ʺTranscriber.Instance.Transcribe(node._ʺx25x35x43ʺ_1, context);

return default;
            }
        }
    }
    
}

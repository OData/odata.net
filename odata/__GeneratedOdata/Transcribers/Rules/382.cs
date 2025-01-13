namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _CLOSETranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._CLOSE>
    {
        private _CLOSETranscriber()
        {
        }
        
        public static _CLOSETranscriber Instance { get; } = new _CLOSETranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._CLOSE value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._CLOSE.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._CLOSE._ʺx29ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx29ʺTranscriber.Instance.Transcribe(node._ʺx29ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._CLOSE._ʺx25x32x39ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx25x32x39ʺTranscriber.Instance.Transcribe(node._ʺx25x32x39ʺ_1, context);

return default;
            }
        }
    }
    
}

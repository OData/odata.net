namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _STARTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._STAR>
    {
        private _STARTranscriber()
        {
        }
        
        public static _STARTranscriber Instance { get; } = new _STARTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._STAR value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._STAR.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._STAR._ʺx2Aʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx2AʺTranscriber.Instance.Transcribe(node._ʺx2Aʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._STAR._ʺx25x32x41ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx25x32x41ʺTranscriber.Instance.Transcribe(node._ʺx25x32x41ʺ_1, context);

return default;
            }
        }
    }
    
}
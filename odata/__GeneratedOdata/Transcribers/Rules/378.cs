namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _SEMITranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._SEMI>
    {
        private _SEMITranscriber()
        {
        }
        
        public static _SEMITranscriber Instance { get; } = new _SEMITranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._SEMI value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._SEMI.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._SEMI._ʺx3Bʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx3BʺTranscriber.Instance.Transcribe(node._ʺx3Bʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._SEMI._ʺx25x33x42ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx25x33x42ʺTranscriber.Instance.Transcribe(node._ʺx25x33x42ʺ_1, context);

return default;
            }
        }
    }
    
}
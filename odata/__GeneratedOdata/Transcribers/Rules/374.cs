namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _COLONTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._COLON>
    {
        private _COLONTranscriber()
        {
        }
        
        public static _COLONTranscriber Instance { get; } = new _COLONTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._COLON value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._COLON.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._COLON._ʺx3Aʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx3AʺTranscriber.Instance.Transcribe(node._ʺx3Aʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._COLON._ʺx25x33x41ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx25x33x41ʺTranscriber.Instance.Transcribe(node._ʺx25x33x41ʺ_1, context);

return default;
            }
        }
    }
    
}
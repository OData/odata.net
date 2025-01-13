namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _COMMATranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._COMMA>
    {
        private _COMMATranscriber()
        {
        }
        
        public static _COMMATranscriber Instance { get; } = new _COMMATranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._COMMA value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._COMMA.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._COMMA._ʺx2Cʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx2CʺTranscriber.Instance.Transcribe(node._ʺx2Cʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._COMMA._ʺx25x32x43ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx25x32x43ʺTranscriber.Instance.Transcribe(node._ʺx25x32x43ʺ_1, context);

return default;
            }
        }
    }
    
}

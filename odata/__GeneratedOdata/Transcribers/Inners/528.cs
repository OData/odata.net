namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _ʺx31ʺⳆʺx32ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ>
    {
        private _ʺx31ʺⳆʺx32ʺTranscriber()
        {
        }
        
        public static _ʺx31ʺⳆʺx32ʺTranscriber Instance { get; } = new _ʺx31ʺⳆʺx32ʺTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ._ʺx31ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx31ʺTranscriber.Instance.Transcribe(node._ʺx31ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ._ʺx32ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx32ʺTranscriber.Instance.Transcribe(node._ʺx32ʺ_1, context);

return default;
            }
        }
    }
    
}

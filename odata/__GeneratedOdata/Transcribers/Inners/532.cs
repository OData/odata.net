namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ>
    {
        private _ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺTranscriber()
        {
        }
        
        public static _ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺTranscriber Instance { get; } = new _ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx30ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx30ʺTranscriber.Instance.Transcribe(node._ʺx30ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx31ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx31ʺTranscriber.Instance.Transcribe(node._ʺx31ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx32ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx32ʺTranscriber.Instance.Transcribe(node._ʺx32ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx33ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx33ʺTranscriber.Instance.Transcribe(node._ʺx33ʺ_1, context);

return default;
            }
        }
    }
    
}
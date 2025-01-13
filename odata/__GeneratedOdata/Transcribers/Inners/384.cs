namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _ʺx5BʺⳆʺx25x35x42ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ>
    {
        private _ʺx5BʺⳆʺx25x35x42ʺTranscriber()
        {
        }
        
        public static _ʺx5BʺⳆʺx25x35x42ʺTranscriber Instance { get; } = new _ʺx5BʺⳆʺx25x35x42ʺTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ._ʺx5Bʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx5BʺTranscriber.Instance.Transcribe(node._ʺx5Bʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ._ʺx25x35x42ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx25x35x42ʺTranscriber.Instance.Transcribe(node._ʺx25x35x42ʺ_1, context);

return default;
            }
        }
    }
    
}

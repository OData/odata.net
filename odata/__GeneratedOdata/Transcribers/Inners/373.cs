namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _ʺx7BʺⳆʺx25x37x42ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._ʺx7BʺⳆʺx25x37x42ʺ>
    {
        private _ʺx7BʺⳆʺx25x37x42ʺTranscriber()
        {
        }
        
        public static _ʺx7BʺⳆʺx25x37x42ʺTranscriber Instance { get; } = new _ʺx7BʺⳆʺx25x37x42ʺTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._ʺx7BʺⳆʺx25x37x42ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._ʺx7BʺⳆʺx25x37x42ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx7BʺⳆʺx25x37x42ʺ._ʺx7Bʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx7BʺTranscriber.Instance.Transcribe(node._ʺx7Bʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx7BʺⳆʺx25x37x42ʺ._ʺx25x37x42ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx25x37x42ʺTranscriber.Instance.Transcribe(node._ʺx25x37x42ʺ_1, context);

return default;
            }
        }
    }
    
}

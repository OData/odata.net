namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ>
    {
        private _ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺTranscriber()
        {
        }
        
        public static _ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺTranscriber Instance { get; } = new _ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Inners._ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ._ʺx68x74x74x70x73ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx68x74x74x70x73ʺTranscriber.Instance.Transcribe(node._ʺx68x74x74x70x73ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ._ʺx68x74x74x70ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx68x74x74x70ʺTranscriber.Instance.Transcribe(node._ʺx68x74x74x70ʺ_1, context);

return default;
            }
        }
    }
    
}

namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ>
    {
        private _ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺTranscriber()
        {
        }
        
        public static _ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺTranscriber Instance { get; } = new _ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ._ʺx24x65x78x70x61x6Ex64ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx24x65x78x70x61x6Ex64ʺTranscriber.Instance.Transcribe(node._ʺx24x65x78x70x61x6Ex64ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ._ʺx65x78x70x61x6Ex64ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx65x78x70x61x6Ex64ʺTranscriber.Instance.Transcribe(node._ʺx65x78x70x61x6Ex64ʺ_1, context);

return default;
            }
        }
    }
    
}

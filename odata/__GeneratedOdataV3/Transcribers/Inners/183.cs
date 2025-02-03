namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _ʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._ʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺ>
    {
        private _ʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺTranscriber()
        {
        }
        
        public static _ʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺTranscriber Instance { get; } = new _ʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._ʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Inners._ʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺ._ʺx24x73x65x61x72x63x68ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx24x73x65x61x72x63x68ʺTranscriber.Instance.Transcribe(node._ʺx24x73x65x61x72x63x68ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺ._ʺx73x65x61x72x63x68ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx73x65x61x72x63x68ʺTranscriber.Instance.Transcribe(node._ʺx73x65x61x72x63x68ʺ_1, context);

return default;
            }
        }
    }
    
}

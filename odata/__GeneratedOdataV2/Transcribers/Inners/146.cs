namespace __GeneratedOdataV2.Trancsribers.Inners
{
    public sealed class _ʺx61x73x63ʺⳆʺx64x65x73x63ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺ>
    {
        private _ʺx61x73x63ʺⳆʺx64x65x73x63ʺTranscriber()
        {
        }
        
        public static _ʺx61x73x63ʺⳆʺx64x65x73x63ʺTranscriber Instance { get; } = new _ʺx61x73x63ʺⳆʺx64x65x73x63ʺTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV2.CstNodes.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺ._ʺx61x73x63ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx61x73x63ʺTranscriber.Instance.Transcribe(node._ʺx61x73x63ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺ._ʺx64x65x73x63ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx64x65x73x63ʺTranscriber.Instance.Transcribe(node._ʺx64x65x73x63ʺ_1, context);

return default;
            }
        }
    }
    
}

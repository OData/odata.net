namespace __GeneratedOdataV2.Trancsribers.Inners
{
    public sealed class _ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Inners._ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ>
    {
        private _ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺTranscriber()
        {
        }
        
        public static _ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺTranscriber Instance { get; } = new _ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Inners._ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV2.CstNodes.Inners._ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Inners._ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ._ʺx24x73x6Bx69x70ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx24x73x6Bx69x70ʺTranscriber.Instance.Transcribe(node._ʺx24x73x6Bx69x70ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Inners._ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ._ʺx73x6Bx69x70ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx73x6Bx69x70ʺTranscriber.Instance.Transcribe(node._ʺx73x6Bx69x70ʺ_1, context);

return default;
            }
        }
    }
    
}

namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ>
    {
        private _ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺTranscriber()
        {
        }
        
        public static _ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺTranscriber Instance { get; } = new _ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ._ʺx24x69x6Ex64x65x78ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx24x69x6Ex64x65x78ʺTranscriber.Instance.Transcribe(node._ʺx24x69x6Ex64x65x78ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ._ʺx69x6Ex64x65x78ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx69x6Ex64x65x78ʺTranscriber.Instance.Transcribe(node._ʺx69x6Ex64x65x78ʺ_1, context);

return default;
            }
        }
    }
    
}

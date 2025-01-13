namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _ʺx2DʺⳆʺx2BʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._ʺx2DʺⳆʺx2Bʺ>
    {
        private _ʺx2DʺⳆʺx2BʺTranscriber()
        {
        }
        
        public static _ʺx2DʺⳆʺx2BʺTranscriber Instance { get; } = new _ʺx2DʺⳆʺx2BʺTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._ʺx2DʺⳆʺx2Bʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._ʺx2DʺⳆʺx2Bʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx2DʺⳆʺx2Bʺ._ʺx2Dʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx2DʺTranscriber.Instance.Transcribe(node._ʺx2Dʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx2DʺⳆʺx2Bʺ._ʺx2Bʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx2BʺTranscriber.Instance.Transcribe(node._ʺx2Bʺ_1, context);

return default;
            }
        }
    }
    
}

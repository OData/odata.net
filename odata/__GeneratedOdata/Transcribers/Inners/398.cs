namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _ʺx2FʺⳆʺx25x32x46ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._ʺx2FʺⳆʺx25x32x46ʺ>
    {
        private _ʺx2FʺⳆʺx25x32x46ʺTranscriber()
        {
        }
        
        public static _ʺx2FʺⳆʺx25x32x46ʺTranscriber Instance { get; } = new _ʺx2FʺⳆʺx25x32x46ʺTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._ʺx2FʺⳆʺx25x32x46ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._ʺx2FʺⳆʺx25x32x46ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx2FʺⳆʺx25x32x46ʺ._ʺx2Fʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(node._ʺx2Fʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx2FʺⳆʺx25x32x46ʺ._ʺx25x32x46ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx25x32x46ʺTranscriber.Instance.Transcribe(node._ʺx25x32x46ʺ_1, context);

return default;
            }
        }
    }
    
}

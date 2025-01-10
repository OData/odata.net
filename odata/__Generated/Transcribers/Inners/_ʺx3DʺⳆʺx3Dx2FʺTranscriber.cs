namespace __Generated.Trancsribers.Inners
{
    public sealed class _ʺx3DʺⳆʺx3Dx2FʺTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Inners._ʺx3DʺⳆʺx3Dx2Fʺ>
    {
        private _ʺx3DʺⳆʺx3Dx2FʺTranscriber()
        {
        }
        
        public static _ʺx3DʺⳆʺx3Dx2FʺTranscriber Instance { get; } = new _ʺx3DʺⳆʺx3Dx2FʺTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Inners._ʺx3DʺⳆʺx3Dx2Fʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __Generated.CstNodes.Inners._ʺx3DʺⳆʺx3Dx2Fʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Inners._ʺx3DʺⳆʺx3Dx2Fʺ._ʺx3Dʺ node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Inners._ʺx3DʺTranscriber.Instance.Transcribe(node._ʺx3Dʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Inners._ʺx3DʺⳆʺx3Dx2Fʺ._ʺx3Dx2Fʺ node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Inners._ʺx3Dx2FʺTranscriber.Instance.Transcribe(node._ʺx3Dx2Fʺ_1, context);

return default;
            }
        }
    }
    
}

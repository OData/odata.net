namespace __Generated.Trancsribers.Inners
{
    public sealed class _ALPHAⳆDIGITⳆʺx2DʺTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2Dʺ>
    {
        private _ALPHAⳆDIGITⳆʺx2DʺTranscriber()
        {
        }
        
        public static _ALPHAⳆDIGITⳆʺx2DʺTranscriber Instance { get; } = new _ALPHAⳆDIGITⳆʺx2DʺTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2Dʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __Generated.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2Dʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2Dʺ._ALPHA node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Rules._ALPHATranscriber.Instance.Transcribe(node._ALPHA_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2Dʺ._DIGIT node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(node._DIGIT_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2Dʺ._ʺx2Dʺ node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Inners._ʺx2DʺTranscriber.Instance.Transcribe(node._ʺx2Dʺ_1, context);

return default;
            }
        }
    }
    
}

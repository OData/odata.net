namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ>
    {
        private _ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺTranscriber()
        {
        }
        
        public static _ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺTranscriber Instance { get; } = new _ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ALPHA node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._ALPHATranscriber.Instance.Transcribe(node._ALPHA_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._DIGIT node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(node._DIGIT_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Bʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx2BʺTranscriber.Instance.Transcribe(node._ʺx2Bʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Dʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx2DʺTranscriber.Instance.Transcribe(node._ʺx2Dʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Eʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(node._ʺx2Eʺ_1, context);

return default;
            }
        }
    }
    
}

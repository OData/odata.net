namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded>
    {
        private _ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedTranscriber()
        {
        }
        
        public static _ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedTranscriber Instance { get; } = new _ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._ALPHA node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._ALPHATranscriber.Instance.Transcribe(node._ALPHA_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._DIGIT node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(node._DIGIT_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._COMMA node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(node._COMMA_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._ʺx2Eʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(node._ʺx2Eʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._pctⲻencoded node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._pctⲻencodedTranscriber.Instance.Transcribe(node._pctⲻencoded_1, context);

return default;
            }
        }
    }
    
}

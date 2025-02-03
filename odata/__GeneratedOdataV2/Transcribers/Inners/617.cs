namespace __GeneratedOdataV2.Trancsribers.Inners
{
    public sealed class _SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ>
    {
        private _SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺTranscriber()
        {
        }
        
        public static _SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺTranscriber Instance { get; } = new _SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV2.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._SP node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._SPTranscriber.Instance.Transcribe(node._SP_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._HTAB node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._HTABTranscriber.Instance.Transcribe(node._HTAB_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._ʺx25x32x30ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx25x32x30ʺTranscriber.Instance.Transcribe(node._ʺx25x32x30ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._ʺx25x30x39ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx25x30x39ʺTranscriber.Instance.Transcribe(node._ʺx25x30x39ʺ_1, context);

return default;
            }
        }
    }
    
}

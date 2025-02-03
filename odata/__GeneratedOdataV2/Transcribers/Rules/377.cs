namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _SIGNTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._SIGN>
    {
        private _SIGNTranscriber()
        {
        }
        
        public static _SIGNTranscriber Instance { get; } = new _SIGNTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._SIGN value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV2.CstNodes.Rules._SIGN.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._SIGN._ʺx2Bʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx2BʺTranscriber.Instance.Transcribe(node._ʺx2Bʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._SIGN._ʺx25x32x42ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx25x32x42ʺTranscriber.Instance.Transcribe(node._ʺx25x32x42ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._SIGN._ʺx2Dʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx2DʺTranscriber.Instance.Transcribe(node._ʺx2Dʺ_1, context);

return default;
            }
        }
    }
    
}

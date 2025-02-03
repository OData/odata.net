namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _ʺx7DʺⳆʺx25x37x44ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._ʺx7DʺⳆʺx25x37x44ʺ>
    {
        private _ʺx7DʺⳆʺx25x37x44ʺTranscriber()
        {
        }
        
        public static _ʺx7DʺⳆʺx25x37x44ʺTranscriber Instance { get; } = new _ʺx7DʺⳆʺx25x37x44ʺTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._ʺx7DʺⳆʺx25x37x44ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Inners._ʺx7DʺⳆʺx25x37x44ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ʺx7DʺⳆʺx25x37x44ʺ._ʺx7Dʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx7DʺTranscriber.Instance.Transcribe(node._ʺx7Dʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ʺx7DʺⳆʺx25x37x44ʺ._ʺx25x37x44ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx25x37x44ʺTranscriber.Instance.Transcribe(node._ʺx25x37x44ʺ_1, context);

return default;
            }
        }
    }
    
}

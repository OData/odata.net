namespace __GeneratedOdataV2.Trancsribers.Inners
{
    public sealed class _ʺx5DʺⳆʺx25x35x44ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Inners._ʺx5DʺⳆʺx25x35x44ʺ>
    {
        private _ʺx5DʺⳆʺx25x35x44ʺTranscriber()
        {
        }
        
        public static _ʺx5DʺⳆʺx25x35x44ʺTranscriber Instance { get; } = new _ʺx5DʺⳆʺx25x35x44ʺTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Inners._ʺx5DʺⳆʺx25x35x44ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV2.CstNodes.Inners._ʺx5DʺⳆʺx25x35x44ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Inners._ʺx5DʺⳆʺx25x35x44ʺ._ʺx5Dʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx5DʺTranscriber.Instance.Transcribe(node._ʺx5Dʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Inners._ʺx5DʺⳆʺx25x35x44ʺ._ʺx25x35x44ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx25x35x44ʺTranscriber.Instance.Transcribe(node._ʺx25x35x44ʺ_1, context);

return default;
            }
        }
    }
    
}

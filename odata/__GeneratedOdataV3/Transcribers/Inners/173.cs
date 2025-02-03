namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _ʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._ʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺ>
    {
        private _ʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺTranscriber()
        {
        }
        
        public static _ʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺTranscriber Instance { get; } = new _ʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._ʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Inners._ʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺ._ʺx24x63x6Fx75x6Ex74ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx24x63x6Fx75x6Ex74ʺTranscriber.Instance.Transcribe(node._ʺx24x63x6Fx75x6Ex74ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺ._ʺx63x6Fx75x6Ex74ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx63x6Fx75x6Ex74ʺTranscriber.Instance.Transcribe(node._ʺx63x6Fx75x6Ex74ʺ_1, context);

return default;
            }
        }
    }
    
}

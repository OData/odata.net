namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ>
    {
        private _ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺTranscriber()
        {
        }
        
        public static _ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺTranscriber Instance { get; } = new _ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Inners._ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ._ʺx24x6Fx72x64x65x72x62x79ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx24x6Fx72x64x65x72x62x79ʺTranscriber.Instance.Transcribe(node._ʺx24x6Fx72x64x65x72x62x79ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ._ʺx6Fx72x64x65x72x62x79ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx6Fx72x64x65x72x62x79ʺTranscriber.Instance.Transcribe(node._ʺx6Fx72x64x65x72x62x79ʺ_1, context);

return default;
            }
        }
    }
    
}

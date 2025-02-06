namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ>
    {
        private _ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺTranscriber()
        {
        }
        
        public static _ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺTranscriber Instance { get; } = new _ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Inners._ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ._ʺx24x73x65x6Cx65x63x74ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx24x73x65x6Cx65x63x74ʺTranscriber.Instance.Transcribe(node._ʺx24x73x65x6Cx65x63x74ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ._ʺx73x65x6Cx65x63x74ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx73x65x6Cx65x63x74ʺTranscriber.Instance.Transcribe(node._ʺx73x65x6Cx65x63x74ʺ_1, context);

return default;
            }
        }
    }
    
}

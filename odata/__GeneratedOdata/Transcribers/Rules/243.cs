namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _abstractSpatialTypeNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._abstractSpatialTypeName>
    {
        private _abstractSpatialTypeNameTranscriber()
        {
        }
        
        public static _abstractSpatialTypeNameTranscriber Instance { get; } = new _abstractSpatialTypeNameTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._abstractSpatialTypeName value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._abstractSpatialTypeName.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._abstractSpatialTypeName._ʺx47x65x6Fx67x72x61x70x68x79ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx47x65x6Fx67x72x61x70x68x79ʺTranscriber.Instance.Transcribe(node._ʺx47x65x6Fx67x72x61x70x68x79ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._abstractSpatialTypeName._ʺx47x65x6Fx6Dx65x74x72x79ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx47x65x6Fx6Dx65x74x72x79ʺTranscriber.Instance.Transcribe(node._ʺx47x65x6Fx6Dx65x74x72x79ʺ_1, context);

return default;
            }
        }
    }
    
}
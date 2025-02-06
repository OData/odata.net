namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _qualifiedTypeNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._qualifiedTypeName>
    {
        private _qualifiedTypeNameTranscriber()
        {
        }
        
        public static _qualifiedTypeNameTranscriber Instance { get; } = new _qualifiedTypeNameTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._qualifiedTypeName value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Rules._qualifiedTypeName.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._qualifiedTypeName._singleQualifiedTypeName node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._singleQualifiedTypeNameTranscriber.Instance.Transcribe(node._singleQualifiedTypeName_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._qualifiedTypeName._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_OPEN_singleQualifiedTypeName_CLOSE node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6EʺTranscriber.Instance.Transcribe(node._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_1, context);
__GeneratedOdataV4.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(node._OPEN_1, context);
__GeneratedOdataV4.Trancsribers.Rules._singleQualifiedTypeNameTranscriber.Instance.Transcribe(node._singleQualifiedTypeName_1, context);
__GeneratedOdataV4.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(node._CLOSE_1, context);

return default;
            }
        }
    }
    
}

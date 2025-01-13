namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _qualifiedComplexTypeNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._qualifiedComplexTypeName>
    {
        private _qualifiedComplexTypeNameTranscriber()
        {
        }
        
        public static _qualifiedComplexTypeNameTranscriber Instance { get; } = new _qualifiedComplexTypeNameTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._qualifiedComplexTypeName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._complexTypeNameTranscriber.Instance.Transcribe(value._complexTypeName_1, builder);

        }
    }
    
}

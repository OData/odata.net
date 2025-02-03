namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _qualifiedComplexTypeNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._qualifiedComplexTypeName>
    {
        private _qualifiedComplexTypeNameTranscriber()
        {
        }
        
        public static _qualifiedComplexTypeNameTranscriber Instance { get; } = new _qualifiedComplexTypeNameTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._qualifiedComplexTypeName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdataV2.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._complexTypeNameTranscriber.Instance.Transcribe(value._complexTypeName_1, builder);

        }
    }
    
}

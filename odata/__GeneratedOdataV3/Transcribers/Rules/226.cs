namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _qualifiedComplexTypeNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._qualifiedComplexTypeName>
    {
        private _qualifiedComplexTypeNameTranscriber()
        {
        }
        
        public static _qualifiedComplexTypeNameTranscriber Instance { get; } = new _qualifiedComplexTypeNameTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._qualifiedComplexTypeName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdataV3.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._complexTypeNameTranscriber.Instance.Transcribe(value._complexTypeName_1, builder);

        }
    }
    
}

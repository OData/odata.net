namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _qualifiedEntityTypeNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._qualifiedEntityTypeName>
    {
        private _qualifiedEntityTypeNameTranscriber()
        {
        }
        
        public static _qualifiedEntityTypeNameTranscriber Instance { get; } = new _qualifiedEntityTypeNameTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._qualifiedEntityTypeName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._entityTypeNameTranscriber.Instance.Transcribe(value._entityTypeName_1, builder);

        }
    }
    
}

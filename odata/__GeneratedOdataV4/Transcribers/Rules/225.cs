namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _qualifiedEntityTypeNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._qualifiedEntityTypeName>
    {
        private _qualifiedEntityTypeNameTranscriber()
        {
        }
        
        public static _qualifiedEntityTypeNameTranscriber Instance { get; } = new _qualifiedEntityTypeNameTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._qualifiedEntityTypeName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdataV4.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._entityTypeNameTranscriber.Instance.Transcribe(value._entityTypeName_1, builder);

        }
    }
    
}

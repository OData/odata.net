namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _qualifiedTypeDefinitionNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._qualifiedTypeDefinitionName>
    {
        private _qualifiedTypeDefinitionNameTranscriber()
        {
        }
        
        public static _qualifiedTypeDefinitionNameTranscriber Instance { get; } = new _qualifiedTypeDefinitionNameTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._qualifiedTypeDefinitionName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdataV2.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._typeDefinitionNameTranscriber.Instance.Transcribe(value._typeDefinitionName_1, builder);

        }
    }
    
}

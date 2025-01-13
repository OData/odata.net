namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _qualifiedTypeDefinitionNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._qualifiedTypeDefinitionName>
    {
        private _qualifiedTypeDefinitionNameTranscriber()
        {
        }
        
        public static _qualifiedTypeDefinitionNameTranscriber Instance { get; } = new _qualifiedTypeDefinitionNameTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._qualifiedTypeDefinitionName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._typeDefinitionNameTranscriber.Instance.Transcribe(value._typeDefinitionName_1, builder);

        }
    }
    
}

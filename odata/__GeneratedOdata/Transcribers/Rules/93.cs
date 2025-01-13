namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _allOperationsInSchemaTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._allOperationsInSchema>
    {
        private _allOperationsInSchemaTranscriber()
        {
        }
        
        public static _allOperationsInSchemaTranscriber Instance { get; } = new _allOperationsInSchemaTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._allOperationsInSchema value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._STARTranscriber.Instance.Transcribe(value._STAR_1, builder);

        }
    }
    
}

namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _allOperationsInSchemaTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._allOperationsInSchema>
    {
        private _allOperationsInSchemaTranscriber()
        {
        }
        
        public static _allOperationsInSchemaTranscriber Instance { get; } = new _allOperationsInSchemaTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._allOperationsInSchema value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdataV4.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._STARTranscriber.Instance.Transcribe(value._STAR_1, builder);

        }
    }
    
}

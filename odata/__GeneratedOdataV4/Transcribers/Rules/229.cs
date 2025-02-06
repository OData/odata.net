namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _namespaceTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._namespace>
    {
        private _namespaceTranscriber()
        {
        }
        
        public static _namespaceTranscriber Instance { get; } = new _namespaceTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._namespace value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._namespacePartTranscriber.Instance.Transcribe(value._namespacePart_1, builder);
foreach (var _Ⲥʺx2Eʺ_namespacePartↃ_1 in value._Ⲥʺx2Eʺ_namespacePartↃ_1)
{
Inners._Ⲥʺx2Eʺ_namespacePartↃTranscriber.Instance.Transcribe(_Ⲥʺx2Eʺ_namespacePartↃ_1, builder);
}

        }
    }
    
}

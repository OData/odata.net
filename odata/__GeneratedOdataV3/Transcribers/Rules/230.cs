namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _namespacePartTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._namespacePart>
    {
        private _namespacePartTranscriber()
        {
        }
        
        public static _namespacePartTranscriber Instance { get; } = new _namespacePartTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._namespacePart value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}

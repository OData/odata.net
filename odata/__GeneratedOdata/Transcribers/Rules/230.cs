namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _namespacePartTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._namespacePart>
    {
        private _namespacePartTranscriber()
        {
        }
        
        public static _namespacePartTranscriber Instance { get; } = new _namespacePartTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._namespacePart value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
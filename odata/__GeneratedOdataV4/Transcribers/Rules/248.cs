namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _primitiveColPropertyTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._primitiveColProperty>
    {
        private _primitiveColPropertyTranscriber()
        {
        }
        
        public static _primitiveColPropertyTranscriber Instance { get; } = new _primitiveColPropertyTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._primitiveColProperty value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}

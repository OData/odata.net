namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _primitiveKeyPropertyTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._primitiveKeyProperty>
    {
        private _primitiveKeyPropertyTranscriber()
        {
        }
        
        public static _primitiveKeyPropertyTranscriber Instance { get; } = new _primitiveKeyPropertyTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._primitiveKeyProperty value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}
